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

        [Fact]
        public void BeatModulator_IsModulateKind()
        {
            var provider = TestServiceProviderFactory.Create();
            var beatModulator = provider.GetRequiredService<BeatModulator>();

            Assert.Equal(ModulatorKind.Modulate, beatModulator.Kind);
        }

        [Fact]
        public void TrackingModulator_IsResolvableAsIModulator_AndIsSetKind()
        {
            var provider = TestServiceProviderFactory.Create();
            var trackingModulator = provider.GetRequiredService<TrackingModulator>();

            Assert.IsAssignableFrom<IModulator>(trackingModulator);
            Assert.Equal(ModulatorKind.Set, trackingModulator.Kind);
            Assert.Equal(new[] { "Count", "X" }, trackingModulator.OutputNames);
        }

        [Fact]
        public void TrackingModulator_ToModel_FromModel_RoundTripsCountAndX()
        {
            var provider = TestServiceProviderFactory.Create();
            var tracking = provider.GetRequiredService<TrackingModulator>();
            tracking.Count.Value = 7;
            tracking.X.Value = 12.5f;

            var model = tracking.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<TrackingModulator>();
            reloaded.FromModel(model);

            Assert.Equal(7, reloaded.Count.Value);
            Assert.Equal(12.5f, reloaded.X.Value);
        }
    }
}
