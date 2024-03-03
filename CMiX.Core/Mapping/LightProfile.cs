// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Colors.Modifiers;
using CMiX.Core.Entities.Lights;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Mapping
{
    public class LightProfile : Profile
    {
        public LightProfile()
        {
            CreateMap<LightEntity, LightEntityModel>().ReverseMap();
            CreateMap<LightSettings, LightSettingsModel>().ReverseMap();
            CreateMap<RandomPosition, RandomPositionModel>().ReverseMap();
            CreateMap<RandomHSV, RandomHSVModel>().ReverseMap();

            CreateMap<IControl, IControlModel>()
                .Include<LightEntity, LightEntityModel>()
                .ReverseMap();
        }
    }
}
