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
            ProjectRepository = compositionService.ProjectRepository;
            CompositionService = compositionService;
            MasterBeat = masterBeat;
        }

        PrefabRepository ProjectRepository { get; set; }
        CompositionService CompositionService { get; set; }
        MasterBeat MasterBeat { get; set; }

        public IPrefab GetPrefab(Guid id)
        {
            return ProjectRepository.GetPrefab(id);
        }

        public bool AppliesTo(Type type)
        {
            return (typeof(Composition).Equals(type) || typeof(CompositionModel).Equals(type));
        }

        public IPrefab CreatePrefab(PrefabService prefabService)
        {
            var factories = new List<IPrefabFactory>();
            factories.Add(new LayerFactory(CompositionService));
            var prefabFactory = new PrefabFactory(factories);

            var prefabManagerDraggable = new DraggablePrefabManager(prefabFactory);
            var modifierManager = new ModifierManager(new TextureModifierFactory());
            var outputSettings = new OutputSettings();
            var composition = new Composition(prefabService, MasterBeat, prefabManagerDraggable, modifierManager, outputSettings);

            ProjectRepository.AddPrefab(composition);

            return composition;
        }

        public IPrefab CreatePrefab(PrefabService prefabService, IPrefabModel prefabModel)
        {
            var composition = ControlMessenger.Mapper.Map(prefabModel, CreatePrefab(prefabService));
            ProjectRepository.AddPrefab(composition);

            return composition;
        }
    }
}
