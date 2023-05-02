// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Animations;

namespace CMiX.Core.Mapping
{
    public class BeatMappingProfile : Profile
    {
        public BeatMappingProfile()
        {
            CreateMap<MasterBeat, MasterBeatModel>().ReverseMap();
            CreateMap<BeatModifier, BeatModifierModel>().ReverseMap();
        }
    }
}
