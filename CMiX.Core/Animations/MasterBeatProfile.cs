// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;

namespace CMiX.Core.Animations
{
    public class MasterBeatProfile : Profile
    {
        public MasterBeatProfile()
        {
            CreateMap<MasterBeat, MasterBeatModel>().ReverseMap();

            CreateMap<IControl, IControlModel>()
                .Include(typeof(MasterBeat), typeof(MasterBeatModel))
                .ReverseMap();
        }
    }
}
