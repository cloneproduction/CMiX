// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Animations;
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
                       MasterBeat masterBeat,
                       OutputMappingManager outputMappingManager,
                       RedisAddressRegistry redisAddressRegistry,
                       ControlRepository controlRepository)
        {
            compositionManager.ManagerData.ID = ManagerIDs.CompositionManager;
            ID = ManagerIDs.CompositionManager;
            CompositionManager = compositionManager;
            PrefabService = prefabService;
            MasterBeat = masterBeat;
            OutputMappingManager = outputMappingManager;
            RedisAddressRegistry = redisAddressRegistry;
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


        public IControlModel ToModel() => new ProjectModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            CompositionManager = (PrefabManagerModel)CompositionManager.ToModel(),
            MasterBeat = (MasterBeatModel)MasterBeat.ToModel(),
            OutputMappingManager = (OutputMappingManagerModel)OutputMappingManager.ToModel(),
            RedisAddressManager = (PrefabManagerModel)RedisAddressRegistry.AddressManager.ToModel()
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

            // A Redis modulator selects an address, so the addresses come before the compositions.
            LoadAddresses(m.RedisAddressManager);
            LoadManager(CompositionManager, m.CompositionManager);
        }

        // Gives the output mappings and the name the values of a new project. The master
        // beat keeps its tempo. The values are set directly because FromModel replaces the fixed ids.
        public void Reset()
        {
            OutputMappingManager.Reset();
            RedisAddressRegistry.AddressManager.ClearAll();
            PrefabService.Name.Value = null;
            PrefabService.Visibility.Value = false;
        }

        // A project file without addresses carries a default model, which must not replace the fixed ID.
        private void LoadAddresses(PrefabManagerModel model)
        {
            if (model == null) return;

            LoadManager(RedisAddressRegistry.AddressManager, model);
            RedisAddressRegistry.AddressManager.ManagerData.ID = ManagerIDs.RedisAddressManager;
        }

        // True while a snapshot is applied: a file that opens or a project that a peer sends. A view can tell loaded items from new ones.
        public bool IsLoading { get; private set; }

        // FromModel only adds items, so clear first to make this a real replacement.
        public void ApplySnapshot(ProjectModel model)
        {
            IsLoading = true;
            try
            {
                CompositionManager.ClearAll();
                RedisAddressRegistry.AddressManager.ClearAll();
                FromModel(model);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
