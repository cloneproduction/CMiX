// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Cameras;

namespace CMiX.Core.Mapping
{
    public class CameraMappingProfile : Profile
    {
        public CameraMappingProfile()
        {
            CreateMap<Camera, CameraModel>().ReverseMap();
            CreateMap<PrefabManager<Camera>, PrefabManagerModel>().ReverseMap();
            CreateMap<GenericValue<CameraAxis>, GenericValueModel<CameraAxis>>().ReverseMap();
        }
    }
}
