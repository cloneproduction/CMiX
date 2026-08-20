// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering;
using CMiX.Core.Texturing;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Compositing
{
    public partial class Composition : ObservableObject, IPrefab, ITextureModifiable, IModifiable, IDisposable
    {
        public Composition(PrefabService prefabService,
                           PrefabManager prefabManager,
                           PrefabManager textureModifierManager,
                           PrefabManager modifierManager,
                           GenericValue<Guid> selectedOutputMappingID,
                           LayerSettings layerSettings,
                           LayerMaskSettings layerMaskSettings,
                           Project project)
        {
            ID = prefabService.ID;
            PrefabService = prefabService;
            SelectedOutputMappingID = selectedOutputMappingID;
            LayerManager = prefabManager;
            LayerSettings = layerSettings;
            LayerMaskSettings = layerMaskSettings;
            ModifierManager = modifierManager;
            TextureModifierManager = textureModifierManager;
            Project = project;

            SelectedOutputMappingID.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(GenericValue<Guid>.Value))
                    OnPropertyChanged(nameof(SelectedOutputMapping));
            };
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }
        public PrefabManager LayerManager { get; set; }
        public PrefabManager TextureModifierManager { get; set; }
        public GenericValue<Guid> SelectedOutputMappingID { get; set; }
        public PrefabManager ModifierManager { get; set; }
        public LayerSettings LayerSettings { get; set; }
        public Project Project { get; }

        public LayerMaskSettings LayerMaskSettings { get; set; }

        // The output mapping this composition currently renders to, looked up in the project's
        // fixed set of 10 by ID. Null once the referenced slot is disabled or was never set.
        public OutputMapping SelectedOutputMapping
        {
            get => Project.OutputMappingManager.GetByID(SelectedOutputMappingID.Value);
            set => SelectedOutputMappingID.Value = value?.ID ?? Guid.Empty;
        }

        [ObservableProperty]
        private bool textureModifierIsExpanded = false;

        [ObservableProperty]
        private bool outputSettingsIsExpanded = false;

        public IControlModel ToModel() => new CompositionModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            SelectedOutputMappingID = (GenericValueModel<Guid>)SelectedOutputMappingID.ToModel(),
            TextureModifierManager = (PrefabManagerModel)TextureModifierManager.ToModel(),
            LayerManager = (PrefabManagerModel)LayerManager.ToModel(),
            ModifierManager = (PrefabManagerModel)ModifierManager.ToModel(),
            LayerSettings = (LayerSettingsModel)LayerSettings.ToModel(),
            LayerMaskSettings = (LayerMaskSettingsModel)LayerMaskSettings.ToModel(),
        };

        public void FromModel(IControlModel model)
        {
            var m = (CompositionModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            SelectedOutputMappingID.FromModel(m.SelectedOutputMappingID);
            LayerSettings.FromModel(m.LayerSettings);
            LayerMaskSettings.FromModel(m.LayerMaskSettings);

            LoadManager(TextureModifierManager, m.TextureModifierManager);
            LoadManager(LayerManager, m.LayerManager);
            LoadManager(ModifierManager, m.ModifierManager);
        }

        public void Dispose()
        {
            LayerManager.Dispose();
            TextureModifierManager.Dispose();
            ModifierManager.Dispose();
        }
    }
}
