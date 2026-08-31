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
        public void RotationModifier_HasThreeChannelsLabeledXYZ()
        {
            var provider = TestServiceProviderFactory.Create();
            var rotation = provider.GetRequiredService<RotationModifier>();

            Assert.Equal(3, rotation.Channels.Count);
            Assert.Equal("X", rotation.Channels[0].Label);
            Assert.Equal("Y", rotation.Channels[1].Label);
            Assert.Equal("Z", rotation.Channels[2].Label);
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
        public void ModulatorManager_CanAddBeatModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var rotation = provider.GetRequiredService<RotationModifier>();

            rotation.ModulatorManager.AddItem(typeof(BeatModifier));

            Assert.Single(rotation.ModulatorManager.ManagerData.Items);
            Assert.IsType<BeatModifier>(rotation.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void RotationModifier_ToModel_FromModel_RoundTripsChannelValuesAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var rotation = provider.GetRequiredService<RotationModifier>();

            rotation.Channels[0].Value.Value = 4f;
            var modulatorId = Guid.NewGuid();
            rotation.Channels[0].Binding.ModulatorID = modulatorId;

            var model = rotation.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<RotationModifier>();
            reloaded.FromModel(model);

            Assert.Equal(4f, reloaded.Channels[0].Value.Value);
            Assert.Equal(modulatorId, reloaded.Channels[0].Binding.ModulatorID);
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
