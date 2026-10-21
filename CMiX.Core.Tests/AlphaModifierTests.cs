using System.Reflection;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class AlphaModifierTests
    {
        private static AlphaModifier Create()
        {
            var provider = TestServiceProviderFactory.Create();
            return (AlphaModifier)provider.GetRequiredService<ControlFactory>().Create(typeof(AlphaModifier));
        }

        [Fact]
        public void Fresh_ValueIsOne()
        {
            Assert.Equal(1.0f, Create().Value.Value);
        }

        [Fact]
        public void RoundTrip_KeepsValue()
        {
            var modifier = Create();
            modifier.Value.Value = 0.25f;

            var copy = Create();
            copy.FromModel(modifier.ToModel());

            Assert.Equal(0.25f, copy.Value.Value);
        }

        [Theory]
        [InlineData(typeof(Layer))]
        [InlineData(typeof(Composition))]
        public void IsOfferedForOwner(System.Type owner)
        {
            Assert.Contains(typeof(AlphaModifier).GetCustomAttributes<ModifierPanelAttribute>(), a => a.PanelOwner == owner);
        }

        [Fact]
        public void IComposable_IsModifiable()
        {
            Assert.True(typeof(IModifiable).IsAssignableFrom(typeof(IComposable)));
        }

        [Fact]
        public void Composition_ModifierManager_AcceptsAlphaModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var composition = provider.GetRequiredService<ControlFactory>().Create(typeof(Composition)) as Composition;
            Assert.NotNull(composition);
            composition.ModifierManager.AddItemCommand.Execute(typeof(AlphaModifier));

            Assert.Single(composition.ModifierManager.ManagerData.Items.OfType<AlphaModifier>());
        }
    }
}
