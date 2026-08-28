// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering.Lights;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation.Modifiers
{
    // Renamed from Grid, [ModifierPanel] removed - superseded by Modulation.GridModifier (now
    // renamed to bare Grid, claiming this name in the VL-facing engine-side patch). Kept only so
    // any already-saved Project data referencing the old Grid still loads and renders correctly;
    // no longer addable via the "Add Modifier" picker.
    public partial class GridLegacy : ObservableObject, ISpreadableModifier
    {
        public GridLegacy(ModifierModeSelector modifierModeSelector,
                    PrefabService prefabService,
                    Integer3 count,
                    Vector3 width,
                    Vector3 phase)
        {
            ModifierModeSelector = modifierModeSelector;
            PrefabService = prefabService;
            Count = count;
            Width = width;
            Phase = phase;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public Integer3 Count { get; set; }
        public Vector3 Width { get; set; }
        public Vector3 Phase { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new GridLegacyModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel(),
            Count = (Integer3Model)Count.ToModel(),
            Width = (Vector3Model)Width.ToModel(),
            Phase = (Vector3Model)Phase.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (GridLegacyModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            Count.FromModel(m.Count);
            Width.FromModel(m.Width);
            Phase.FromModel(m.Phase);
        }
    }
}
