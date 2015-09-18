using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using CrossPlatformAdapter.Exceptions;
using CrossPlatformAdapter.ProbingStrategies;

using Guards;

namespace CrossPlatformAdapter
{
    // An implementation IAdapterResolver that probes for platforms-specific adapters by dynamically
    // looking for concrete types in platform-specific assemblies.
    public class ProbingAdapterResolver : IAdapterResolver
    {
        private readonly Func<AssemblyName, Assembly> assemblyLoader;
        private readonly object lockObject = new object();

        /// <summary>
        /// Default constructor.
        /// Uses DefaultProbingStrategy in order to resolve platform-specific assemblies.
        /// </summary>
        public ProbingAdapterResolver()
            : this(Assembly.Load, new DefaultProbingStrategy(), new PlatformProbingStrategy())
        {
        }

        public ProbingAdapterResolver(params IProbingStrategy[] probingStrategies)
            : this(Assembly.Load, probingStrategies)
        {
        }

        public ProbingAdapterResolver(Func<AssemblyName, Assembly> assemblyLoader, params IProbingStrategy[] probingStrategies)
        {
            Guard.ArgumentNotNull(() => probingStrategies);
            Guard.ArgumentNotNull(() => assemblyLoader);

            int index = 0;
            this.ProbingStrategies = probingStrategies.ToDictionary(strategy => index++, strategy => strategy);
            this.assemblyLoader = assemblyLoader;
        }

        /// <inheritdoc />
        public Dictionary<int, IProbingStrategy> ProbingStrategies { get; set; }

        /// <inheritdoc />
        public TInterface Resolve<TInterface>(params object[] args)
        {
            return (TInterface)this.Resolve(typeof(TInterface), args);
        }

        /// <inheritdoc />
        public object Resolve(Type interfaceType, params object[] args)
        {
            return this.Resolve(interfaceType, throwIfNotFound: true, args: args);
        }

        public TInterface TryResolve<TInterface>(params object[] args)
        {
            return (TInterface)this.TryResolve(typeof(TInterface), args);
        }

        /// <inheritdoc />
        public object TryResolve(Type interfaceType, params object[] args)
        {
            return this.Resolve(interfaceType, throwIfNotFound: false, args: args);
        }

        /// <inheritdoc />
        private object Resolve(Type interfaceType, bool throwIfNotFound, params object[] args)
        {
            Guard.ArgumentNotNull(() => interfaceType);

            var classType = this.DoResolveClassTypeUsingAllStrategies(interfaceType, throwIfNotFound);
            if (classType == null)
            {
                return null;
            }

            return this.CreateInstance(classType, throwIfNotFound, args);
        }

        private object CreateInstance(Type type, bool throwIfCannotCreate, params object[] args)
        {
            try
            {
                return Activator.CreateInstance(type, args);
            }
            catch
            {
                if (throwIfCannotCreate)
                {
                    throw;
                }
            }

            return null;
        }

        /// <inheritdoc />
        public Type ResolveClassType<TInterface>()
        {
            return this.DoResolveClassTypeUsingAllStrategies(typeof(TInterface));
        }

        /// <inheritdoc />
        public Type ResolveClassType(Type interfaceType)
        {
            return this.DoResolveClassTypeUsingAllStrategies(interfaceType, throwIfNotFound: true);
        }

        /// <inheritdoc />
        public Type TryResolveClassType<TInterface>()
        {
            return this.TryResolveClassType(typeof(TInterface));
        }

        /// <inheritdoc />
        public Type TryResolveClassType(Type interfaceType)
        {
            return this.DoResolveClassTypeUsingAllStrategies(interfaceType, throwIfNotFound: false);
        }

        private Type DoResolveClassTypeUsingAllStrategies(Type interfaceType, bool throwIfNotFound = true)
        {
            var exceptions = new List<Exception>();
            foreach (var probingStrategy in this.ProbingStrategies)
            {
                var resolveResult = this.DoResolveClassType(probingStrategy.Value, interfaceType, throwIfNotFound);
                if (resolveResult.IsSuccessful)
                {
                    return resolveResult.Type;
                }
                
                exceptions.Add(resolveResult.Exception);
            }

            if (throwIfNotFound)
            {
                throw new AggregateException(exceptions);
            }
            
            return null;
        }

        private ProbingResult DoResolveClassType(IProbingStrategy probingStrategy, Type interfaceType, bool throwIfNotFound = true)
        {
            Guard.ArgumentNotNull(() => interfaceType);

            lock (this.lockObject)
            {
                var platformSpecificAssembly = this.ProbeForPlatformSpecificAssembly(probingStrategy, interfaceType);
                if (platformSpecificAssembly == null)
                {
                    string errorMessage = string.Format("Platform-specific assembly provides an implementation for interface {0} could not be found. Make sure your project references all necessary platform-specific assemblies.", interfaceType.FullName);
                    return new ProbingResult(new PlatformSpecificAssemblyNotFoundException(errorMessage));
                }
                else
                {
                    var classType = this.TryConvertInterfaceTypeToClassType(probingStrategy, platformSpecificAssembly, interfaceType);
                    if (classType != null)
                    {
                        return new ProbingResult(classType);
                    }

                    string errorMessage = string.Format("Interface type {0} could not be resolved in assembly {1}.", interfaceType.FullName, platformSpecificAssembly.FullName);
                    return new ProbingResult(new PlatformSpecificTypeNotFoundException(errorMessage));
                }
            }
        }

        private Type TryConvertInterfaceTypeToClassType(IProbingStrategy probingStrategy, Assembly assembly, Type interfaceType)
        {
            string typeName = probingStrategy.InterfaceToClassNamingConvention(interfaceType);
            try
            {
                 return assembly.GetType(typeName);
            }
            catch(Exception ex)
            {
                //TODO GATH: Trace
            }

            try
            {
                return this.GetType().GetTypeInfo().Assembly.GetType(typeName);
            }
            catch(Exception ex)
            {
                //TODO GATH: Trace
            }

            return null;
        }

        private Assembly ProbeForPlatformSpecificAssembly(IProbingStrategy probingStrategy, Type interfaceType)
        {
            AssemblyName assemblyName = new AssemblyName(interfaceType.GetTypeInfo().Assembly.FullName);
            assemblyName.Name = probingStrategy.PlatformNamingConvention(assemblyName);

            Assembly platformSpecificAssembly = null;
            try
            {
                platformSpecificAssembly = this.assemblyLoader(assemblyName);
            }
            catch (Exception)
            {
                // Try again without the SN for WP8
                // HACK...no real strong name support here
                assemblyName.SetPublicKey(null);
                assemblyName.SetPublicKeyToken(null);

                try
                {
                    platformSpecificAssembly = this.assemblyLoader(assemblyName);
                }
                catch (Exception)
                {
                }
            }

            return platformSpecificAssembly;
        }

        private class ProbingResult
        {
            public Type Type { get; private set; }

            public Exception Exception { get; private set; }

            public bool IsSuccessful
            {
                get
                {
                    return this.Type != null;
                }
            }

            public ProbingResult(Type type)
            {
                this.Type = type;
            }

            public ProbingResult(Exception exception)
            {
                this.Exception = exception;
            }
        }
    }
}