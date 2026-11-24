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
    }
}
