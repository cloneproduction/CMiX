// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Texturing;

namespace CMiX.Core.Mapping
{
    public class PrefabMappingProfile : Profile
    {
        public PrefabMappingProfile()
        {
            CreateMap(typeof(PrefabService), typeof(PrefabServiceModel)).ReverseMap().ConstructUsingServiceLocator();
            CreateMap(typeof(EmptyPrefab), typeof(EmptyPrefabModel)).ReverseMap().ConstructUsingServiceLocator();


            CreateMap<Composition, CompositionModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Composition, CompositionModel>().ReverseMap().ConstructUsingServiceLocator();

            CreateMap<Layer, LayerModel>().ReverseMap();

            CreateMap(typeof(Entity), typeof(EntityModel)).ReverseMap().ConstructUsingServiceLocator(); ;
            CreateMap(typeof(Texture), typeof(TextureModel)).ReverseMap().ConstructUsingServiceLocator(); ;
            CreateMap(typeof(Camera), typeof(CameraModel)).ReverseMap().ConstructUsingServiceLocator(); ;

            //CreateMap<IPrefab, IPrefabModel>().ConvertUsing<CustomConverter>();
            CreateMap<IControl, IControlModel>()
                .Include(typeof(PrefabService), typeof(PrefabServiceModel))
                .ReverseMap().ConstructUsingServiceLocator(); ;

            CreateMap<IControl, IControlModel>()
                .Include(typeof(EmptyPrefab), typeof(EmptyPrefabModel))
                .Include<Composition, CompositionModel>()
                .Include(typeof(Layer), typeof(LayerModel))
                .Include(typeof(Entity), typeof(EntityModel))
                .Include(typeof(Texture), typeof(TextureModel))
                .Include(typeof(Camera), typeof(CameraModel))
                .ReverseMap().ConstructUsingServiceLocator();
            ; ;
        }
    }
}
