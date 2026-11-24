using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class PulseTests
    {
        private static Pulse CreatePulse()
        {
            var provider = TestServiceProviderFactory.Create();
            return (Pulse)provider.GetRequiredService<ControlFactory>().Create(typeof(Pulse));
        }

        [Fact]
        public void Create_NewPulse_HasTheDefaultValues()
        {
            var pulse = CreatePulse();

            Assert.Equal(1.0f, pulse.Center.Value);
            Assert.Equal(1.0f, pulse.SlopeWidth.Value);
            Assert.Equal(1.0f, pulse.CenterWidth.Value);
            Assert.False(pulse.Invert.Value);
            Assert.Equal(PulseChannel.Red, pulse.Channel.Value);
            Assert.Equal(3, pulse.Bindables.Count);
        }

        [Fact]
        public void FromModel_OfToModel_KeepsEveryValue()
        {
            var pulse = CreatePulse();
            pulse.Center.Value = 0.5f;
            pulse.SlopeWidth.Value = 0.25f;
            pulse.CenterWidth.Value = 0.75f;
            pulse.Invert.Value = true;
            pulse.Channel.Value = PulseChannel.Alpha;

            var model = pulse.ToModel();
            var reloaded = CreatePulse();
            reloaded.FromModel(model);

            Assert.Equal(0.5f, reloaded.Center.Value);
            Assert.Equal(0.25f, reloaded.SlopeWidth.Value);
            Assert.Equal(0.75f, reloaded.CenterWidth.Value);
            Assert.True(reloaded.Invert.Value);
            Assert.Equal(PulseChannel.Alpha, reloaded.Channel.Value);
        }
    }
}
