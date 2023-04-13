// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.Animation;
using CMiX.Core.Presentations.ViewModels;

namespace CMiX.Core.Mapper
{
    public class AnimationMappingProfile : Profile
    {
        public AnimationMappingProfile()
        {
            CreateMap<Easing, EasingModel>().ReverseMap();
            CreateMap<GenericValue<EasingMode>, GenericValueModel<EasingMode>>().ReverseMap();
            CreateMap<GenericValue<EasingFunction>, GenericValueModel<EasingFunction>>().ReverseMap();
        }
    }
}
