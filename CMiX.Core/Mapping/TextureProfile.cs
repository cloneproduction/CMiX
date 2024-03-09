// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Texturing.Sources;

namespace CMiX.Core.Mapping
{
    public class TextureProfile : Profile
    {
        public TextureProfile()
        {
            CreateMap<Image, ImageModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Gradient, GradientModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<BubbleNoise, BubbleNoiseModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<TypeWriter, TypeWriterModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<VideoIn, VideoInModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<VideoPlayer, VideoPlayerModel>().ReverseMap().ConstructUsingServiceLocator();
        }
    }
}
