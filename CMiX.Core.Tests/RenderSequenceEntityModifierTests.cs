using CMiX.Core.Compositing;
using CMiX.Core.Layering.Modifiers;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class RenderSequenceEntityModifierTests
    {
        [Fact]
        public void RenderSequenceEntityModifier_HasOneBindableLabeledControl()
        {
            var provider = TestServiceProviderFactory.Create();
            var render = provider.GetRequiredService<RenderSequenceEntityModifier>();

            Assert.Single(render.Bindables);
            Assert.Equal("Control", render.Bindables[0].Label);
            Assert.Same(render.Bindables[0], render.Control);
        }

        [Fact]
        public void RenderSequenceEntityModifier_IsDiscoverableOnLayer()
        {
            var attributes = typeof(RenderSequenceEntityModifier).GetCustomAttributes(typeof(ModifierPanelAttribute), false);
            var owners = System.Array.ConvertAll(attributes, a => ((ModifierPanelAttribute)a).PanelOwner);

            Assert.Contains(typeof(Layer), owners);
        }

        [Fact]
        public void RenderSequenceEntityModifier_IsAlsoAnIModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var render = provider.GetRequiredService<RenderSequenceEntityModifier>();

            Assert.IsAssignableFrom<IModifier>(render);
        }

        [Fact]
        public void ModulatorManager_CanAddRandomModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var render = provider.GetRequiredService<RenderSequenceEntityModifier>();

            render.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Single(render.ModulatorManager.ManagerData.Items);
            Assert.IsType<RandomModulator>(render.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void RenderSequenceEntityModifier_ToModel_FromModel_RoundTripsBindableAndEntityType()
        {
            var provider = TestServiceProviderFactory.Create();
            var render = provider.GetRequiredService<RenderSequenceEntityModifier>();

            render.Control.Value.Value = 0.75f;
            render.EntityType.Value = EntityType.Camera;

            var model = render.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<RenderSequenceEntityModifier>();
            reloaded.FromModel(model);

            Assert.Equal(0.75f, reloaded.Control.Value.Value);
            Assert.Equal(EntityType.Camera, reloaded.EntityType.Value);
        }
    }
}
