using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class TrackingModulatorTests
    {
        private static TrackingModulator Create()
        {
            var provider = TestServiceProviderFactory.Create();
            return (TrackingModulator)provider.GetRequiredService<ControlFactory>().Create(typeof(TrackingModulator));
        }

        [Fact]
        public void FromModel_OfToModel_KeepsEveryValue()
        {
            var modulator = Create();
            modulator.Count.Value = 7;
            modulator.X.Value = 1.5f;
            modulator.Y.Value = 2.5f;
            modulator.XRange.Minimum.Value = 10f;
            modulator.XRange.Maximum.Value = 11f;
            modulator.YRange.Minimum.Value = 20f;
            modulator.YRange.Maximum.Value = 21f;

            var copy = Create();
            copy.FromModel(modulator.ToModel());

            Assert.Equal(7, copy.Count.Value);
            Assert.Equal(1.5f, copy.X.Value);
            Assert.Equal(2.5f, copy.Y.Value);
            Assert.Equal(10f, copy.XRange.Minimum.Value);
            Assert.Equal(11f, copy.XRange.Maximum.Value);
            Assert.Equal(20f, copy.YRange.Minimum.Value);
            Assert.Equal(21f, copy.YRange.Maximum.Value);
        }
    }
}
