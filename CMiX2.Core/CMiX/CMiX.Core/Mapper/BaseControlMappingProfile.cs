// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CMiX.Core.Presentations.ViewModels;
using System.Windows.Media;
using CMiX.Core.BaseControl;

namespace CMiX.Core.Mapper
{
    public class BaseControlMappingProfile : Profile
    {
        public BaseControlMappingProfile()
        {
            CreateMap<Vector3, Vector3Model>().ConstructUsing(src => new Vector3Model()).ReverseMap();
            CreateMap<Vector2, Vector2Model>().ConstructUsing(src => new Vector2Model()).ReverseMap();
            CreateMap<Integer2, Integer2Model>().ConstructUsing(src => new Integer2Model()).ReverseMap();
            CreateMap<IntegerValue, IntegerValueModel>().ReverseMap();
            CreateMap<FloatValue, FloatValueModel>().ReverseMap();
            CreateMap<BooleanValue, BooleanValueModel>().ReverseMap();
            CreateMap<StringValue, StringValueModel>().ReverseMap();
            CreateMap<ColorSelector, ColorSelectorModel>()
                .ForMember(dest => dest.SelectedColor, opt => opt.MapFrom(src => src.SelectedColor.ToString()))
                .ReverseMap().ForMember(dest => dest.SelectedColor, opt => opt.MapFrom(src => (Color)ColorConverter.ConvertFromString(src.SelectedColor)));
        }
    }
}
