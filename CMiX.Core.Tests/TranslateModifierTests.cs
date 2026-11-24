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
    public class TranslateModifierTests
    {
        [Fact]
        public void TranslateModifier_HasThreeBindablesLabeledXYZ()
        {
            var provider = TestServiceProviderFactory.Create();
            var position = (TranslateModifier)provider.GetRequiredService<ControlFactory>().Create(typeof(TranslateModifier));

            Assert.Equal(3, position.Bindables.Count);
            Assert.Equal("X", position.Bindables[0].Label);
            Assert.Equal("Y", position.Bindables[1].Label);
            Assert.Equal("Z", position.Bindables[2].Label);
        }

        [Fact]
        public void TranslateModifier_IsDiscoverableOnBothEntityAndLightEntity()
        {
            var attributes = typeof(TranslateModifier).GetCustomAttributes(typeof(ModifierPanelAttribute), false);
            var owners = System.Array.ConvertAll(attributes, a => ((ModifierPanelAttribute)a).PanelOwner);

            Assert.Contains(typeof(Entity), owners);
            Assert.Contains(typeof(LightEntity), owners);
        }

        [Fact]
        public void ModulatorManager_CanAddRandomModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var position = provider.GetRequiredService<TranslateModifier>();

            position.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Single(position.ModulatorManager.ManagerData.Items);
            Assert.IsType<RandomModulator>(position.ModulatorManager.ManagerData.Items[0]);
        }
    }
}
