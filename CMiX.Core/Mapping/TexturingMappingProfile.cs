// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.BaseControls;
using CMiX.Core.Texturing;
using CMiX.Core.Texturing.Mixers;
using CMiX.Core.Texturing.Sampling;
using CMiX.Core.Texturing.Sources;

namespace CMiX.Core.Mapping
{
    public class TexturingMappingProfile : Profile
    {
        public TexturingMappingProfile()
        {
            CreateMap<Texture, TextureModel>().ReverseMap();
            CreateMap<TextureSourceSelector, TextureSourceSelectorModel>().ReverseMap();
            CreateMap<GenericValue<BlendModeEnum>, GenericValueModel<BlendModeEnum>>().ReverseMap();

            CreateMap<SamplerState, SamplerStateModel>().ReverseMap();
            CreateMap<GenericValue<TextureAddressMode>, GenericValueModel<TextureAddressMode>>().ReverseMap();

        }
    }
}
