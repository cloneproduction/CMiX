// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;

namespace CMiX.Core.Prefabs.Managers
{
    public class ManagerProfile : Profile
    {
        public ManagerProfile()
        {
            CreateMap<ReorderablePrefabManager, PrefabManagerModel>().ReverseMap();
            CreateMap<PrefabManager, PrefabManagerModel>().ReverseMap();
            CreateMap<PrefabManagerBase, PrefabManagerModel>().ReverseMap();
            CreateMap<ManagerData, ManagerDataModel>().ReverseMap();

            CreateMap<IControl, IControlModel>()
                .Include<ReorderablePrefabManager, PrefabManagerModel>()
                .Include<PrefabManager, PrefabManagerModel>()
                .Include<PrefabManagerBase, PrefabManagerModel>()
                .Include<ManagerData, ManagerDataModel>()
                .ReverseMap();
        }
    }
}
