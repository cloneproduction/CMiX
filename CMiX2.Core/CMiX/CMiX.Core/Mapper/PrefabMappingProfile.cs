// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Beat;
using CMiX.Core.Presentations.Beat;
using CMiX.Core.Presentations.Components;
using CMiX.Core.Presentations.Entities.Lights;
using CMiX.Core.Presentations.Materials;
using CMiX.Core.Presentations.Prefabs;
using CMiX.Core.Presentations.ViewModels;

namespace CMiX.Core.Mapper
{
    public class PrefabMappingProfile : Profile
    {
        public PrefabMappingProfile()
        {
            CreateMap<PrefabManager<Layer>, PrefabManagerModel>().ReverseMap();
            CreateMap<PrefabManager<Entity>, PrefabManagerModel>().ReverseMap();
            CreateMap<PrefabManager<LightEntity>, PrefabManagerModel>().ReverseMap();

            CreateMap<PrefabContainer, PrefabContainerModel>().ReverseMap();

            CreateMap<IPrefab, IPrefabModel>()
                .Include<Composition, CompositionModel>()
                .Include<Layer, LayerModel>()
                .Include<Entity, EntityModel>()
                .Include<Camera, CameraModel>()
                .Include<LightEntity, LightEntityModel>()
                .Include<Material, MaterialModel>()
                .Include<MasterBeat, MasterBeatModel>()
                .ReverseMap();

            CreateMap<Entity, EntityModel>().ReverseMap();
            CreateMap<Layer, LayerModel>().ReverseMap();
            CreateMap<Composition, CompositionModel>().ReverseMap();
            CreateMap<LightEntity, LightEntityModel>().ReverseMap();
        }
    }
}
