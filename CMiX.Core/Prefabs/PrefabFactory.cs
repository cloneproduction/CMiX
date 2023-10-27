// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Prefabs
{
    public class PrefabFactory
    {
        public PrefabFactory(List<IPrefabFactory> prefabFactories, PrefabRepository prefabRepository)
        {
            Factories = prefabFactories;
            PrefabRepository = prefabRepository;
        }

        public PrefabRepository PrefabRepository { get; set; }
        public List<IPrefabFactory> Factories { get; set; }

        private int nameCount = 0;

        PrefabService CreatePrefabService(Type type)
        {
            var name = new StringValue(type.Name + "." + nameCount.ToString("000"));
            var isRenaming = new BooleanValue(false);
            var isSelected = new BooleanValue(true);
            var visibility = new BooleanValue(true);

            nameCount++;

            return new PrefabService(name, isRenaming, isSelected, visibility);
        }

        public IPrefab CreatePrefab(Type type)
        {
            var factory = Factories.FirstOrDefault(factory => factory.AppliesTo(type));

            if (factory == null)
                throw new InvalidOperationException($"{type} not registered");

            var prefabService = CreatePrefabService(type);
            var prefab = factory.CreatePrefab(prefabService);

            PrefabRepository.AddPrefab(prefab);
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

            PrefabRepository.AddPrefab(prefab);
            return prefab;
            //return ControlMessenger.Mapper.Map<IPrefab>(prefabModel);
        }
    }
}
