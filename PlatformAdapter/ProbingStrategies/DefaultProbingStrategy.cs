using System;
using System.Reflection;

using Guards;

namespace CrossPlatformAdapter.ProbingStrategies
{
    /// <summary>
    /// DefaultProbingStrategy is a registration convention which probes inside the assembly
    /// where the given interface type was found.
    /// Platform-specific types are made of interface type name excluding the leading string "I".
    /// </summary>
    public class DefaultProbingStrategy : IProbingStrategy
    {
        public virtual string PlatformNamingConvention(AssemblyName assemblyName)
        {
            return assemblyName.Name;
        }

        public virtual string InterfaceToClassNamingConvention(Type interfaceType)
        {
            Guard.ArgumentMustBeInterface(interfaceType);
            Guard.ArgumentIsTrue(() => interfaceType.DeclaringType == null);
            Guard.ArgumentIsTrue(() => interfaceType.Name.StartsWith("I", StringComparison.Ordinal));

            return string.Format("{0}.{1}", interfaceType.Namespace, interfaceType.Name.Substring(1));
        }
    }
}
