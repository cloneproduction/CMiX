// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;
using CMiX.Core.Transformation.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Rendering.Lights
{
    public partial class LightEntity : ObservableObject, IPrefab, IModifiable, IHasCompositionID, IDisposable
    {
        public LightEntity(PrefabService prefabService,
                           LightSettings settings,
                           TransformSRTModifier transformSRT,
                           PrefabManager modifierManager)
        {
            ID = prefabService.ID;
            PrefabService = prefabService;
            TransformSRT = transformSRT;
            Settings = settings;
            ModifierManager = modifierManager;
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }
        public LightSettings Settings { get; set; }
        public PrefabManager ModifierManager { get; set; }
        public TransformSRTModifier TransformSRT { get; set; }

        private Guid _compositionID;
        public Guid CompositionID
        {
            get => _compositionID;
            set
            {
                _compositionID = value;
                new CompositionIDAssigner(ModifierManager, value);
            }
        }

        [ObservableProperty]
        private bool transformSRTIsExpanded = false;

        [ObservableProperty]
        private bool modifierManagerIsExpanded = true;

        [ObservableProperty]
        private bool settingsIsExpanded = true;

        public IControlModel ToModel() => new LightEntityModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Settings = (LightSettingsModel)Settings.ToModel(),
            TransformSRT = (TransformSRTModifierModel)TransformSRT.ToModel(),
            ModifierManager = (PrefabManagerModel)ModifierManager.ToModel(),
        };

        public void FromModel(IControlModel model)
        {
            var m = (LightEntityModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Settings.FromModel(m.Settings);
            TransformSRT.FromModel(m.TransformSRT);
            LoadManager(ModifierManager, m.ModifierManager);
        }

        public void Dispose() => DisposeAll(ModifierManager, TransformSRT);
    }
}
