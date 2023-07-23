// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Compositing;
using CMiX.Core.Prefab;
using CMiX.Core.Entities.Lights;
using CMiX.Core.Materials;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Prefab.Managers;
using CMiX.Core.Texturing;

namespace CMiX.Core.Mapping
{
    public class PrefabMappingProfile : Profile
    {
        public PrefabMappingProfile()
        {
            //CreateMap<PrefabManager, PrefabManagerModel>().ReverseMap();

            CreateMap<PrefabManager<Composition>, PrefabManagerModel>().ReverseMap();
            CreateMap<PrefabManager<Layer>, PrefabManagerModel>().ReverseMap();
            CreateMap<PrefabManager<Entity>, PrefabManagerModel>().ReverseMap();
            CreateMap<PrefabManager<Camera>, PrefabManagerModel>().ReverseMap();
            CreateMap<PrefabManager<LightEntity>, PrefabManagerModel>().ReverseMap();
            CreateMap<PrefabManager<Texture>, PrefabManagerModel>().ReverseMap();

            CreateMap<IPrefab, IPrefabModel>()
                .Include<EmptyPrefab, EmptyPrefabModel>()
                .Include<Composition, CompositionModel>()
                .Include<Layer, LayerModel>()
                .Include<Entity, EntityModel>()
                .Include<Camera, CameraModel>()
                .Include<LightEntity, LightEntityModel>()
                .Include<Texture, TextureModel>()
                .ReverseMap();

            CreateMap<EmptyPrefab, EmptyPrefabModel>().ReverseMap();
            CreateMap<Entity, EntityModel>().ReverseMap();
            CreateMap<Layer, LayerModel>().ReverseMap();
            CreateMap<Composition, CompositionModel>().ReverseMap();
            CreateMap<LightEntity, LightEntityModel>().ReverseMap();
        }
    }
}
