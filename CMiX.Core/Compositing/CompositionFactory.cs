// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.Modifiers;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering;
using CMiX.Core.Services;
using CMiX.Core.Texturing.Filters;

namespace CMiX.Core.Compositing
{
    public class CompositionFactory : IPrefabFactory
    {
        public CompositionFactory(MasterBeat masterBeat, CompositionService compositionService)
        {
            CompositionService = compositionService;
            MasterBeat = masterBeat;
        }

        CompositionService CompositionService { get; set; }
        MasterBeat MasterBeat { get; set; }

        public bool AppliesTo(Type type)
        {
            return (typeof(Composition).Equals(type) || typeof(CompositionModel).Equals(type));
        }

        public IPrefab CreatePrefab(PrefabService prefabService)
        {
            var factories = new List<IPrefabFactory>();
            factories.Add(new LayerFactory(CompositionService));
            var prefabFactory = new PrefabFactory(factories);

            var prefabManagerDraggable = new DraggablePrefabManager(CompositionService.CompositionRepository, prefabFactory);
            var modifierManager = new ModifierManager(new TextureModifierFactory());
            var outputSettings = new OutputSettings();

            return new Composition(prefabService, MasterBeat, prefabManagerDraggable, modifierManager, outputSettings);
        }

        public IPrefab CreatePrefab(PrefabService prefabService, IPrefabModel prefabModel)
        {
            return ControlMessenger.Mapper.Map(prefabModel, CreatePrefab(prefabService));
        }
    }
}
