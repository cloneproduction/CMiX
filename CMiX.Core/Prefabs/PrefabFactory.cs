// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Services;

namespace CMiX.Core.Prefabs
{
    public class PrefabFactory
    {
        public PrefabFactory(Dictionary<Type, Func<IPrefab>> factories, PrefabRepositories prefabRepositories)
        {
            Factories = factories;
        }

        Dictionary<Type, Func<IPrefab>> Factories { get; set; }

        private int nameCount = 0;

        public IPrefab CreatePrefab(Type type) 
        {
            if (!Factories.TryGetValue(type, out var factory) || factory is null)
                throw new ArgumentOutOfRangeException(nameof(type), $"type '{type}' is not registered");

            return factory();
        }

        public IPrefab CreatePrefab(IPrefabModel prefabModel)
        {
            var type = prefabModel.GetType();

            if (!Factories.TryGetValue(type, out var factory) || factory is null)
                throw new ArgumentOutOfRangeException(nameof(type), $"type '{type}' is not registered");
            var prefab = factory();

            return prefab;
        }

        public IPrefab GetPrefab(Guid id)
        {
            //if (Factories == null)
            //    return null;

            //var factory = Factories.FirstOrDefault(x => x.GetPrefab(id) != null);

            //if (factory == null)
            //    return null;

            //var prefab = factory.GetPrefab(id);
            //return prefab;
            return null;
        }
    }
}
