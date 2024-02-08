// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Mapping;

namespace CMiX.Core.Texturing.Filters
{
    public  class TextureModifierPairProfile : ControlPairProfile
    {
        public TextureModifierPairProfile()
        {
            CreatePair<HSCB, HSCBModel>();
            CreatePair<Edge, EdgeModel>();
            CreatePair<RandomUV, RandomUVModel>();
            CreatePair<Invert, InvertModel>();
            CreatePair<LFOUV, LFOUVModel>();
            CreatePair<Pixelate, PixelateModel>();
            CreatePair<Blur, BlurModel>();
            CreatePair<Echo, EchoModel>();
            CreatePair<Feedback, FeedbackModel>();
            CreatePair<TransformTexture, TransformTextureModel>();
        }
    }
}
