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
    public class VisibilityModifierTests
    {
        [Fact]
        public void VisibilityModifier_HasOneChannel()
        {
            var provider = TestServiceProviderFactory.Create();
            var visibility = provider.GetRequiredService<VisibilityModifier>();

            Assert.Single(visibility.Channels);
            Assert.Same(visibility.Channels[0], visibility.Value);
        }

        [Fact]
        public void VisibilityModifier_IsDiscoverableOnBothEntityAndLightEntity()
        {
            var attributes = typeof(VisibilityModifier).GetCustomAttributes(typeof(ModifierPanelAttribute), false);
            var owners = System.Array.ConvertAll(attributes, a => ((ModifierPanelAttribute)a).PanelOwner);

            Assert.Contains(typeof(Entity), owners);
            Assert.Contains(typeof(LightEntity), owners);
        }

        [Fact]
        public void VisibilityModifier_IsAlsoAnIModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var visibility = provider.GetRequiredService<VisibilityModifier>();

            Assert.IsAssignableFrom<IModifier>(visibility);
        }

        [Fact]
        public void ModulatorManager_CanAddBeatModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var visibility = provider.GetRequiredService<VisibilityModifier>();

            visibility.ModulatorManager.AddItem(typeof(BeatModifier));

            Assert.Single(visibility.ModulatorManager.ManagerData.Items);
            Assert.IsType<BeatModifier>(visibility.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void VisibilityModifier_ToModel_FromModel_RoundTripsChannelValuesAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var visibility = provider.GetRequiredService<VisibilityModifier>();

            visibility.Value.Value.Value = 0.5f;
            var modulatorId = Guid.NewGuid();
            visibility.Value.ModulatorID.Value = modulatorId;

            var model = visibility.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<VisibilityModifier>();
            reloaded.FromModel(model);

            Assert.Equal(0.5f, reloaded.Value.Value.Value);
            Assert.Equal(modulatorId, reloaded.Value.ModulatorID.Value);
        }
    }
}
