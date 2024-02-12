// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Animations;
using CMiX.Core.BaseControls;

namespace CMiX.Core.Mapping
{
    public class BaseControlMappingProfile : Profile
    {
        public BaseControlMappingProfile()
        {
            CreateMap<Integer2, Integer2Model>().ConstructUsing(src => new Integer2Model()).ReverseMap();
            CreateMap<Vector2, Vector2Model>().ConstructUsing(src => new Vector2Model()).ReverseMap();
            CreateMap<Vector3, Vector3Model>().ConstructUsing(src => new Vector3Model()).ReverseMap();
            CreateMap<DirectionXYZ, DirectionXYZModel>().ReverseMap();
            CreateMap(typeof(GenericValue<>), typeof(GenericValueModel<>)).ReverseMap();

            CreateMap<IControl, IControlModel>()
                .Include<Integer2, Integer2Model>()
                .Include<Vector2, Vector2Model>()
                .Include<Vector3, Vector3Model>()
                .Include<DirectionXYZ, DirectionXYZModel>()
                .Include(typeof(GenericValue<>), typeof(GenericValueModel<>))
                .ReverseMap();

            CreateMap<Button, ButtonModel>().ReverseMap();
        }
    }
}
