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

        [Fact]
        public void FFTModulator_IsResolvableAsIModulator_AndHasNamedFloatOutputs()
        {
            var provider = TestServiceProviderFactory.Create();
            var fftModulator = provider.GetRequiredService<FFTModulator>();

            Assert.IsAssignableFrom<IModulator>(fftModulator);
            Assert.Equal(new[] { "FFT", "Bass", "LowerMid", "HigherMid", "High" }, fftModulator.Outputs.Select(o => o.Name));
            Assert.All(fftModulator.Outputs, o => Assert.IsType<ModulatorOutput<float>>(o));
        }

        [Fact]
        public void FFTModulator_ToModel_FromModel_RoundTripsFFTAndBass()
        {
            var provider = TestServiceProviderFactory.Create();
            var fftModulator = provider.GetRequiredService<FFTModulator>();
            fftModulator.FFT.Value = 0.75f;
            fftModulator.Bass.Value = 0.25f;

            var model = fftModulator.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<FFTModulator>();
            reloaded.FromModel(model);

            Assert.Equal(0.75f, reloaded.FFT.Value);
            Assert.Equal(0.25f, reloaded.Bass.Value);
        }
    }
}
