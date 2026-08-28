// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;
using static VL.Core.Import.ProcessNodeFactory;

namespace CMiX.Core.Transformation.Modifiers
{
    // Renamed from LinearXYZ, [ModifierPanel] removed - superseded by
    // Modulation.LinearXYZModifier (now renamed to bare LinearXYZ, claiming this name in the
    // VL-facing engine-side patch). Kept only so any already-saved Project data referencing the
    // old LinearXYZ still loads and renders correctly; no longer addable via the "Add Modifier"
    // picker.
    public partial class LinearXYZLegacy : ObservableObject, ISpreadableModifier
    {
        public LinearXYZLegacy(PrefabService prefabService,
                         ModifierModeSelector modifierModeSelector, 
                         GenericValue<TransformType> transformType, 
                         GenericValue<float> width, 
                         GenericValue<float> phase, 
                         DirectionXYZ directionXYZ)
        {
            PrefabService = prefabService;
            ModifierModeSelector = modifierModeSelector;
            TransformTypeSelector = transformType;
            Width = width;
            Phase = phase;
            DirectionXYZ = directionXYZ;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public GenericValue<TransformType> TransformTypeSelector { get; set; }
        public GenericValue<float> Width { get; set; }
        public GenericValue<float> Phase { get; set; }
        public DirectionXYZ DirectionXYZ { get; set; }


        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new LinearXYZLegacyModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Width = (GenericValueModel<float>)Width.ToModel(),
            DirectionXYZ = (DirectionXYZModel)DirectionXYZ.ToModel(),
            Phase = (GenericValueModel<float>)Phase.ToModel(),
            TransformTypeSelector = (GenericValueModel<TransformType>)TransformTypeSelector.ToModel(),
            ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (LinearXYZLegacyModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Width.FromModel(m.Width);
            DirectionXYZ.FromModel(m.DirectionXYZ);
            Phase.FromModel(m.Phase);
            TransformTypeSelector.FromModel(m.TransformTypeSelector);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
        }
    }
}
