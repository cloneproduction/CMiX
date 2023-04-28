// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Materials;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Mapping
{
    public class MaterialMappingProfile : Profile
    {
        public MaterialMappingProfile()
        {
            CreateMap<PrefabManager<Material>, PrefabManagerModel>().ReverseMap();
            CreateMap<Material, MaterialModel>().ReverseMap();
            CreateMap<GenericValue<PipelineType>, GenericValueModel<PipelineType>>().ReverseMap();
            CreateMap<GenericValue<TransparencyType>, GenericValueModel<TransparencyType>>().ReverseMap();
            CreateMap<GenericValue<CullModeType>, GenericValueModel<CullModeType>>().ReverseMap();
        }
    }
}
