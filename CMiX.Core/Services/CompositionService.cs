// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.ComponentModel;
using CMiX.Core.Animations;
using CMiX.Core.Materials;
using CMiX.Core.Networking;
using CMiX.Core.Prefab;

namespace CMiX.Core.Services
{
    public class CompositionService : IService
    {
        public CompositionService(IPrefabDataBase prefabDataBase)
        {
            PrefabRepository = new PrefabRepository(prefabDataBase);
            MasterBeat = new MasterBeat();
        }

        public PrefabRepository PrefabRepository { get; set; }
        public MasterBeat MasterBeat { get; set; }
    }
}
