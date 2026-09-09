using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Modulation.Modulators;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ScaleModifierTests
    {
        [Fact]
        public void ScaleModifier_HasFourBindablesLabeledXYZUniform()
        {
            var provider = TestServiceProviderFactory.Create();
            var scale = provider.GetRequiredService<ScaleModifier>();

            Assert.Equal(4, scale.Bindables.Count);
            Assert.Equal("X", scale.Bindables[0].Label);
            Assert.Equal("Y", scale.Bindables[1].Label);
            Assert.Equal("Z", scale.Bindables[2].Label);
            Assert.Equal("Uniform", scale.Bindables[3].Label);
        }

        [Fact]
        public void TwoScaleModifiers_HaveDistinctModulatorManagers()
        {
            var provider = TestServiceProviderFactory.Create();
            var scale1 = provider.GetRequiredService<ScaleModifier>();
            var scale2 = provider.GetRequiredService<ScaleModifier>();

            Assert.NotSame(scale1.ModulatorManager, scale2.ModulatorManager);
        }

        [Fact]
        public void ModulatorManager_CanAddRandomModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var scale = provider.GetRequiredService<ScaleModifier>();

            scale.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Single(scale.ModulatorManager.ManagerData.Items);
            Assert.IsType<RandomModulator>(scale.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void ScaleModifier_ToModel_FromModel_RoundTripsBindableValuesAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var scale = provider.GetRequiredService<ScaleModifier>();

            scale.Bindables[0].Value = 4f;
            var modulatorId = Guid.NewGuid();
            scale.Bindables[0].ModulatorID = modulatorId;

            var model = scale.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<ScaleModifier>();
            reloaded.FromModel(model);

            Assert.Equal(4f, reloaded.Bindables[0].Value);
            Assert.Equal(modulatorId, reloaded.Bindables[0].ModulatorID);
        }

        [Fact]
        public void XYZUniform_AreConvenienceAccessorsIntoBindables()
        {
            var provider = TestServiceProviderFactory.Create();
            var scale = provider.GetRequiredService<ScaleModifier>();

            Assert.Same(scale.Bindables[0], scale.X);
            Assert.Same(scale.Bindables[1], scale.Y);
            Assert.Same(scale.Bindables[2], scale.Z);
            Assert.Same(scale.Bindables[3], scale.Uniform);
        }

        [Fact]
        public void ScaleModifier_ToModel_FromModel_RoundTripsModifierModeSelector()
        {
            var provider = TestServiceProviderFactory.Create();
            var scale = provider.GetRequiredService<ScaleModifier>();

            scale.ModifierModeSelector.Mode.Value = ModifierMode.ToSpread;
            scale.ModifierModeSelector.Count.Value = 5;
            scale.Uniform.Value = 2.5f;

            var model = scale.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<ScaleModifier>();
            reloaded.FromModel(model);

            Assert.Equal(ModifierMode.ToSpread, reloaded.ModifierModeSelector.Mode.Value);
            Assert.Equal(5, reloaded.ModifierModeSelector.Count.Value);
            Assert.Equal(2.5f, reloaded.Uniform.Value);
        }

        [Fact]
        public void DeletingAssignedModulator_UnassignsItFromEveryBindableUsingIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var scale = provider.GetRequiredService<ScaleModifier>();
            scale.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)scale.ModulatorManager.ManagerData.Items[0];

            scale.X.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));
            scale.Y.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            scale.ModulatorManager.DeleteItem(randomModulator);

            Assert.Null(scale.X.ModulatorID);
            Assert.Null(scale.X.BoundModulator);
            Assert.Null(scale.Y.ModulatorID);
            Assert.Null(scale.Y.BoundModulator);
            Assert.Null(scale.Z.ModulatorID);
        }

        [Fact]
        public void DeletingAssignedModulator_UnassignsItFromModifierModeSelectorCountToo()
        {
            var provider = TestServiceProviderFactory.Create();
            var scale = provider.GetRequiredService<ScaleModifier>();
            scale.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)scale.ModulatorManager.ManagerData.Items[0];

            scale.ModifierModeSelector.Count.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            scale.ModulatorManager.DeleteItem(randomModulator);

            Assert.Null(scale.ModifierModeSelector.Count.ModulatorID);
            Assert.Null(scale.ModifierModeSelector.Count.BoundModulator);
        }

        [Fact]
        public void ScaleModifier_ToModel_FromModel_ResolvesModifierModeSelectorCountLiveBoundModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var scale = provider.GetRequiredService<ScaleModifier>();
            scale.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)scale.ModulatorManager.ManagerData.Items[0];
            scale.ModifierModeSelector.Count.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            var model = scale.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<ScaleModifier>();
            reloaded.FromModel(model);

            var reloadedRandomModulator = Assert.IsType<RandomModulator>(reloaded.ModulatorManager.ManagerData.Items[0]);
            Assert.Same(reloadedRandomModulator, reloaded.ModifierModeSelector.Count.BoundModulator);
        }
    }
}
