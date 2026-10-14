using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ModulatorDefaultsTests
    {
        [Fact]
        public void LFOModulator_ConstructsAtItsIntendedDefaults()
        {
            var provider = TestServiceProviderFactory.Create();
            var lfo = (LFOModulator)provider.GetRequiredService<ControlFactory>().Create(typeof(LFOModulator));

            Assert.Equal(10.0f, lfo.Period.Value);
            Assert.Equal(-1.0f, lfo.Minimum.Value);
            Assert.Equal(1.0f, lfo.Maximum.Value);

            lfo.Period.Value = 3.0f;
            lfo.Minimum.Value = -5.0f;
            lfo.Maximum.Value = 5.0f;

            lfo.Period.Reset();
            lfo.Minimum.Reset();
            lfo.Maximum.Reset();

            Assert.Equal(10.0f, lfo.Period.Value);
            Assert.Equal(-1.0f, lfo.Minimum.Value);
            Assert.Equal(1.0f, lfo.Maximum.Value);

            Assert.False(lfo.Quantize.IsEnabled.Value);
            Assert.Equal(4, lfo.Quantize.Steps.Value);
        }

        [Fact]
        public void BeatRandomModulator_ConstructsAtItsIntendedDefaults()
        {
            var provider = TestServiceProviderFactory.Create();
            var beatRandom = (BeatRandomModulator)provider.GetRequiredService<ControlFactory>().Create(typeof(BeatRandomModulator));

            Assert.Equal(0.0f, beatRandom.Center.Value);
            Assert.Equal(1.0f, beatRandom.Width.Value);

            beatRandom.Center.Value = 2.0f;
            beatRandom.Width.Value = 4.0f;

            beatRandom.Center.Reset();
            beatRandom.Width.Reset();

            Assert.Equal(0.0f, beatRandom.Center.Value);
            Assert.Equal(1.0f, beatRandom.Width.Value);
        }

        [Fact]
        public void BeatLFOModulator_ConstructsAtItsIntendedDefaults()
        {
            var provider = TestServiceProviderFactory.Create();
            var beatLfo = (BeatLFOModulator)provider.GetRequiredService<ControlFactory>().Create(typeof(BeatLFOModulator));

            Assert.Equal(0.0f, beatLfo.Center.Value);
            Assert.Equal(1.0f, beatLfo.Width.Value);

            beatLfo.Center.Value = 2.0f;
            beatLfo.Width.Value = 4.0f;

            beatLfo.Center.Reset();
            beatLfo.Width.Reset();

            Assert.Equal(0.0f, beatLfo.Center.Value);
            Assert.Equal(1.0f, beatLfo.Width.Value);

            Assert.False(beatLfo.Quantize.IsEnabled.Value);
            Assert.Equal(4, beatLfo.Quantize.Steps.Value);

            beatLfo.Quantize.IsEnabled.Value = true;
            beatLfo.Quantize.Steps.Value = 8;

            beatLfo.Quantize.IsEnabled.Reset();
            beatLfo.Quantize.Steps.Reset();

            Assert.False(beatLfo.Quantize.IsEnabled.Value);
            Assert.Equal(4, beatLfo.Quantize.Steps.Value);
        }
    }
}
