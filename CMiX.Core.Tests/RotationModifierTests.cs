using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering.Lights;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class RotationModifierTests
    {
        [Fact]
        public void RotationModifier_HasThreeBindablesLabeledXYZ()
        {
            var provider = TestServiceProviderFactory.Create();
            var rotation = (RotationModifier)provider.GetRequiredService<ControlFactory>().Create(typeof(RotationModifier));

            Assert.Equal(3, rotation.Bindables.Count);
            Assert.Equal("X", rotation.Bindables[0].Label);
            Assert.Equal("Y", rotation.Bindables[1].Label);
            Assert.Equal("Z", rotation.Bindables[2].Label);
        }

        [Fact]
        public void RotationModifier_IsDiscoverableOnBothEntityAndLightEntity()
        {
            var attributes = typeof(RotationModifier).GetCustomAttributes(typeof(ModifierPanelAttribute), false);
            var owners = System.Array.ConvertAll(attributes, a => ((ModifierPanelAttribute)a).PanelOwner);

            Assert.Contains(typeof(Entity), owners);
            Assert.Contains(typeof(LightEntity), owners);
        }

        [Fact]
        public void RotationModifier_IsAlsoAnIModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var rotation = provider.GetRequiredService<RotationModifier>();

            Assert.IsAssignableFrom<IModifier>(rotation);
        }

        [Fact]
        public void ModulatorManager_CanAddRandomModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var rotation = provider.GetRequiredService<RotationModifier>();

            rotation.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Single(rotation.ModulatorManager.ManagerData.Items);
            Assert.IsType<RandomModulator>(rotation.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void RotationModifier_ToModel_FromModel_RoundTripsBindableValuesAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var rotation = provider.GetRequiredService<RotationModifier>();

            rotation.Bindables[0].Value = 4f;
            var modulatorId = Guid.NewGuid();
            rotation.Bindables[0].ModulatorID = modulatorId;

            var model = rotation.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<RotationModifier>();
            reloaded.FromModel(model);

            Assert.Equal(4f, reloaded.Bindables[0].Value);
            Assert.Equal(modulatorId, reloaded.Bindables[0].ModulatorID);
        }

        [Fact]
        public void RotationModifier_ToModel_FromModel_RoundTripsModifierModeSelector()
        {
            var provider = TestServiceProviderFactory.Create();
            var rotation = provider.GetRequiredService<RotationModifier>();

            rotation.ModifierModeSelector.Mode.Value = ModifierMode.ToSpread;
            rotation.ModifierModeSelector.Count.Value = 5;

            var model = rotation.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<RotationModifier>();
            reloaded.FromModel(model);

            Assert.Equal(ModifierMode.ToSpread, reloaded.ModifierModeSelector.Mode.Value);
            Assert.Equal(5, reloaded.ModifierModeSelector.Count.Value);
        }
    }
}
