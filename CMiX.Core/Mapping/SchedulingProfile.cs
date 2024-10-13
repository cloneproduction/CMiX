// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Animations;
using CMiX.Core.Scheduling;

namespace CMiX.Core.Mapping
{
    public class SchedulingProfile : Profile
    {
        public SchedulingProfile()
        {
            CreateMap<Job, JobModel>().ReverseMap().ConstructUsingServiceLocator();

            CreateMap<IControl, IControlModel>()
                .Include(typeof(Job), typeof(JobModel))
                .ReverseMap().ConstructUsingServiceLocator();
        }
    }
}
