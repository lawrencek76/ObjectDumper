using System;
using System.Globalization;
using Xunit;

namespace ObjectDumping.Tests.Utils
{
    [CollectionDefinition(TestCollections.CultureSpecific)]
    public sealed class CultureSpecificFixture : ICollectionFixture<CultureSpecificFixture>
    {
        private readonly IDisposable changeCultureHelper;

        public CultureSpecificFixture()
        {
            var testCulture = new CultureInfo("de-CH");
            changeCultureHelper = CurrentCultureHelper.ChangeCulture(testCulture);
        }

        ~CultureSpecificFixture()
        {
            Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            changeCultureHelper.Dispose();
        }
    }
}
