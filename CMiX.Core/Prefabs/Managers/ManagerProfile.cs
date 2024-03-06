// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Mapping;

namespace CMiX.Core.Prefabs.Managers
{
    public class ManagerProfile : Profile
    {
        public ManagerProfile()
        {
            CreateMap<PrefabManager, PrefabManagerModel>().ReverseMap().ConstructUsingServiceLocator().AfterMap<MappingAction>(); ; ;//.ForMember(d => d.ManagerData, opt => opt.Ignore());//.ForAllMembers(x => x.Ignore());

            CreateMap<IControl, IControlModel>()
                .Include<PrefabManager, PrefabManagerModel>()
                .ReverseMap().ConstructUsingServiceLocator() ;

           
            CreateMap<ManagerData, ManagerDataModel>()
                .ReverseMap().ConstructUsingServiceLocator();

            CreateMap<IControl, IControlModel>()
                .Include<ManagerData, ManagerDataModel>()
                .ReverseMap().ConstructUsingServiceLocator();
        }
    }
}
