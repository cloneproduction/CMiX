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

            // A brand-new composition otherwise starts with no output selected (Guid.Empty),
            // rendering nowhere until the user opens Settings and picks one. Default to the
            // first of the project's fixed output slots instead - see FromModel for why a
            // loaded model's own valid selection still wins over this default.
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
            // Keep the constructor default when the model has no selection (Guid.Empty).
            // A new composition and an old project file give such a model.
            // Also keep the default when the ID names no output slot of the project.
            // Duplicate and open replace every GUID in the model JSON, and this ID too.
            if (m.SelectedOutputMappingID.Value != Guid.Empty &&
                Project.OutputMappingManager.GetByID(m.SelectedOutputMappingID.Value) != null)
                SelectedOutputMappingID.FromModel(m.SelectedOutputMappingID);
            LayerSettings.FromModel(m.LayerSettings);
            LayerMaskSettings.FromModel(m.LayerMaskSettings);

            LoadManager(TextureModifierManager, m.TextureModifierManager);
            LoadManager(LayerManager, m.LayerManager);
            LoadManager(ModifierManager, m.ModifierManager);

            // FromModel runs for every composition, fresh or loaded, right after construction,
            // and ID above is the composition's real, final id from this point on. The composition
            // is the root of the ownership tree, so this is where its id first enters the tree,
            // sent down to every item already in these managers and to anything added later.
            new CompositionIDAssigner(ModifierManager, ID);
            new CompositionIDAssigner(TextureModifierManager, ID);
            new CompositionIDAssigner(LayerManager, ID);
        }

        public void Dispose() => DisposeAll(LayerManager, TextureModifierManager, ModifierManager);
    }
}
