using System.Linq;
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

            Assert.Equal(ModulatorKind.Modulate, beatModulator.Outputs[0].Kind);
        }

        [Fact]
        public void TrackingModulator_IsResolvableAsIModulator_AndIsSetKind()
        {
            var provider = TestServiceProviderFactory.Create();
            var trackingModulator = provider.GetRequiredService<TrackingModulator>();

            Assert.IsAssignableFrom<IModulator>(trackingModulator);
            Assert.All(trackingModulator.Outputs, o => Assert.Equal(ModulatorKind.Set, o.Kind));
            Assert.Equal(new[] { "Count", "X" }, trackingModulator.Outputs.Select(o => o.Name));
        }

        [Fact]
        public void TrackingModulator_Outputs_FilterByValueType_ExcludesMismatchedOutputs()
        {
            var provider = TestServiceProviderFactory.Create();
            var trackingModulator = provider.GetRequiredService<TrackingModulator>();

            var integerOutputs = trackingModulator.Outputs.Where(o => o.ValueType == ModulatorValueType.Integer).ToList();
            var floatOutputs = trackingModulator.Outputs.Where(o => o.ValueType == ModulatorValueType.Float).ToList();

            Assert.Equal(new[] { "Count" }, integerOutputs.Select(o => o.Name));
            Assert.Equal(new[] { "X" }, floatOutputs.Select(o => o.Name));
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
