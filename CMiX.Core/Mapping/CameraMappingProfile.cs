// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.BaseControls;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Cameras.Modifiers;

namespace CMiX.Core.Mapping
{
    public class CameraMappingProfile : Profile
    {
        public CameraMappingProfile()
        {
            CreateMap<Camera, CameraModel>().ReverseMap();

            CreateMap<GenericValue<CameraAxis>, GenericValueModel<CameraAxis>>().ReverseMap();

            CreateMap<CameraLFO, CameraLFOModel>().ReverseMap();
            CreateMap<CameraRandom, CameraRandomModel>().ReverseMap();
        }
    }
}
