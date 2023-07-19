// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using System.Windows.Media;
using CMiX.Core.BaseControls;

namespace CMiX.Core.Mapping
{
    public class BaseControlMappingProfile : Profile
    {
        public BaseControlMappingProfile()
        {
            CreateMap<Button, ButtonModel>().ReverseMap();



            CreateMap<IntegerValue, IntegerValueModel>().ReverseMap();
            CreateMap<FloatValue, FloatValueModel>().ReverseMap();
            CreateMap<BooleanValue, BooleanValueModel>().ReverseMap();
            CreateMap<StringValue, StringValueModel>().ReverseMap();

            //this is necessary because they have multiple constructor so they must be specified when mapping
            CreateMap<Vector3, Vector3Model>().ConstructUsing(src => new Vector3Model());
            CreateMap<Vector3Model, Vector3>().ConstructUsing(src => new Vector3());



            CreateMap<Vector2, Vector2Model>().ConstructUsing(src => new Vector2Model()).ReverseMap();
            CreateMap<Integer2, Integer2Model>().ConstructUsing(src => new Integer2Model()).ReverseMap();

            CreateMap<ColorSelector, ColorSelectorModel>()
                .ForMember(dest => dest.SelectedColor, opt => opt.MapFrom(src => src.SelectedColor.ToString()))
                .ReverseMap().ForMember(dest => dest.SelectedColor, opt => opt.MapFrom(src => (Color)ColorConverter.ConvertFromString(src.SelectedColor)));

            CreateMap<DirectionXYZ, DirectionXYZModel>().ReverseMap();
        }
    }
}
