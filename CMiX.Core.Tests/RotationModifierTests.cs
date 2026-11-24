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
        public void ModulatorManager_CanAddRandomModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var rotation = provider.GetRequiredService<RotationModifier>();

            rotation.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Single(rotation.ModulatorManager.ManagerData.Items);
            Assert.IsType<RandomModulator>(rotation.ModulatorManager.ManagerData.Items[0]);
        }
    }
}
