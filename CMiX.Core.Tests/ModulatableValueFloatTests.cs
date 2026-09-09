using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ModulatableValueFloatTests
    {
        [Fact]
        public void Modulatable_DefaultsToUnbound()
        {
            var provider = TestServiceProviderFactory.Create();
            var modulatable = provider.GetRequiredService<ModulatableValue<float>>();

            Assert.Null(modulatable.ModulatorID.Value);
            Assert.Null(modulatable.BoundModulator);
        }

        [Fact]
        public void Modulatable_ToModel_FromModel_RoundTripsValueAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var modulatable = provider.GetRequiredService<ModulatableValue<float>>();

            modulatable.Label = "X";
            modulatable.Value.Value = 2.5f;
            modulatable.ModulatorID.Value = Guid.NewGuid();

            var model = modulatable.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<ModulatableValue<float>>();
            reloaded.FromModel(model);

            Assert.Equal("X", reloaded.Label);
            Assert.Equal(2.5f, reloaded.Value.Value);
            Assert.Equal(modulatable.ModulatorID.Value, reloaded.ModulatorID.Value);
        }

        [Fact]
        public void SetModulatorCommand_SetsIdAndLiveReference_ThenNullClearsBoth()
        {
            var provider = TestServiceProviderFactory.Create();
            var modulatable = provider.GetRequiredService<ModulatableValue<float>>();
            var randomModulator = provider.GetRequiredService<RandomModulator>();

            modulatable.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            Assert.Equal(randomModulator.ID, modulatable.ModulatorID.Value);
            Assert.Same(randomModulator, modulatable.BoundModulator);

            modulatable.SetModulatorCommand.Execute(null);

            Assert.Null(modulatable.ModulatorID.Value);
            Assert.Null(modulatable.BoundModulator);
        }
    }
}
