// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.BaseControls;
using CMiX.Core.Rendering.Lights;

namespace CMiX.Core.Mapping
{
    public class LightMappingProfile : Profile
    {
        public LightMappingProfile()
        {
            CreateMap<GenericValue<LightType>, GenericValueModel<LightType>>().ReverseMap();
        }
    }
}
