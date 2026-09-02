using CMiX.Core.Modulation.Modulators;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class IModulatorTests
    {
        [Fact]
        public void BeatModulator_IsResolvableAsIModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var beatModulator = provider.GetRequiredService<BeatModulator>();

            Assert.IsAssignableFrom<IModulator>(beatModulator);
        }
    }
}
