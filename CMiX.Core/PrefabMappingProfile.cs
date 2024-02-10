// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing;

namespace CMiX.Core
{
    public class PrefabMappingProfile : Profile
    {
        public PrefabMappingProfile()
        {
            CreateMap(typeof(PrefabService), typeof(PrefabServiceModel)).ReverseMap();
            CreateMap(typeof(EmptyPrefab), typeof(EmptyPrefabModel)).ReverseMap();
            CreateMap(typeof(Composition), typeof(CompositionModel)).ReverseMap();
            CreateMap(typeof(Layer), typeof(LayerModel)).ReverseMap();
            CreateMap(typeof(Entity), typeof(EntityModel)).ReverseMap();
            CreateMap(typeof(Texture), typeof(TextureModel)).ReverseMap();
            CreateMap(typeof(MasterBeat), typeof(MasterBeatModel)).ReverseMap();

            CreateMap<IControl, IControlModel>()
                .Include(typeof(PrefabService), typeof(PrefabServiceModel))
                .Include(typeof(EmptyPrefab), typeof(EmptyPrefabModel))
                .Include(typeof(Composition), typeof(CompositionModel))
                .Include(typeof(Layer), typeof(LayerModel))
                .Include(typeof(Entity), typeof(EntityModel))
                .Include(typeof(Texture), typeof(TextureModel))
                .Include(typeof(MasterBeat), typeof(MasterBeatModel))
                .ReverseMap();
        }
    }
}
