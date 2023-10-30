// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Prefabs
{
    public class PrefabFactory
    {
        public PrefabFactory()
        {

        }


        public PrefabFactory(List<IPrefabFactory> factories)
        {
            Factories = factories;
        }


        List<IPrefabFactory> Factories { get; set; }

        private int nameCount = 0;

        PrefabService CreatePrefabService(Type type)
        {
            var id = Guid.NewGuid();
            var name = new StringValue(type.Name + "." + nameCount.ToString("000"));
            var isRenaming = new BooleanValue(false);
            var isSelected = new BooleanValue(true);
            var visibility = new BooleanValue(false);

            nameCount++;

            return new PrefabService(id, name, isRenaming, isSelected, visibility);
        }

        public void RegisterFactory(IPrefabFactory prefabFactory)
        {
            if(Factories == null)
                Factories = new List<IPrefabFactory>();

            Factories.Add(prefabFactory);
        }

        public IPrefab CreatePrefab(Type type)
        {
            var factory = Factories.FirstOrDefault(factory => factory.AppliesTo(type));

            if (factory == null)
                throw new InvalidOperationException($"{type} not registered");

            var prefabService = CreatePrefabService(type);
            var prefab = factory.CreatePrefab(prefabService);

            return prefab;
        }

        public IPrefab CreatePrefab(IPrefabModel prefabModel)
        {
            var factory = Factories.FirstOrDefault(factory => factory.AppliesTo(prefabModel.GetType()));

            if (factory == null)
                throw new InvalidOperationException($"{prefabModel.GetType()} not registered");

            var prefabService = CreatePrefabService(prefabModel.GetType());
            var prefab = factory.CreatePrefab(prefabService, prefabModel);
            prefab.Name.Value = prefabModel.Name.Value;

            return prefab;
        }

        public IPrefab GetPrefab(Guid id)
        {
            if (Factories == null)
                return null;

            var factory = Factories.FirstOrDefault(x => x.GetPrefab(id) != null);

            if (factory == null)
                return null;

            var prefab = factory.GetPrefab(id);
            return prefab;
        }
    }
}
