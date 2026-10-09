// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering;
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
                       MasterBeat masterBeat,
                       OutputMappingManager outputMappingManager,
                       AssetSelector model)
        {
            compositionManager.ManagerData.ID = ManagerIDs.CompositionManager;
            compositionManager.UndoManager = undoManager;  // ← set here
            ID = ManagerIDs.CompositionManager;
            CompositionManager = compositionManager;
            PrefabService = prefabService;
            MasterBeat = masterBeat;
            OutputMappingManager = outputMappingManager;
            Model = model;
            Model.FilePath.ID = ManagerIDs.ProjectModelFilePath;
            PrefabService.ID = this.ID;
            PrefabService.Name.ID = ManagerIDs.ProjectPrefabServiceName;
            PrefabService.IsSelected.ID = ManagerIDs.ProjectPrefabServiceIsSelected;
            PrefabService.Visibility.ID = ManagerIDs.ProjectPrefabServiceVisibility;
            compositionManager.Activate();
        }

        public Guid ID { get; set; }
        public PrefabManager CompositionManager { get; set; }
        public PrefabService PrefabService { get; set; }
        public MasterBeat MasterBeat { get; set; }
        public OutputMappingManager OutputMappingManager { get; set; }

        // The 3D model the project's output mappings render onto - one per project, not one
        // per composition or per output mapping.
        public AssetSelector Model { get; set; }

        public IControlModel ToModel() => new ProjectModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            CompositionManager = (PrefabManagerModel)CompositionManager.ToModel(),
            MasterBeat = (MasterBeatModel)MasterBeat.ToModel(),
            OutputMappingManager = (OutputMappingManagerModel)OutputMappingManager.ToModel(),
            Model = (AssetSelectorModel)Model.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (ProjectModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            PrefabService.Name.ID = ManagerIDs.ProjectPrefabServiceName;
            PrefabService.IsSelected.ID = ManagerIDs.ProjectPrefabServiceIsSelected;
            PrefabService.Visibility.ID = ManagerIDs.ProjectPrefabServiceVisibility;
            MasterBeat.FromModel(m.MasterBeat);
            MasterBeat.ID = ManagerIDs.MasterBeat;
            OutputMappingManager.FromModel(m.OutputMappingManager);
            Model.FromModel(m.Model);

            LoadManager(CompositionManager, m.CompositionManager);
        }

        // FromModel only adds items, so clear first to make this a real replacement.
        public void ApplySnapshot(ProjectModel model)
        {
            CompositionManager.ClearAll();
            FromModel(model);
        }
    }
}
