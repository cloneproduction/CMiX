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
    }
}
