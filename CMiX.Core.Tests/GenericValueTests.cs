using CMiX.Core.BaseControls;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class GenericValueTests
    {
        [Fact]
        public void SetDefault_SetsBothValueAndOriginalValue()
        {
            var provider = TestServiceProviderFactory.Create();
            var value = provider.GetRequiredService<GenericValue<int>>();

            value.SetDefault(5);

            Assert.Equal(5, value.Value);
            Assert.Equal(5, value.OriginalValue);
        }

        [Fact]
        public void Reset_AfterFromModel_ReturnsToTheSetDefaultValue_NotTheFromModelValue()
        {
            var provider = TestServiceProviderFactory.Create();
            var value = provider.GetRequiredService<GenericValue<int>>();

            value.SetDefault(5);
            value.FromModel(new GenericValueModel<int>(9));
            value.Reset();

            Assert.Equal(5, value.Value);
        }
    }
}
