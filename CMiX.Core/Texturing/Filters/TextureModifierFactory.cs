// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Texturing.Filters
{
    public class TextureModifierFactory : ModifierFactory
    {
        public TextureModifierFactory()
        {
            TypePairs.Add(typeof(HSCB), typeof(HSCBModel));
            TypePairs.Add(typeof(Invert), typeof(InvertModel));
            TypePairs.Add(typeof(Blur), typeof(BlurModel));
            TypePairs.Add(typeof(Edge), typeof(EdgeModel));
            TypePairs.Add(typeof(TransformTexture), typeof(TransformTextureModel));
            TypePairs.Add(typeof(Pixelate), typeof(PixelateModel));
            TypePairs.Add(typeof(Echo), typeof(EchoModel));
            TypePairs.Add(typeof(Feedback), typeof(FeedbackModel));
            TypePairs.Add(typeof(TriColor), typeof(TriColorModel));
            TypePairs.Add(typeof(RandomUV), typeof(RandomUVModel));
            TypePairs.Add(typeof(LFOUV), typeof(LFOUVModel));
        }
    }
}
