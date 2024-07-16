// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Mapping
{
    public class ManagerProfile : Profile
    {
        public ManagerProfile()
        {
            CreateMap<PrefabManager, PrefabManagerModel>()
                .ReverseMap()
                .AfterMap<MappingAction>()
                .ConstructUsingServiceLocator();

            CreateMap<IControl, IControlModel>()
                .Include<ManagerData, ManagerDataModel>()
                .Include<PrefabManager, PrefabManagerModel>()
                .Include<PrefabSelector, PrefabSelectorModel>()
                .ReverseMap()
                .ConstructUsingServiceLocator();

            CreateMap<ManagerData, ManagerDataModel>()
                .ReverseMap()
                .ConstructUsingServiceLocator();

            CreateMap<PrefabSelector, PrefabSelectorModel>()
                .ReverseMap()
                .ConstructUsingServiceLocator();

            CreateMap<IControl, IControlModel>()
                .Include<PrefabSelector, PrefabSelectorModel>()
                .ReverseMap()
                .ConstructUsingServiceLocator();
        }
    }
}
