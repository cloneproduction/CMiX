using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class FlipModifierTests
    {
        [Fact]
        public void FlipModifier_HasNoBindables()
        {
            var provider = TestServiceProviderFactory.Create();
            var flip = provider.GetRequiredService<FlipModifier>();

            Assert.Empty(flip.Bindables);
        }

        [Fact]
        public void FlipModifier_IsDiscoverableOnEntity()
        {
            var attributes = typeof(FlipModifier).GetCustomAttributes(typeof(ModifierPanelAttribute), false);
            var owners = System.Array.ConvertAll(attributes, a => ((ModifierPanelAttribute)a).PanelOwner);

            Assert.Contains(typeof(Entity), owners);
        }

        [Fact]
        public void FlipModifier_IsAlsoAnIModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var flip = provider.GetRequiredService<FlipModifier>();

            Assert.IsAssignableFrom<IModifier>(flip);
        }

        [Fact]
        public void FlipModifier_ToModel_FromModel_RoundTripsDirectionXYZ()
        {
            var provider = TestServiceProviderFactory.Create();
            var flip = provider.GetRequiredService<FlipModifier>();

            flip.DirectionXYZ.DirectionX.Value = false;
            flip.DirectionXYZ.DirectionY.Value = true;
            flip.DirectionXYZ.DirectionZ.Value = true;

            var model = flip.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<FlipModifier>();
            reloaded.FromModel(model);

            Assert.False(reloaded.DirectionXYZ.DirectionX.Value);
            Assert.True(reloaded.DirectionXYZ.DirectionY.Value);
            Assert.True(reloaded.DirectionXYZ.DirectionZ.Value);
        }
    }
}
