using System;
using System.Reflection;

using Guards;

using PlatformAdapter.Exceptions;

namespace PlatformAdapter
{
    // An implementation IAdapterResolver that probes for platforms-specific adapters by dynamically
    // looking for concrete types in platform-specific assemblies.
    public class ProbingAdapterResolver : IAdapterResolver
    {
        private readonly Func<AssemblyName, Assembly> assemblyLoader;
        private readonly object lockObject = new object();

        /// <summary>
        /// Default constructor.
        /// Uses DefaultRegistrationConvention in order to resolve platform-specific assemblies.
        /// </summary>
        public ProbingAdapterResolver()
            : this(new DefaultRegistrationConvention(), Assembly.Load)
        {
        }

        public ProbingAdapterResolver(IRegistrationConvention registrationConvention)
            : this(registrationConvention, Assembly.Load)
        {
        }

        public ProbingAdapterResolver(IRegistrationConvention registrationConvention, Func<AssemblyName, Assembly> assemblyLoader)
        {
            Guard.ArgumentNotNull(() => registrationConvention);
            Guard.ArgumentNotNull(() => assemblyLoader);

            this.RegistrationConvention = registrationConvention;
            this.assemblyLoader = assemblyLoader;
        }

        public IRegistrationConvention RegistrationConvention { get; set; }

        public object Resolve(Type interfaceType, bool throwIfNotFound, object[] args)
        {
            Guard.ArgumentNotNull(() => interfaceType);

            var classType = this.ResolveClassType(interfaceType, throwIfNotFound);
            if (classType == null)
            {
                return null;
            }

            return this.CreateInstance(classType, throwIfNotFound, args);
        }

        private object CreateInstance(Type type, bool throwIfCannotCreate, object[] args)
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

        public Type ResolveClassType(Type interfaceType, bool throwIfNotFound = true)
        {
            Guard.ArgumentNotNull(() => interfaceType);

            lock (this.lockObject)
            {
                var platformSpecificAssembly = this.ProbeForPlatformSpecificAssembly(interfaceType);
                if (platformSpecificAssembly == null)
                {
                    if (throwIfNotFound)
                    {
                        string errorMessage = string.Format("Platform-specific assembly provides an implementation for interface {0} could not be found. Make sure your project references all necessary platform-specific assemblies.", interfaceType.FullName);
                        throw new PlatformSpecificAssemblyNotFoundException(errorMessage);
                    }
                }
                else
                {
                    var classType = this.TryConvertInterfaceTypeToClassType(platformSpecificAssembly, interfaceType);
                    if (classType != null)
                    {
                        return classType;
                    }

                    if (throwIfNotFound)
                    {
                        string errorMessage = string.Format("Type {0} could not be resolved.", interfaceType.FullName);
                        throw new PlatformSpecificTypeNotFoundException(errorMessage);
                    }
                }

                return null;
            }
        }

        private Type TryConvertInterfaceTypeToClassType(Assembly assembly, Type interfaceType)
        {
            string typeName = this.RegistrationConvention.InterfaceToClassNamingConvention(interfaceType);
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

        private Assembly ProbeForPlatformSpecificAssembly(Type interfaceType)
        {
            AssemblyName assemblyName = new AssemblyName(interfaceType.GetTypeInfo().Assembly.FullName);
            assemblyName.Name = this.RegistrationConvention.PlatformNamingConvention(assemblyName);

            Assembly assm = null;
            try
            {
                assm = this.assemblyLoader(assemblyName);
            }
            catch (Exception)
            {
                // Try again without the SN for WP8
                // HACK...no real strong name support here
                assemblyName.SetPublicKey(null);
                assemblyName.SetPublicKeyToken(null);

                try
                {
                    assm = this.assemblyLoader(assemblyName);
                }
                catch (Exception)
                {
                }
            }

            return assm;
        }
    }
}