// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.BaseControls;

namespace CMiX.Core.Mapping
{
    public class BaseControlProfile : Profile
    {
        public BaseControlProfile()
        {
            //CreateMap(typeof(GenericValue<>), typeof(GenericValueModel<>));
            //CreateMap<Button, ButtonModel>();
            //CreateMap<Integer2, Integer2Model>();
            //CreateMap<Vector2, Vector2Model>();
            //CreateMap<Vector3, Vector3Model>();
            //CreateMap<DirectionXYZ, DirectionXYZModel>();

            //CreateMap<IControl, IControlModel>()
            //    .Include(typeof(GenericValue<>), typeof(GenericValueModel<>))
            //    .Include<Button, ButtonModel>()
            //    .Include<Integer2, Integer2Model>()
            //    .Include<Vector2, Vector2Model>()
            //    .Include<Vector3, Vector3Model>()
            //    .Include<DirectionXYZ, DirectionXYZModel>()
            //    .ReverseMap();
        }
    }
}
