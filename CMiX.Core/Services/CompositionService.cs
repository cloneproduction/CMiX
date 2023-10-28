// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;

namespace CMiX.Core.Services
{
    public class CompositionService : IService
    {
        public CompositionService()
        {
            ProjectRepository = new PrefabRepository();
            CompositionRepository = new PrefabRepository();
            LayerRepository = new PrefabRepository();
            EntityRepository = new PrefabRepository();
        }

        public PrefabRepository ProjectRepository { get; set; }
        public PrefabRepository CompositionRepository { get; set; }
        public PrefabRepository LayerRepository { get; set; }
        public PrefabRepository EntityRepository { get; set; }
    }
}
