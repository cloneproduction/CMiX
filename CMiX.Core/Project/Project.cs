// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Compositing
{
    public class Project : ObservableObject, IPrefab
    {
        public Project(PrefabManager compositionManager,
                       PrefabService prefabService,
                       UndoManager undoManager,
                       MasterBeat masterBeat)
        {
            compositionManager.ManagerData.ID = ManagerIDs.CompositionManager;
            compositionManager.UndoManager = undoManager;  // ← set here
            ID = ManagerIDs.CompositionManager;
            CompositionManager = compositionManager;
            PrefabService = prefabService;
            MasterBeat = masterBeat;
            PrefabService.ID = this.ID;
            compositionManager.Activate();
        }

        public Guid ID { get; set; }
        public PrefabManager CompositionManager { get; set; }
        public PrefabService PrefabService { get; set; }
        public MasterBeat MasterBeat { get; set; }

        public IControlModel ToModel() => new ProjectModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            CompositionManager = (PrefabManagerModel)CompositionManager.ToModel(),
            MasterBeat = (MasterBeatModel)MasterBeat.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (ProjectModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            MasterBeat.FromModel(m.MasterBeat);

            LoadManager(CompositionManager, m.CompositionManager);
        }
    }
}
