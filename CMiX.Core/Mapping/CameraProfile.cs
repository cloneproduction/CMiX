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
            CreateMap<Camera, CameraModel>().ReverseMap();
            CreateMap<CameraSettings, CameraSettingsModel>().ReverseMap();


            CreateMap<CameraLFO, CameraLFOModel>().ReverseMap();
            CreateMap<CameraRandom, CameraRandomModel>().ReverseMap();

            CreateMap<IControl, IControlModel>()
                .Include<CameraLFO, CameraLFOModel>()
                .Include<CameraRandom, CameraRandomModel>()
                .ReverseMap();
        }
    }
}
