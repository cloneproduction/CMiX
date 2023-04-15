// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.BaseControls;
using CMiX.Core.Transformation.Modifiers;
using CMiX.Core.Texturing.Filters;

namespace CMiX.Core.Mapping
{
    public class FiltersMappingProfile : Profile
    {
        public FiltersMappingProfile()
        {
            CreateMap<TriColor, TriColorModel>().ReverseMap();
            CreateMap<Edge, EdgeModel>().ReverseMap();
            CreateMap<RandomUV, RandomUVModel>().ReverseMap();
            CreateMap<HSCB, HSCBModel>().ReverseMap();
            CreateMap<Invert, InvertModel>().ReverseMap();
            CreateMap<GenericValue<InvertChannel>, GenericValueModel<InvertChannel>>().ReverseMap();
            CreateMap<LFOUV, LFOUVModel>().ReverseMap();
            CreateMap<Pixelate, PixelateModel>().ReverseMap();
            CreateMap<Blur, BlurModel>().ReverseMap();
            CreateMap<Echo, EchoModel>().ReverseMap();
            CreateMap<Feedback, FeedbackModel>().ReverseMap();
            CreateMap<Blur, BlurModel>().ReverseMap();
            CreateMap<TransformTexture, TransformTextureModel>().ReverseMap();
        }
    }
}
