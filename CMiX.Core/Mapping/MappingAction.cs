// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Mapping
{
    public class MappingAction : IMappingAction<PrefabManagerModel, PrefabManager>
    {
        private readonly ControlRepository ControlRepository;

        public MappingAction(ControlRepository controlRepository)
        {
            ControlRepository = controlRepository;
        }

        public void Process(PrefabManagerModel source, PrefabManager destination, ResolutionContext context)
        {
            foreach (var item in destination.ManagerData.Items)
            {
                ControlRepository.AddControl(item);
            }
        }
    }
}
