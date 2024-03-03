// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Texturing;
using CMiX.Core.Texturing.Sources;

namespace CMiX.Core.Mapping
{
    public class TexturingProfile : Profile
    {
        public TexturingProfile()
        {
            CreateMap<DiffuseTexture, DiffuseTextureModel>().ReverseMap();
            CreateMap<SamplerState, SamplerStateModel>().ReverseMap();
            CreateMap<TextureSourceSelector, TextureSourceSelectorModel>().ReverseMap();
            CreateMap<Texture, TextureModel>().ReverseMap();
            CreateMap<DiffuseTexture, DiffuseTextureModel>().ReverseMap();
            CreateMap<MaskTexture, MaskTextureModel>().ReverseMap();
        }
    }
}
