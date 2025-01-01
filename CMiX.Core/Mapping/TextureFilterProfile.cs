// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Texturing.Filters;

namespace CMiX.Core.Mapping
{
    public class TextureFilterProfile : Profile
    {
        public TextureFilterProfile()
        {
            CreateMap<HSCB, HSCBModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Edge, EdgeModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Invert, InvertModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<ShiftRGB, ShiftRGBModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<LFOUV, LFOUVModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Texturing.Filters.RandomUV, Texturing.Filters.RandomUVModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Pixelate, PixelateModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Blur, BlurModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Echo, EchoModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Feedback, FeedbackModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<TransformTexture, TransformTextureModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<TriColor, TriColorModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<SetAlpha, SetAlphaModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Displace, DisplaceModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Shadow, ShadowModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Ascii, AsciiModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Dither, DitherModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Kuwahara, KuwaharaModel>().ReverseMap().ConstructUsingServiceLocator();

            CreateMap<IControl, IControlModel>()
                .Include<SetAlpha, SetAlphaModel>()
                .Include<HSCB, HSCBModel>()
                .Include<Edge, EdgeModel>()
                .Include<Invert, InvertModel>()
                .Include<ShiftRGB, ShiftRGBModel>()
                .Include<LFOUV, LFOUVModel>()
                .Include<Texturing.Filters.RandomUV, Texturing.Filters.RandomUVModel>()
                .Include<Pixelate, PixelateModel>()
                .Include<Blur, BlurModel>()
                .Include<Echo, EchoModel>()
                .Include<Feedback, FeedbackModel>()
                .Include<TransformTexture, TransformTextureModel>()
                .Include<TriColor, TriColorModel>()
                .Include<Displace, DisplaceModel>()
                .Include<Shadow, ShadowModel>()
                .Include<Ascii, AsciiModel>()
                .Include<Dither, DitherModel>()
                .Include<Kuwahara, KuwaharaModel>()
                .ReverseMap().ConstructUsingServiceLocator();
        }
    }
}
