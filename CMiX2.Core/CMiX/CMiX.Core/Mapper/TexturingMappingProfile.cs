// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.Texturing;
using CMiX.Core.Presentations.ViewModels;

namespace CMiX.Core.Mapper
{
    public class TexturingMappingProfile : Profile
    {
        public TexturingMappingProfile()
        {
            CreateMap<Texture, TextureModel>().ReverseMap();
            CreateMap<ProceduralSelector, ProceduralSelectorModel>().ReverseMap();
            CreateMap<GenericValue<BlendModeEnum>, GenericValueModel<BlendModeEnum>>().ReverseMap();
        }
    }
}
