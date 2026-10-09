// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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
    public partial class Composition : ObservableObject, IPrefab, ITextureModifiable, IModifiable, IDisposable, IComposable
    {
        public Composition(PrefabService prefabService,
                           PrefabManager prefabManager,
                           PrefabManager textureModifierManager,
                           PrefabManager modifierManager,
                           GenericValue<Guid> selectedOutputMappingID,
                           CompositingSettings compositingSettings,
                           MaskSettings maskSettings,
                           Project project)
        {
            ID = prefabService.ID;
            PrefabService = prefabService;
            SelectedOutputMappingID = selectedOutputMappingID;
            LayerManager = prefabManager;
            Compositing = compositingSettings;
            Mask = maskSettings;
            ModifierManager = modifierManager;
            TextureModifierManager = textureModifierManager;
            Project = project;

            SelectedOutputMappingID.Value = project.OutputMappingManager.Items.FirstOrDefault()?.ID ?? Guid.Empty;

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
        public CompositingSettings Compositing { get; set; }
        public Project Project { get; }

        public MaskSettings Mask { get; set; }

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
            Compositing = (CompositingSettingsModel)Compositing.ToModel(),
            Mask = (MaskSettingsModel)Mask.ToModel(),
        };

        public void FromModel(IControlModel model)
        {
            var m = (CompositionModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);

            if (m.SelectedOutputMappingID.Value != Guid.Empty &&
                Project.OutputMappingManager.GetByID(m.SelectedOutputMappingID.Value) != null)
                SelectedOutputMappingID.FromModel(m.SelectedOutputMappingID);

            Compositing.FromModel(m.Compositing);
            Mask.FromModel(m.Mask);

            LoadManager(TextureModifierManager, m.TextureModifierManager);
            LoadManager(LayerManager, m.LayerManager);
            LoadManager(ModifierManager, m.ModifierManager);

            ModifierManager.CompositionID = ID;
            TextureModifierManager.CompositionID = ID;
            LayerManager.CompositionID = ID;
        }

        public void Dispose() => DisposeAll
        (
            LayerManager, 
            TextureModifierManager, 
            ModifierManager
        );
    }
}
