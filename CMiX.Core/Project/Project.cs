// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
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
                       RedisAddressRegistry redisAddressRegistry,
                       AssetSelector model,
                       ControlRepository controlRepository)
        {
            compositionManager.ManagerData.ID = ManagerIDs.CompositionManager;
            compositionManager.UndoManager = undoManager;  // ← set here
            ID = ManagerIDs.CompositionManager;
            CompositionManager = compositionManager;
            PrefabService = prefabService;
            MasterBeat = masterBeat;
            OutputMappingManager = outputMappingManager;
            RedisAddressRegistry = redisAddressRegistry;
            Model = model;
            Model.FilePath.ID = ManagerIDs.ProjectModelFilePath;
            PrefabService.ID = this.ID;
            PrefabService.Name.ID = ManagerIDs.ProjectPrefabServiceName;
            PrefabService.Visibility.ID = ManagerIDs.ProjectPrefabServiceVisibility;
            compositionManager.Activate();
            controlRepository.AddRoot(this);
        }

        public Guid ID { get; set; }
        public PrefabManager CompositionManager { get; set; }
        public PrefabService PrefabService { get; set; }
        public MasterBeat MasterBeat { get; set; }
        public OutputMappingManager OutputMappingManager { get; set; }
        public RedisAddressRegistry RedisAddressRegistry { get; set; }

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
            RedisAddressManager = (PrefabManagerModel)RedisAddressRegistry.AddressManager.ToModel(),
            Model = (AssetSelectorModel)Model.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (ProjectModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            PrefabService.Name.ID = ManagerIDs.ProjectPrefabServiceName;
            PrefabService.Visibility.ID = ManagerIDs.ProjectPrefabServiceVisibility;
            MasterBeat.FromModel(m.MasterBeat);
            MasterBeat.ID = ManagerIDs.MasterBeat;
            OutputMappingManager.FromModel(m.OutputMappingManager);
            Model.FromModel(m.Model);

            LoadManager(CompositionManager, m.CompositionManager);
            LoadAddresses(m.RedisAddressManager);
        }

        // Gives the output mappings, the name and the model the values of a new project. The master
        // beat keeps its tempo. The values are set directly because FromModel replaces the fixed ids.
        public void Reset()
        {
            OutputMappingManager.Reset();
            RedisAddressRegistry.AddressManager.ClearAll();
            PrefabService.Name.Value = null;
            PrefabService.Visibility.Value = false;
            Model.FilePath.Value = string.Empty;
        }

        // Applies a project file to an empty session. The compositions are the file's compositions,
        // already given new ids, in file order.
        public void Open(ProjectModel model, IReadOnlyList<CompositionModel> compositions)
        {
            // The slots are restored before the compositions, because a composition refers to a slot.
            if (model.OutputMappingManager != null)
                OutputMappingManager.FromModel(model.OutputMappingManager);

            LoadAddresses(model.RedisAddressManager);

            foreach (var composition in compositions)
                CompositionManager.AddItemFromModel(composition);

            // Adding always selects the item just added, so the loop above always leaves the
            // last composition selected. Restore whichever one was actually selected when the
            // file was saved instead, by position in the file's own list.
            var savedIndex = model.CompositionManager.ManagerData.SelectedIndex;
            if (savedIndex >= 0 && savedIndex < compositions.Count)
                CompositionManager.ManagerData.SelectAt(savedIndex);

            // The master beat keeps the ids it was saved with, so it is restored from the
            // file as loaded rather than from the guid replaced clone each composition goes
            // through. Project files written before the master beat was serialized carry
            // none. PrefabService and Model (the project's 3D backdrop) carry their own
            // well known ids the same way and are restored the same way. The output mappings
            // also keep their fixed ids and are restored above, before the compositions.
            if (model.MasterBeat != null)
                MasterBeat.FromModel(model.MasterBeat);

            if (model.PrefabService != null)
                PrefabService.FromModel(model.PrefabService);

            if (model.Model != null)
                Model.FromModel(model.Model);
        }

        // A project file without addresses carries a default model, which must not replace the fixed ID.
        private void LoadAddresses(PrefabManagerModel model)
        {
            if (model == null) return;

            LoadManager(RedisAddressRegistry.AddressManager, model);
            RedisAddressRegistry.AddressManager.ManagerData.ID = ManagerIDs.RedisAddressManager;
        }

        // FromModel only adds items, so clear first to make this a real replacement.
        public void ApplySnapshot(ProjectModel model)
        {
            CompositionManager.ClearAll();
            RedisAddressRegistry.AddressManager.ClearAll();
            FromModel(model);
        }
    }
}
