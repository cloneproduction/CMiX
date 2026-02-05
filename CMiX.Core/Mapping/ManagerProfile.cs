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
            this.MapControlByConvention(typeof(ManagerData));

            CreateMap<PrefabManager, PrefabManagerModel>()
                .ReverseMap()
                .AfterMap<MappingAction>()
                .ConstructUsingServiceLocator();

            CreateMap<IControl, IControlModel>()
                .Include<PrefabManager, PrefabManagerModel>()
                .ReverseMap()
                .ConstructUsingServiceLocator();

        }
    }
}
