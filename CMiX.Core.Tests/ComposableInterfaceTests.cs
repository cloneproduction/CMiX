using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Layer and Composition must stay IComposable, and the interface must return their settings.
    public class ComposableInterfaceTests
    {
        private static ControlFactory CreateFactory() =>
            TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>();

        [Theory]
        [InlineData(typeof(Layer))]
        [InlineData(typeof(Composition))]
        public void Type_ImplementsIComposable(Type type)
        {
            Assert.True(typeof(IComposable).IsAssignableFrom(type),
                $"{type.Name} must implement IComposable so the VL side can use one set of nodes.");
        }

        [Fact]
        public void Layer_ExposesItsSettingsThroughIComposable()
        {
            var layer = (Layer)CreateFactory().Create(typeof(Layer));
            var composable = (IComposable)layer;

            Assert.Same(layer.LayerSettings, composable.Compositing);
            Assert.Same(layer.LayerMaskSettings, composable.Mask);
        }

        [Fact]
        public void Composition_ExposesItsSettingsThroughIComposable()
        {
            var composition = (Composition)CreateFactory().Create(typeof(Composition));
            var composable = (IComposable)composition;

            Assert.Same(composition.LayerSettings, composable.Compositing);
            Assert.Same(composition.LayerMaskSettings, composable.Mask);
        }
    }
}
