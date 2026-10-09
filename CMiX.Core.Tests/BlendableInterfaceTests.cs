using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Layers, compositions and filters must give their blend through IBlendable, and the interface must return their own values.
    public class BlendableInterfaceTests
    {
        private const float TestOpacity = 0.3f;
        private static readonly BlendMode TestBlendMode = Enum.GetValues<BlendMode>().Last();

        private static ControlFactory CreateFactory() =>
            TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>();

        [Theory]
        [InlineData(typeof(CompositingSettings))]
        [InlineData(typeof(Blend))]
        public void Type_ImplementsIBlendable(Type type)
        {
            Assert.True(typeof(IBlendable).IsAssignableFrom(type),
                $"{type.Name} must implement IBlendable so the VL side can use one blend node.");
        }

        [Fact]
        public void LayerCompositionAndFilter_ShareTheBlendableValues()
        {
            var factory = CreateFactory();
            var layer = (Layer)factory.Create(typeof(Layer));
            var composition = (Composition)factory.Create(typeof(Composition));
            var blur = (Blur)factory.Create(typeof(Blur));

            IBlendable layerBlend = ((IComposable)layer).Compositing;
            IBlendable compositionBlend = ((IComposable)composition).Compositing;
            IBlendable filterBlend = (IBlendable)blur.Blend;

            Assert.Same(layer.Compositing, layerBlend);
            Assert.Same(composition.Compositing, compositionBlend);
            Assert.Same(blur.Blend, filterBlend);

            Assert.Equal((TestBlendMode, TestOpacity), SetAndReadBack(layerBlend));
            Assert.Equal((TestBlendMode, TestOpacity), SetAndReadBack(compositionBlend));
            Assert.Equal((TestBlendMode, TestOpacity), SetAndReadBack(filterBlend));
        }

        // Writes the test values through the interface and returns what the interface reads back.
        private static (BlendMode Mode, float Opacity) SetAndReadBack(IBlendable blendable)
        {
            blendable.Opacity.Value = TestOpacity;
            blendable.BlendMode.Value = TestBlendMode;
            return (blendable.BlendMode.Value, blendable.Opacity.Value);
        }
    }
}
