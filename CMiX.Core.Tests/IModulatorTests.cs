using System.Linq;
using CMiX.Core.Modulation.Modulators;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class IModulatorTests
    {
        [Fact]
        public void RandomModulator_IsResolvableAsIModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var randomModulator = provider.GetRequiredService<RandomModulator>();

            Assert.IsAssignableFrom<IModulator>(randomModulator);
        }

        [Fact]
        public void RandomModulator_HasFloatOutput()
        {
            var provider = TestServiceProviderFactory.Create();
            var randomModulator = provider.GetRequiredService<RandomModulator>();

            Assert.IsType<ModulatorOutput<float>>(randomModulator.Outputs[0]);
        }

        [Fact]
        public void TrackingModulator_IsResolvableAsIModulator_AndHasNamedOutputs()
        {
            var provider = TestServiceProviderFactory.Create();
            var trackingModulator = provider.GetRequiredService<TrackingModulator>();

            Assert.IsAssignableFrom<IModulator>(trackingModulator);
            Assert.Equal(new[] { "Count", "X", "Y" }, trackingModulator.Outputs.Select(o => o.Name));
        }

        [Fact]
        public void TrackingModulator_Outputs_FilterByOutputType_ExcludesMismatchedOutputs()
        {
            var provider = TestServiceProviderFactory.Create();
            var trackingModulator = provider.GetRequiredService<TrackingModulator>();

            var integerOutputs = trackingModulator.Outputs.OfType<ModulatorOutput<int>>().ToList();
            var floatOutputs = trackingModulator.Outputs.OfType<ModulatorOutput<float>>().ToList();

            Assert.Equal(new[] { "Count" }, integerOutputs.Select(o => o.Name));
            Assert.Equal(new[] { "X", "Y" }, floatOutputs.Select(o => o.Name));
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
