// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Assets;
using CMiX.Core.BaseControls;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.ViewModels.Assets;

namespace CMiX.Core.Mapping
{
    public class SourceMappingProfile : Profile
    {
        public SourceMappingProfile()
        {
            CreateMap<GenericValue<TextureSourceName>, GenericValueModel<TextureSourceName>>().ReverseMap();
            CreateMap<Gradient, GradientModel>().ReverseMap();
            CreateMap<BubbleNoise, BubbleNoiseModel>().ReverseMap();
            CreateMap<TypeWriter, TypeWriterModel>().ReverseMap();
            CreateMap<VideoIn, VideoInModel>().ReverseMap();
            CreateMap<VideoPlayer, VideoPlayerModel>().ReverseMap();
            CreateMap<Image, ImageModel>().ReverseMap();

            CreateMap<IAsset, IAssetModel>().ReverseMap();

            CreateMap<GenericValue<Asset>, GenericValueModel<Asset>>().ReverseMap();
        }
    }
}
