// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;

namespace CMiX.Core.Mapping
{
    public class BaseControlProfile : Profile
    {
        public BaseControlProfile()
        {
            CreateMap<Integer2, Integer2Model>().ConstructUsing(src => new Integer2Model()).ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Vector2, Vector2Model>().ConstructUsing(src => new Vector2Model()).ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Vector3, Vector3Model>().ConstructUsing(src => new Vector3Model()).ReverseMap().ConstructUsingServiceLocator();
            CreateMap<DirectionXYZ, DirectionXYZModel>().ReverseMap().ConstructUsingServiceLocator();
            CreateMap(typeof(GenericValue<>), typeof(GenericValueModel<>)).ReverseMap().ConstructUsingServiceLocator();
            CreateMap<Button, ButtonModel>().ReverseMap().ConstructUsingServiceLocator();
            //CreateMap<GenericValue<float>, GenericValueModel<float>>().ReverseMap().AfterMap<ManagerMapperAction>();


            CreateMap<IControl, IControlModel>()
                .Include<Integer2, Integer2Model>()
                .Include<Vector2, Vector2Model>()
                .Include<Vector3, Vector3Model>()
                .Include<DirectionXYZ, DirectionXYZModel>()
                .Include<Button, ButtonModel>()
                .Include(typeof(GenericValue<>), typeof(GenericValueModel<>))
                .ReverseMap().ConstructUsingServiceLocator();


        }
    }
}
