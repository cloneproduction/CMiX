using CMiX.Core.BaseControls;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class GenericValueTests
    {
        [Fact]
        public void SetDefaults_CopiesTheValueToOriginalValue()
        {
            var provider = TestServiceProviderFactory.Create();
            var value = provider.GetRequiredService<GenericValue<int>>();

            value.Value = 5;
            value.SetDefaults();

            Assert.Equal(5, value.Value);
            Assert.Equal(5, value.OriginalValue);
        }

        [Fact]
        public void Reset_AfterFromModel_ReturnsToTheDefaultValue_NotTheFromModelValue()
        {
            var provider = TestServiceProviderFactory.Create();
            var value = provider.GetRequiredService<GenericValue<int>>();

            value.Value = 5;
            value.SetDefaults();
            value.FromModel(new GenericValueModel<int>(9));
            value.Reset();

            Assert.Equal(5, value.Value);
        }
    }
}
