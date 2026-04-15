// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.Collections;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using CMiX.Core.ViewModels.Assets;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Compositing
{
    public class Project : ObservableObject, IPrefab//, IModifiable
    {
        public Project(PrefabManager compositionManager, 
                       PrefabService prefabService, 
                       UndoManager undoManager,
                       MasterBeat masterBeat)
        {
            compositionManager.ManagerData.ID = ManagerIDs.CompositionManager;
            compositionManager.UndoManager = undoManager;  // ← set here
            ID = ManagerIDs.CompositionManager;
            Assets = new SortableObservableCollection<IAsset>();
            CompositionManager = compositionManager;
            PrefabService = prefabService;
            MasterBeat = masterBeat;
            PrefabService.ID = this.ID;
            compositionManager.Activate();
        }

        public Guid ID { get; set; }
        public SortableObservableCollection<IAsset> Assets { get; set; }
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
        }
    }
}
