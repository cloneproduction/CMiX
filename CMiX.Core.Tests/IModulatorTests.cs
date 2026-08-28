using CMiX.Core.Animations;
using CMiX.Core.Modulation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class IModulatorTests
    {
        [Fact]
        public void BeatModifier_IsResolvableAsIModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var beatModifier = provider.GetRequiredService<BeatModifier>();

            Assert.IsAssignableFrom<IModulator>(beatModifier);
        }
    }
}
