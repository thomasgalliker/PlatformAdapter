using System;

namespace CrossPlatformAdapter
{
    public interface IAdapterResolver
    {
        /// <summary>
        /// Resolves a platform-specific object for a given interface type TInterface
        /// using the configured registration convention.
        /// </summary>
        /// <typeparam name="TInterface">
        /// The platform-agnostic interface type 
        /// for which we want to find a platform-specific implementation.
        /// </typeparam>
        /// <param name="args">Constructor parameters which are eventually needed to create the platform-specific object.</param>
        /// <returns>The platform-specific object which implements interface type TInterface.</returns>
        TInterface Resolve<TInterface>(params object[] args);

        /// <summary>
        /// Resolves a platform-specific object for a given interface type TInterface
        /// using the configured registration convention.
        /// </summary>
        /// <param name="interfaceType">
        /// The platform-agnostic interface type 
        /// for which we want to find a platform-specific implementation.
        /// </param>
        /// <param name="args">Constructor parameters which are eventually needed to create the platform-specific object.</param>
        /// <returns>The platform-specific object which implements interface type TInterface.</returns>
        object Resolve(Type interfaceType, params object[] args);

        /// <summary>
        /// Tries to resolve a platform-specific object for a given interface type TInterface
        /// using the configured registration convention.
        /// </summary>
        /// <typeparam name="TInterface">
        /// The platform-agnostic interface type 
        /// for which we want to find a platform-specific implementation.
        /// </typeparam>
        /// <param name="args">Constructor parameters which are eventually needed to create the platform-specific object.</param>
        /// <returns>
        /// The platform-specific object which implements the given interface type.
        /// If the platform-specific assembly or the desired type cannot be found, TryResolve returns null.
        /// </returns>
        TInterface TryResolve<TInterface>(params object[] args);

        /// <summary>
        /// Tries to resolve a platform-specific object for a given interface type TInterface
        /// using the configured registration convention.
        /// </summary>
        /// <param name="interfaceType">
        /// The platform-agnostic interface type 
        /// for which we want to find a platform-specific implementation.
        /// </param>
        /// <param name="args">Constructor parameters which are eventually needed to create the platform-specific object.</param>
        /// <returns>
        /// The platform-specific object which implements the given interface type.
        /// If the platform-specific assembly or the desired type cannot be found, TryResolve returns null.
        /// </returns>
        object TryResolve(Type interfaceType, params object[] args);

        /// <summary>
        /// Resolves a class type based on the given interface type
        /// using the configured registration convention.
        /// </summary>
        /// <typeparam name="TInterface">
        /// The platform-agnostic interface type 
        /// for which we want to find a platform-specific implementation.
        /// </typeparam>
        /// <returns>The platform-specific class type which implements the given interface type.
        /// </returns>
        Type ResolveClassType<TInterface>();

        /// <summary>
        /// Resolves a class type based on the given interface type
        /// using the configured registration convention.
        /// </summary>
        /// <param name="interfaceType">
        /// The platform-agnostic interface type 
        /// for which we want to find a platform-specific implementation.
        /// </param>
        /// <returns>The platform-specific class type which implements the given interface type.
        /// </returns>
        Type ResolveClassType(Type interfaceType);

        /// <summary>
        /// Resolves a class type based on the given interface type
        /// using the configured registration convention.
        /// </summary>
        /// <typeparam name="TInterface">
        /// The platform-agnostic interface type 
        /// for which we want to find a platform-specific implementation.
        /// </typeparam>
        /// <returns>The platform-specific class type which implements the given interface type.
        /// If the platform-specific assembly or the desired type cannot be found, TryResolve returns null.
        /// </returns>
        Type TryResolveClassType<TInterface>();

        /// <summary>
        /// Resolves a class type based on the given interface type
        /// using the configured registration convention.
        /// </summary>
        /// <param name="interfaceType">
        /// The platform-agnostic interface type 
        /// for which we want to find a platform-specific implementation.
        /// </param>
        /// <returns>The platform-specific class type which implements the given interface type.
        /// If the platform-specific assembly or the desired type cannot be found, TryResolve returns null.
        /// </returns>
        Type TryResolveClassType(Type interfaceType);

        /// <summary>
        /// The registration convention to be used to translate the given platform-agnostic interface type
        /// into a platform-dependent class type.
        /// </summary>
        //ICollection<IProbingStrategy> ProbingStrategies { get; set; }
    }
}