// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs;
using CMiX.Core.Services;
using CMiX.Core.Texturing.Filters;

namespace CMiX.Core.Texturing
{
    public class TextureFactory : IPrefabFactory
    {
        public TextureFactory(CompositionService compositionService) 
        {
            PrefabRepository = compositionService.EntityRepository;
        }

        PrefabRepository PrefabRepository { get; }
        public bool AppliesTo(Type type)
        {
            return (typeof(Texture).Equals(type) || typeof(TextureModel).Equals(type));
        }

        public IPrefab CreatePrefab(PrefabService prefabService)
        {
            var modifierManager = new ModifierManager(new TextureModifierFactory());
            var texture = new Texture(prefabService, modifierManager);
            PrefabRepository.AddPrefab(texture);
            return texture;
        }

        public IPrefab CreatePrefab(PrefabService prefabService, IPrefabModel prefabModel)
        {
            var texture = ControlMessenger.Mapper.Map(prefabModel, CreatePrefab(prefabService));
            PrefabRepository.AddPrefab(texture);
            return texture;
        }
    }
}
