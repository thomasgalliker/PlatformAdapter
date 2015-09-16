using System;
using System.Reflection;

namespace PlatformAdapter.Tests
{
    /// <summary>
    /// This implementation of IRegistrationConvention allows to probe for stubs in the CrossPlatformLibrary.Tests assembly.
    /// </summary>
    public class TestRegistrationConvention : DefaultRegistrationConvention
    {
        public override string PlatformNamingConvention(AssemblyName assemblyName)
        {
            return string.Format("{0}", assemblyName.Name.Replace("PlatformDemoAbstraction", "PlatformDemoAssembly"));
        }

        public override string InterfaceToClassNamingConvention(Type interfaceType)
        {
            var defaultNamingConvention = base.InterfaceToClassNamingConvention(interfaceType);
            var testNamingConvention = defaultNamingConvention.Replace("PlatformDemoAbstraction", "PlatformDemoAssembly");
            return testNamingConvention;
        }
    }
}
