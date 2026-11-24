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

        [Fact]
        public void Create_ANewModulator_HasEveryRangeAtZeroToOne()
        {
            var modulator = Create();

            Assert.Equal(0f, modulator.BassRange.Minimum.Value);
            Assert.Equal(1f, modulator.BassRange.Maximum.Value);
            Assert.Equal(0f, modulator.LowerMidRange.Minimum.Value);
            Assert.Equal(1f, modulator.LowerMidRange.Maximum.Value);
            Assert.Equal(0f, modulator.HigherMidRange.Minimum.Value);
            Assert.Equal(1f, modulator.HigherMidRange.Maximum.Value);
            Assert.Equal(0f, modulator.HighRange.Minimum.Value);
            Assert.Equal(1f, modulator.HighRange.Maximum.Value);
        }

        [Fact]
        public void ResetAll_ARange_GoesBackToZeroToOne()
        {
            var modulator = Create();
            modulator.HighRange.Minimum.Value = -3f;
            modulator.HighRange.Maximum.Value = 3f;

            modulator.HighRange.ResetAllCommand.Execute(null);

            Assert.Equal(0f, modulator.HighRange.Minimum.Value);
            Assert.Equal(1f, modulator.HighRange.Maximum.Value);
        }
    }
}
