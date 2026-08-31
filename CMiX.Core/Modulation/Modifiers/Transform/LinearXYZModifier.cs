// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Modulation.Modifiers
{
    // Renamed from LinearXYZModifier to bare LinearXYZ, claiming the name freed up by the old
    // LinearXYZ (now LinearXYZLegacy) so VL's exact-name matching can target this class directly.
    // One-to-one field parity with the old LinearXYZ is complete: ModifierModeSelector,
    // TransformTypeSelector, and DirectionXYZ are ported as-is (non-modulatable); Width and Phase
    // become modulatable channels - new capability the old LinearXYZ never had.
    [ModifierPanel(typeof(Entity))]
    public partial class LinearXYZModifier : Modifier, ISpreadableModifier
    {
        public LinearXYZModifier(PrefabService prefabService,
                                 PrefabManager modulatorManager,
                                 ModifierModeSelector modifierModeSelector,
                                 GenericValue<TransformType> transformTypeSelector,
                                 DirectionXYZ directionXYZ,
                                 Modulatable width,
                                 Modulatable phase)
            : base(prefabService, modulatorManager)
        {
            ModifierModeSelector = modifierModeSelector;
            TransformTypeSelector = transformTypeSelector;
            DirectionXYZ = directionXYZ;
            width.Label = "Width";
            phase.Label = "Phase";
            Channels = new List<Modulatable> { width, phase };
        }

        // Convenience accessors into Channels, purely for the view's ModulatableValue bindings -
        // Channels itself stays the source of truth (used by Modifier's own ToModel/FromModel).
        public Modulatable Width => Channels[0];
        public Modulatable Phase => Channels[1];

        // Non-modulatable, ported as-is from LinearXYZ for one-to-one field parity.
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public GenericValue<TransformType> TransformTypeSelector { get; set; }
        public DirectionXYZ DirectionXYZ { get; set; }

        public override IControlModel ToModel()
        {
            var model = new LinearXYZModifierModel
            {
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel(),
                TransformTypeSelector = (GenericValueModel<TransformType>)TransformTypeSelector.ToModel(),
                DirectionXYZ = (DirectionXYZModel)DirectionXYZ.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (LinearXYZModifierModel)model;
            LoadBaseModel(m);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            TransformTypeSelector.FromModel(m.TransformTypeSelector);
            DirectionXYZ.FromModel(m.DirectionXYZ);
        }
    }
}
