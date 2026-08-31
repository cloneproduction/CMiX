using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ModulatableTests
    {
        [Fact]
        public void Modulatable_DefaultsToUnbound()
        {
            var provider = TestServiceProviderFactory.Create();
            var modulatable = provider.GetRequiredService<Modulatable>();

            Assert.Null(modulatable.Binding.ModulatorID);
            Assert.Null(modulatable.Binding.BoundModulator);
        }

        [Fact]
        public void Modulatable_ToModel_FromModel_RoundTripsValueAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var modulatable = provider.GetRequiredService<Modulatable>();

            modulatable.Label = "X";
            modulatable.Value.Value = 2.5f;
            modulatable.Binding.ModulatorID = Guid.NewGuid();

            var model = modulatable.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<Modulatable>();
            reloaded.FromModel(model);

            Assert.Equal("X", reloaded.Label);
            Assert.Equal(2.5f, reloaded.Value.Value);
            Assert.Equal(modulatable.Binding.ModulatorID, reloaded.Binding.ModulatorID);
        }

        [Fact]
        public void SetModulatorCommand_SetsIdAndLiveReference_ThenNullClearsBoth()
        {
            var provider = TestServiceProviderFactory.Create();
            var modulatable = provider.GetRequiredService<Modulatable>();
            var beatModifier = provider.GetRequiredService<BeatModifier>();

            modulatable.Binding.SetModulatorCommand.Execute(beatModifier);

            Assert.Equal(beatModifier.ID, modulatable.Binding.ModulatorID);
            Assert.Same(beatModifier, modulatable.Binding.BoundModulator);

            modulatable.Binding.SetModulatorCommand.Execute(null);

            Assert.Null(modulatable.Binding.ModulatorID);
            Assert.Null(modulatable.Binding.BoundModulator);
        }
    }
}
