// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Modulation.Modifiers
{
    // Discoverable on both Entity and LightEntity, matching the old RandomHSV's own scope exactly
    // - both stay addable side by side until RandomHSV is confirmed superseded by a live VL
    // check, per this session's migration approach. RandomHSV is not touched by this change.
    // ModifierModeSelector and ColorMode are ported as-is (non-modulatable); Hue, Saturation,
    // Value, and Alpha all become modulatable channels.
    [ModifierPanel(typeof(LightEntity))]
    [ModifierPanel(typeof(Entity))]
    public partial class HSVModifier : Modifier, ISpreadableModifier
    {
        public HSVModifier(PrefabService prefabService,
                           PrefabManager modulatorManager,
                           ModifierModeSelector modifierModeSelector,
                           GenericValue<ColorMode> colorMode,
                           Modulatable hue,
                           Modulatable saturation,
                           Modulatable value,
                           Modulatable alpha)
            : base(prefabService, modulatorManager)
        {
            ModifierModeSelector = modifierModeSelector;
            ColorMode = colorMode;
            hue.Label = "Hue";
            saturation.Label = "Saturation";
            value.Label = "Value";
            alpha.Label = "Alpha";
            Channels = new List<Modulatable> { hue, saturation, value, alpha };
        }

        // Convenience accessors into Channels, purely for the view's ModulatableValue bindings -
        // Channels itself stays the source of truth (used by Modifier's own ToModel/FromModel).
        public Modulatable Hue => Channels[0];
        public Modulatable Saturation => Channels[1];
        public Modulatable Value => Channels[2];
        public Modulatable Alpha => Channels[3];

        // Non-modulatable, ported as-is from RandomHSV for one-to-one field parity.
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public GenericValue<ColorMode> ColorMode { get; set; }

        public override IControlModel ToModel()
        {
            var model = new HSVModifierModel
            {
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel(),
                ColorMode = (GenericValueModel<ColorMode>)ColorMode.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (HSVModifierModel)model;
            LoadBaseModel(m);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            ColorMode.FromModel(m.ColorMode);
        }
    }
}
