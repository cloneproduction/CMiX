using CMiX.Core.Compositing;
using CMiX.Core.Layering.Modifiers;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class RenderRandomEntityModifierTests
    {
        [Fact]
        public void RenderRandomEntityModifier_HasOneChannelLabeledControl()
        {
            var provider = TestServiceProviderFactory.Create();
            var render = provider.GetRequiredService<RenderRandomEntityModifier>();

            Assert.Single(render.Channels);
            Assert.Equal("Control", render.Channels[0].Label);
            Assert.Same(render.Channels[0], render.Control);
        }

        [Fact]
        public void RenderRandomEntityModifier_IsDiscoverableOnLayer()
        {
            var attributes = typeof(RenderRandomEntityModifier).GetCustomAttributes(typeof(ModifierPanelAttribute), false);
            var owners = System.Array.ConvertAll(attributes, a => ((ModifierPanelAttribute)a).PanelOwner);

            Assert.Contains(typeof(Layer), owners);
        }

        [Fact]
        public void RenderRandomEntityModifier_IsAlsoAnIModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var render = provider.GetRequiredService<RenderRandomEntityModifier>();

            Assert.IsAssignableFrom<IModifier>(render);
        }

        [Fact]
        public void ModulatorManager_CanAddBeatModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var render = provider.GetRequiredService<RenderRandomEntityModifier>();

            render.ModulatorManager.AddItem(typeof(BeatModifier));

            Assert.Single(render.ModulatorManager.ManagerData.Items);
            Assert.IsType<BeatModifier>(render.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void RenderRandomEntityModifier_ToModel_FromModel_RoundTripsChannelAndEntityType()
        {
            var provider = TestServiceProviderFactory.Create();
            var render = provider.GetRequiredService<RenderRandomEntityModifier>();

            render.Control.Value.Value = 0.5f;
            render.EntityType.Value = EntityType.Light;

            var model = render.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<RenderRandomEntityModifier>();
            reloaded.FromModel(model);

            Assert.Equal(0.5f, reloaded.Control.Value.Value);
            Assert.Equal(EntityType.Light, reloaded.EntityType.Value);
        }
    }
}
