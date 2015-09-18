using System.Reflection;

namespace CrossPlatformAdapter.ProbingStrategies
{
    /// <summary>
    /// PlatformProbingStrategy is a registration convention which adds the string ".Platform"
    /// to the platform-agnostic assembly name in order to resolve the platform-specific assembly.
    /// Furthermore, platform-specific types are made of interface type name excluding the leading string "I".
    /// </summary>
    public class PlatformProbingStrategy : DefaultProbingStrategy
    {
        public override string PlatformNamingConvention(AssemblyName assemblyName)
        {
            return string.Format("{0}.Platform", assemblyName.Name);
        }
    }
}
