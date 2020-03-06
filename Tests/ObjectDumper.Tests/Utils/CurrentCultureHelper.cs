using System;
using System.Globalization;

#if NET452
using System.Threading;
#endif

namespace ObjectDumping.Tests.Utils
{
    public static class CurrentCultureHelper
    {
        public static IDisposable ChangeCulture(CultureInfo temporaryCultureInfo)
        {
            if (temporaryCultureInfo == null)
            {
                throw new ArgumentNullException(nameof(temporaryCultureInfo));
            }

#if NET452
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            var currentUiCulture = Thread.CurrentThread.CurrentUICulture;
#else
            var currentCulture = CultureInfo.CurrentCulture;
            var currentUiCulture = CultureInfo.CurrentUICulture;
#endif

            var revertCurrentCultureHandler = new CurrentCultureHandler(currentCulture, currentUiCulture);

#if NET452
            Thread.CurrentThread.CurrentCulture = temporaryCultureInfo;
            Thread.CurrentThread.CurrentUICulture = temporaryCultureInfo;
#else
            CultureInfo.CurrentCulture = temporaryCultureInfo;
            CultureInfo.CurrentUICulture = temporaryCultureInfo;
#endif

            CultureInfo.DefaultThreadCurrentCulture = temporaryCultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = temporaryCultureInfo;

            return revertCurrentCultureHandler;
        }
    }

    public class CurrentCultureHandler : IDisposable
    {
        private readonly CultureInfo currentCulture;
        private readonly CultureInfo currentUiCulture;

        internal CurrentCultureHandler(CultureInfo currentCulture, CultureInfo currentUiCulture)
        {
            this.currentCulture = currentCulture;
            this.currentUiCulture = currentUiCulture;
        }

        public void Dispose()
        {
            try
            {
#if NET452
                Thread.CurrentThread.CurrentCulture = currentCulture;
                Thread.CurrentThread.CurrentUICulture = currentCulture;
#else
                CultureInfo.CurrentCulture = currentCulture;
                CultureInfo.CurrentUICulture = currentCulture;
#endif

                CultureInfo.DefaultThreadCurrentCulture = currentCulture;
                CultureInfo.DefaultThreadCurrentUICulture = currentUiCulture;
            }
            catch
            {
            }
        }
    }
}
