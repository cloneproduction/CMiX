// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking;

namespace CMiX.Core.Prefabs
{
    public class EmptyPrefabFactory : IPrefabFactory
    {
        public EmptyPrefabFactory() 
        { 
        
        }

        public bool AppliesTo(Type type)
        {
            return (typeof(EmptyPrefab).Equals(type) || typeof(EmptyPrefabModel).Equals(type));
        }

        public IPrefab CreatePrefab(PrefabService prefabService)
        {
            return new EmptyPrefab(prefabService);
        }

        public IPrefab CreatePrefab(PrefabService prefabService, IPrefabModel prefabModel)
        {
            return ControlMessenger.Mapper.Map(prefabModel, CreatePrefab(prefabService));
        }
    }
}
