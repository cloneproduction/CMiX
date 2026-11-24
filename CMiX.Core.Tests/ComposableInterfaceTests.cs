// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // The VL pins IComposable, IBlendable and IMaskable. Each interface must return the own values of the control.
    public class ComposableInterfaceTests
    {
        private const float TestOpacity = 0.3f;
        private static readonly BlendMode TestBlendMode = Enum.GetValues<BlendMode>().Last();
        private static readonly MaskChannel TestMaskChannel = Enum.GetValues<MaskChannel>().Last();

        private static ControlFactory CreateFactory() =>
            TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>();

        [Theory]
        [InlineData(typeof(Layer))]
        [InlineData(typeof(Composition))]
        public void IComposable_OfALayerOrAComposition_GivesItsOwnSettings(Type type)
        {
            var control = CreateFactory().Create(type);
            var composable = (IComposable)control;

            Assert.Same(type.GetProperty("Compositing")!.GetValue(control), composable.Compositing);
            Assert.Same(type.GetProperty("Mask")!.GetValue(control), composable.Mask);
        }

        [Fact]
        public void IBlendable_OfLayerCompositionAndFilter_ReadsBackTheirOwnValues()
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

            Assert.Equal((TestBlendMode, TestOpacity), SetBlendAndReadBack(layerBlend));
            Assert.Equal((TestBlendMode, TestOpacity), SetBlendAndReadBack(compositionBlend));
            Assert.Equal((TestBlendMode, TestOpacity), SetBlendAndReadBack(filterBlend));
        }

        // Writes the test values through the interface and returns what the interface reads back.
        private static (BlendMode Mode, float Opacity) SetBlendAndReadBack(IBlendable blendable)
        {
            blendable.Opacity.Value = TestOpacity;
            blendable.BlendMode.Value = TestBlendMode;
            return (blendable.BlendMode.Value, blendable.Opacity.Value);
        }

        [Fact]
        public void IMaskable_OfLayerCompositionAndMaskTexture_ReadsBackTheirOwnValues()
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

            Assert.Equal((TestMaskChannel, layerInvert), SetMaskAndReadBack(layerMask));
            Assert.Equal((TestMaskChannel, compositionInvert), SetMaskAndReadBack(compositionMask));
            Assert.Equal((TestMaskChannel, textureInvert), SetMaskAndReadBack(textureMask));
        }

        // Writes the test values through the interface and returns what the interface reads back.
        private static (MaskChannel Channel, bool Invert) SetMaskAndReadBack(IMaskable maskable)
        {
            maskable.MaskChannel.Value = TestMaskChannel;
            maskable.Invert.Value = !maskable.Invert.Value;
            return (maskable.MaskChannel.Value, maskable.Invert.Value);
        }
    }
}
