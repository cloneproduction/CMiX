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
        public CompositionFactory(MasterBeat masterBeat, PrefabRepositories prefabRepositories, ControlMessenger controlMessenger)
        {
            ProjectRepository = prefabRepositories.ProjectRepository;
            PrefabRepositories = prefabRepositories;
            MasterBeat = masterBeat;
            ControlMessenger = controlMessenger;
        }

        PrefabRepository ProjectRepository { get; set; }
        PrefabRepositories PrefabRepositories { get; set; }
        MasterBeat MasterBeat { get; set; }
        ControlMessenger ControlMessenger { get; set; }

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
            var prefabFactory = new PrefabFactory();
            prefabFactory.RegisterFactory(new LayerFactory(PrefabRepositories, ControlMessenger));

            var prefabManagerDraggable = new DraggablePrefabManager(prefabFactory);
            var modifierManager = new ModifierManager(new TextureModifierFactory());
            var outputSettings = new OutputSettings();
            var composition = new Composition(prefabService, MasterBeat, prefabManagerDraggable, modifierManager, outputSettings);

            ProjectRepository.AddPrefab(composition);

            return composition;
        }

        public IPrefab CreatePrefab(PrefabService prefabService, IPrefabModel prefabModel)
        {
            return ControlMessenger.Mapper.Map(prefabModel, CreatePrefab(prefabService));
        }
    }
}
