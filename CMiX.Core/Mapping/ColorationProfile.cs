// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Colors;

namespace CMiX.Core.Mapping
{
    public class ColorationProfile : Profile
    {
        public ColorationProfile()
        {
            CreateMap<Coloration, ColorationModel>().ReverseMap().ConstructUsingServiceLocator();

            CreateMap<IControl, IControlModel>()
                .Include<Coloration, ColorationModel>()
                .ReverseMap().ConstructUsingServiceLocator();
        }
    }
}
