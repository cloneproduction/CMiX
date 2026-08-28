using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering.Lights;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class PositionModifierTests
    {
        [Fact]
        public void PositionModifier_HasThreeChannelsLabeledXYZ()
        {
            var provider = TestServiceProviderFactory.Create();
            var position = provider.GetRequiredService<PositionModifier>();

            Assert.Equal(3, position.Channels.Count);
            Assert.Equal("X", position.Channels[0].Label);
            Assert.Equal("Y", position.Channels[1].Label);
            Assert.Equal("Z", position.Channels[2].Label);
        }

        [Fact]
        public void PositionModifier_IsDiscoverableOnBothEntityAndLightEntity()
        {
            var attributes = typeof(PositionModifier).GetCustomAttributes(typeof(ModifierPanelAttribute), false);
            var owners = System.Array.ConvertAll(attributes, a => ((ModifierPanelAttribute)a).PanelOwner);

            Assert.Contains(typeof(Entity), owners);
            Assert.Contains(typeof(LightEntity), owners);
        }

        [Fact]
        public void PositionModifier_IsAlsoAnIModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var position = provider.GetRequiredService<PositionModifier>();

            Assert.IsAssignableFrom<IModifier>(position);
        }

        [Fact]
        public void ModulatorManager_CanAddBeatModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var position = provider.GetRequiredService<PositionModifier>();

            position.ModulatorManager.AddItem(typeof(BeatModifier));

            Assert.Single(position.ModulatorManager.ManagerData.Items);
            Assert.IsType<BeatModifier>(position.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void PositionModifier_ToModel_FromModel_RoundTripsChannelValuesAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var position = provider.GetRequiredService<PositionModifier>();

            position.Channels[0].Value.Value = 4f;
            var modulatorId = Guid.NewGuid();
            position.Channels[0].Binding.ModulatorID = modulatorId;

            var model = position.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<PositionModifier>();
            reloaded.FromModel(model);

            Assert.Equal(4f, reloaded.Channels[0].Value.Value);
            Assert.Equal(modulatorId, reloaded.Channels[0].Binding.ModulatorID);
        }
    }
}
