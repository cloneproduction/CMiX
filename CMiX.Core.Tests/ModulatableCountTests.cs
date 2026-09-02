using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ModulatableCountTests
    {
        [Fact]
        public void ModulatableCount_DefaultsToUnbound()
        {
            var provider = TestServiceProviderFactory.Create();
            var count = provider.GetRequiredService<ModulatableCount>();

            Assert.Null(count.ModulatorID.Value);
            Assert.Null(count.BoundModulator);
        }

        [Fact]
        public void SetModulatorCommand_SetsIdAndLiveReference_ThenNullClearsBoth()
        {
            var provider = TestServiceProviderFactory.Create();
            var count = provider.GetRequiredService<ModulatableCount>();
            var beatModulator = provider.GetRequiredService<BeatModulator>();

            count.SetModulatorCommand.Execute(new ModulatorOutputSelection(beatModulator, beatModulator.Outputs[0]));

            Assert.Equal(beatModulator.ID, count.ModulatorID.Value);
            Assert.Same(beatModulator, count.BoundModulator);
            Assert.Equal("Value", count.BoundOutputName.Value);

            count.SetModulatorCommand.Execute(null);

            Assert.Null(count.ModulatorID.Value);
            Assert.Null(count.BoundModulator);
            Assert.Null(count.BoundOutputName.Value);
        }

        [Fact]
        public void ModulatableCount_ToModel_FromModel_RoundTripsValueAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var count = provider.GetRequiredService<ModulatableCount>();
            var beatModulator = provider.GetRequiredService<BeatModulator>();

            count.Label = "Count";
            count.Value.Value = 5;
            count.SetModulatorCommand.Execute(new ModulatorOutputSelection(beatModulator, beatModulator.Outputs[0]));

            var model = count.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<ModulatableCount>();
            reloaded.FromModel(model);

            Assert.Equal("Count", reloaded.Label);
            Assert.Equal(5, reloaded.Value.Value);
            Assert.Equal(beatModulator.ID, reloaded.ModulatorID.Value);
            Assert.Equal("Value", reloaded.BoundOutputName.Value);
        }
    }
}
