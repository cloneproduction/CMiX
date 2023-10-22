// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Entities.Lights;
using CMiX.Core.Modifiers;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering.Lights.Modifiers;
using CMiX.Core.Services;

namespace CMiX.Core.Rendering.Lights
{
    public class LightFactory : IPrefabFactory
    {
        public LightFactory(CompositionService compositionService)
        {
            CompositionService = compositionService;
        }

        CompositionService CompositionService { get; set; }

        public bool AppliesTo(Type type)
        {
            return (typeof(LightEntity).Equals(type) || typeof(LightEntityModel).Equals(type));
        }

        public IPrefab CreatePrefab(PrefabService prefabService)
        {
            var lightSettings = new LightSettings();
            var modifierManager = new ModifierManager(new LightModifierFactory());
            return new LightEntity(prefabService, lightSettings, modifierManager);
        }

        public IPrefab CreatePrefab(PrefabService prefabService, IPrefabModel prefabModel)
        {
            return ControlMessenger.Mapper.Map(prefabModel, CreatePrefab(prefabService));
        }
    }
}
