using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Layers, compositions and mask textures must give their mask through IMaskable, and the interface must return their own values.
    public class MaskableInterfaceTests
    {
        private static readonly MaskChannel TestMaskChannel = Enum.GetValues<MaskChannel>().Last();

        private static ControlFactory CreateFactory() =>
            TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>();

        [Theory]
        [InlineData(typeof(MaskSettings))]
        [InlineData(typeof(MaskTexture))]
        public void Type_ImplementsIMaskable(Type type)
        {
            Assert.True(typeof(IMaskable).IsAssignableFrom(type),
                $"{type.Name} must implement IMaskable so the VL side can use one mask node.");
        }

        [Fact]
        public void LayerCompositionAndMaskTexture_ShareTheMaskableValues()
        {
            var factory = CreateFactory();
            var layer = (Layer)factory.Create(typeof(Layer));
            var composition = (Composition)factory.Create(typeof(Composition));
            var maskTexture = (MaskTexture)factory.Create(typeof(MaskTexture));

            IMaskable layerMask = ((IComposable)layer).Mask;
            IMaskable compositionMask = ((IComposable)composition).Mask;
            IMaskable textureMask = (IMaskable)maskTexture;

            Assert.Same(layer.Mask, layerMask);
            Assert.Same(composition.Mask, compositionMask);
            Assert.Same(maskTexture, textureMask);

            var layerInvert = !layerMask.Invert.Value;
            var compositionInvert = !compositionMask.Invert.Value;
            var textureInvert = !textureMask.Invert.Value;

            Assert.Equal((TestMaskChannel, layerInvert), SetAndReadBack(layerMask));
            Assert.Equal((TestMaskChannel, compositionInvert), SetAndReadBack(compositionMask));
            Assert.Equal((TestMaskChannel, textureInvert), SetAndReadBack(textureMask));
        }

        // Writes the test values through the interface and returns what the interface reads back.
        private static (MaskChannel Channel, bool Invert) SetAndReadBack(IMaskable maskable)
        {
            maskable.MaskChannel.Value = TestMaskChannel;
            maskable.Invert.Value = !maskable.Invert.Value;
            return (maskable.MaskChannel.Value, maskable.Invert.Value);
        }
    }
}
