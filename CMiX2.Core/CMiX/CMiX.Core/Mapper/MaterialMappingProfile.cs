// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.Materials;
using CMiX.Core.Presentations.Prefabs;
using CMiX.Core.Presentations.ViewModels;

namespace CMiX.Core.Mapper
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
