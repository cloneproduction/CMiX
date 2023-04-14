// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Presentations.PostFX;
using CMiX.Core.Presentations.Rendering;
using CMiX.Core.Presentations.ViewModels;

namespace CMiX.Core.Mapper
{
    public class RenderingMappingProfile : Profile
    {
        public RenderingMappingProfile()
        {
            CreateMap<OutputSettings, OutputSettingsModel>().ReverseMap();
            CreateMap<AmbientOcclusion, AmbientOcclusionModel>().ReverseMap();
        }
    }
}
