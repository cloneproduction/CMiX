// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Cameras.Modifiers;

namespace CMiX.Core.Mapping
{
    public class CameraProfile : Profile
    {
        public CameraProfile()
        {
            CreateMap<CameraSettings, CameraSettingsModel>().ReverseMap().ConstructUsingServiceLocator(); ;


            CreateMap<CameraLFO, CameraLFOModel>().ReverseMap().ConstructUsingServiceLocator(); ;
            CreateMap<CameraRandom, CameraRandomModel>().ReverseMap().ConstructUsingServiceLocator(); ;

            CreateMap<IControl, IControlModel>()
                .Include<CameraLFO, CameraLFOModel>()
                .Include<CameraRandom, CameraRandomModel>()
                .ReverseMap().ConstructUsingServiceLocator(); ;
        }
    }
}
