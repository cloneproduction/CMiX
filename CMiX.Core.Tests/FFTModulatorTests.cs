using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // An FFTModulator: one track index, four band outputs, one range per band.
    public class FFTModulatorTests
    {
        private static FFTModulator Create()
        {
            var provider = TestServiceProviderFactory.Create();
            return (FFTModulator)provider.GetRequiredService<ControlFactory>().Create(typeof(FFTModulator));
        }

        [Fact]
        public void Outputs_OfANewModulator_AreFourNamedFloatBands()
        {
            var modulator = Create();

            Assert.Equal(new[] { "Bass", "LowerMid", "HigherMid", "High" }, modulator.Outputs.Select(o => o.Name));
            Assert.All(modulator.Outputs, o => Assert.IsType<ModulatorOutput<float>>(o));
        }
    }
}
