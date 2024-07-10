// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Animations;

namespace CMiX.Core.Mapping
{
    public class MasterBeatProfile : Profile
    {
        public MasterBeatProfile()
        {
            CreateMap<MasterBeat, MasterBeatModel>().ReverseMap().ConstructUsingServiceLocator();

            CreateMap<IControl, IControlModel>()
                .Include(typeof(MasterBeat), typeof(MasterBeatModel))
                .ReverseMap().ConstructUsingServiceLocator();
        }
    }
}
