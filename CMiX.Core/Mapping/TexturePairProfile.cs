// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Texturing.Sources;

namespace CMiX.Core.Mapping
{
    public class TexturePairProfile : ControlPairProfile
    {
        public TexturePairProfile()
        {
            CreatePair<Image, ImageModel>();
            CreatePair<Gradient, GradientModel>();
            CreatePair<BubbleNoise, BubbleNoiseModel>();
            CreatePair<TypeWriter, TypeWriterModel>();
            CreatePair<VideoIn, VideoInModel>();
            CreatePair<VideoPlayer, VideoPlayerModel>();
        }
    }
}
