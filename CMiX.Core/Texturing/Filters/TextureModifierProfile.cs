// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Texturing.Filters;

namespace CMiX.Core.Mapping
{
    public  class TextureModifierProfile : Profile
    {
        public TextureModifierProfile()
        {
            CreateMap<HSCB, HSCBModel>().ReverseMap();
            CreateMap<Edge, EdgeModel>().ReverseMap();
            CreateMap<RandomUV, RandomUVModel>().ReverseMap();
            CreateMap<Invert, InvertModel>().ReverseMap();
            CreateMap<LFOUV, LFOUVModel>().ReverseMap();
            CreateMap<Pixelate, PixelateModel>().ReverseMap();
            CreateMap<Blur, BlurModel>().ReverseMap();
            CreateMap<Echo, EchoModel>().ReverseMap();
            CreateMap<Feedback, FeedbackModel>().ReverseMap();
            CreateMap<TransformTexture, TransformTextureModel>().ReverseMap();

            CreateMap<IControl, IControlModel>()
                .Include<HSCB, HSCBModel>()
                .Include<Edge, EdgeModel>()
                .Include<RandomUV, RandomUVModel>()
                .Include<Invert, InvertModel>()
                .Include<LFOUV, LFOUVModel>()
                .Include<Pixelate, PixelateModel>()
                .Include<Blur, BlurModel>()
                .Include<Echo, EchoModel>()
                .Include<Feedback, FeedbackModel>()
                .Include<TransformTexture, TransformTextureModel>()
                .ReverseMap();
        }
    }
}
