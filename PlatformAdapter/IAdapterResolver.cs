using System;

namespace PlatformAdapter
{
    public interface IAdapterResolver
    {
        object Resolve(Type interfaceType, bool throwIfNotFound, object[] args);

        /// <summary>
        /// Resolves a class type based on the given interface type.
        /// </summary>
        /// <param name="interfaceType">The interface type for which the implementation type is looked up.</param>
        /// <param name="throwIfNotFound">Throws an exception if class type cannot be found.</param>
        /// <returns>The class type (if it can be found) or null (if it cannot be found and throwIfNotFound is false).</returns>
        Type ResolveClassType(Type interfaceType, bool throwIfNotFound = true);

        IRegistrationConvention RegistrationConvention { get; set; }
    }
}