
using System;
using System.Threading;

namespace PlatformAdapter
{
    public static class PlatformAdapter
    {
        private static readonly Lazy<IAdapterResolver> AdapterResolver = new Lazy<IAdapterResolver>(CreateProbingAdapterResolver, LazyThreadSafetyMode.PublicationOnly);
        private static IAdapterResolver customResolver;

        private static IAdapterResolver CreateProbingAdapterResolver()
        {
            return new ProbingAdapterResolver();
        }

        /// <summary>
        /// Returns a singleton instance of an IAdapterResolver.
        /// </summary>
        public static IAdapterResolver Current
        {
            get
            {
                if (customResolver != null)
                {
                    return customResolver;
                }

                var adapterResolver = AdapterResolver.Value;
                if (adapterResolver != null)
                {
                    return adapterResolver;
                }

                throw new InvalidOperationException("Could not create adapter resolver.");
            }
        }

        /// <summary>
        /// SetResolver can be used to inject another IAdapterResolver instance
        /// for unit testing purposes.
        /// </summary>
        /// <param name="resolver"></param>
        internal static void SetResolver(IAdapterResolver resolver)
        {
            customResolver = resolver;
        }
    }
}
