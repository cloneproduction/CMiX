// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Materials;

namespace CMiX.Core.Mapping
{
    public class MaterialProfile : Profile
    {
        public MaterialProfile()
        {
            CreateMap<Material, MaterialModel>().ReverseMap();
            CreateMap<MaterialSettings, MaterialSettingsModel>().ReverseMap();
        }
    }
}
