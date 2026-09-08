using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class LinearModifierTests
    {
        [Fact]
        public void LinearXYZ_HasTwoBindablesLabeledWidthAndPhase()
        {
            var provider = TestServiceProviderFactory.Create();
            var linearXYZ = provider.GetRequiredService<LinearModifier>();

            Assert.Equal(2, linearXYZ.Bindables.Count);
            Assert.Equal("Width", linearXYZ.Bindables[0].Label);
            Assert.Equal("Phase", linearXYZ.Bindables[1].Label);
            Assert.Same(linearXYZ.Bindables[0], linearXYZ.Width);
            Assert.Same(linearXYZ.Bindables[1], linearXYZ.Phase);
        }

        [Fact]
        public void LinearXYZ_IsDiscoverableOnEntity()
        {
            var attributes = typeof(LinearModifier).GetCustomAttributes(typeof(ModifierPanelAttribute), false);
            var owners = System.Array.ConvertAll(attributes, a => ((ModifierPanelAttribute)a).PanelOwner);

            Assert.Contains(typeof(Entity), owners);
        }

        [Fact]
        public void LinearXYZ_IsAlsoAnIModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var linearXYZ = provider.GetRequiredService<LinearModifier>();

            Assert.IsAssignableFrom<IModifier>(linearXYZ);
        }

        [Fact]
        public void WidthAndPhase_CanIndependentlyShareOneModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var linearXYZ = provider.GetRequiredService<LinearModifier>();
            linearXYZ.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)linearXYZ.ModulatorManager.ManagerData.Items[0];

            linearXYZ.Width.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));
            linearXYZ.Phase.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            Assert.Equal(randomModulator.ID, linearXYZ.Width.ModulatorID.Value);
            Assert.Equal(randomModulator.ID, linearXYZ.Phase.ModulatorID.Value);
        }

        [Fact]
        public void LinearXYZ_ToModel_FromModel_RoundTripsBindableValuesAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var linearXYZ = provider.GetRequiredService<LinearModifier>();

            linearXYZ.Width.Value.Value = 3f;
            var modulatorId = Guid.NewGuid();
            linearXYZ.Phase.ModulatorID.Value = modulatorId;

            var model = linearXYZ.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<LinearModifier>();
            reloaded.FromModel(model);

            Assert.Equal(3f, reloaded.Width.Value.Value);
            Assert.Equal(modulatorId, reloaded.Phase.ModulatorID.Value);
        }

        [Fact]
        public void LinearXYZ_ToModel_FromModel_RoundTripsModifierModeSelector()
        {
            var provider = TestServiceProviderFactory.Create();
            var linearXYZ = provider.GetRequiredService<LinearModifier>();

            linearXYZ.ModifierModeSelector.Mode.Value = ModifierMode.ToSpread;
            linearXYZ.ModifierModeSelector.Count.Value.Value = 5;

            var model = linearXYZ.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<LinearModifier>();
            reloaded.FromModel(model);

            Assert.Equal(ModifierMode.ToSpread, reloaded.ModifierModeSelector.Mode.Value);
            Assert.Equal(5, reloaded.ModifierModeSelector.Count.Value.Value);
        }
    }
}
