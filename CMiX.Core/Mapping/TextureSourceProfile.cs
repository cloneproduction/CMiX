// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Texturing.Sources;

namespace CMiX.Core.Mapping
{
    public class TextureSourceProfile : Profile
    {
        public TextureSourceProfile()
        {
            CreateMap<BubbleNoise, BubbleNoiseModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<VideoPlayer, VideoPlayerModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Image, ImageModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Gradient, GradientModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<VideoIn, VideoInModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<TypeWriter, TypeWriterModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<TouchBlob, TouchBlobModel>().ReverseMap().ConstructUsingServiceLocator();

            CreateMap<IControl, IControlModel>()
                .Include<BubbleNoise, BubbleNoiseModel>()
                .Include<VideoPlayer, VideoPlayerModel>()
                .Include<Image, ImageModel>()
                .Include<Gradient, GradientModel>()
                .Include<VideoIn, VideoInModel>()
                .Include<TypeWriter, TypeWriterModel>()
                .Include<TouchBlob, TouchBlobModel>()
                .ReverseMap().ConstructUsingServiceLocator();
        }
    }
}
