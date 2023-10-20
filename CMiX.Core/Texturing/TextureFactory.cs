// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;

namespace CMiX.Core.Texturing
{
    public class TextureFactory : IPrefabFactory
    {
        public TextureFactory() 
        { 
        
        }

        public bool AppliesTo(Type type)
        {
            return (typeof(Texture).Equals(type) || typeof(TextureModel).Equals(type));
        }

        public IPrefab CreatePrefab(PrefabService prefabService)
        {
            var modifierManager = new ModifierManager(new TextureModifierFactory());
            return new Texture(prefabService, modifierManager);
        }

        public IPrefab CreatePrefab(PrefabService prefabService, IPrefabModel prefabModel)
        {
            return ControlMessenger.Mapper.Map(prefabModel, CreatePrefab(prefabService));
        }
    }
}
