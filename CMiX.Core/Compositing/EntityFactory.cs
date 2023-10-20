// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Materials;
using CMiX.Core.Modifiers;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing;
using CMiX.Core.Transformation.Modifiers;
using CMiX.Core.ViewModels;

namespace CMiX.Core.Compositing
{
    public class EntityFactory : IPrefabFactory
    {
        public EntityFactory(PrefabRepository prefabRepository)
        {
            PrefabRepository = prefabRepository;
        }

        PrefabRepository PrefabRepository { get; set; }

        public bool AppliesTo(Type type)
        {
            return (typeof(Entity).Equals(type) || typeof(EntityModel).Equals(type));
        }

        public IPrefab CreatePrefab(PrefabService prefabService)
        {
            var factories = new List<IPrefabFactory>();
            var textureFactory = new TextureFactory();
            factories.Add(textureFactory);

            var prefabFactory = new PrefabFactory(factories);

            var maskTexture = new MaskTexture(new PrefabManagerBase(PrefabRepository, prefabFactory));
            var diffuseTexture = new DiffuseTexture(new PrefabManagerBase(PrefabRepository, prefabFactory));
            var material = new Material(diffuseTexture, maskTexture);
            var mesh = new Mesh();
            var modifierManager = new ModifierManager(new EntityModifierFactory());

            return new Entity(prefabService, mesh, material, modifierManager);
        }

        public IPrefab CreatePrefab(PrefabService prefabService, IPrefabModel prefabModel)
        {
            return ControlMessenger.Mapper.Map(prefabModel, CreatePrefab(prefabService));
        }
    }
}
