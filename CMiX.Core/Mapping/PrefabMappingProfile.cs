// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering.Cameras;

namespace CMiX.Core.Mapping
{
    public class PrefabMappingProfile : Profile
    {
        public PrefabMappingProfile()
        {
            CreateMap(typeof(PrefabService), typeof(PrefabServiceModel)).ReverseMap().ConstructUsingServiceLocator();
            CreateMap(typeof(EmptyPrefab), typeof(EmptyPrefabModel)).ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Composition, CompositionModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Layer, LayerModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<LayerSettings, LayerSettingsModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap(typeof(Entity), typeof(EntityModel)).ReverseMap().ConstructUsingServiceLocator();
            CreateMap(typeof(Camera), typeof(CameraModel)).ReverseMap().ConstructUsingServiceLocator();
            CreateMap(typeof(TextEntity), typeof(TextEntityModel)).ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Color, ColorModel>().ReverseMap().ConstructUsingServiceLocator();

            CreateMap<IControl, IControlModel>()
                .Include(typeof(PrefabService), typeof(PrefabServiceModel))
                .ReverseMap().ConstructUsingServiceLocator(); ;

            CreateMap<IControl, IControlModel>()
                .Include(typeof(EmptyPrefab), typeof(EmptyPrefabModel))
                .Include<Composition, CompositionModel>()
                .Include(typeof(Layer), typeof(LayerModel))
                .Include(typeof(LayerSettings), typeof(LayerSettingsModel))
                .Include(typeof(Entity), typeof(EntityModel))
                .Include(typeof(Camera), typeof(CameraModel))
                .Include(typeof(TextEntity), typeof(TextEntityModel))
                .Include<Color, ColorModel>()
                .ReverseMap().ConstructUsingServiceLocator();
        }
    }
}
