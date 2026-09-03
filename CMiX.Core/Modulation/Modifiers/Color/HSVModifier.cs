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
    [ModifierPanel(typeof(LightEntity))]
    [ModifierPanel(typeof(Entity))]
    public partial class HSVModifier : Modifier, ISpreadableModifier
    {
        public HSVModifier(PrefabService prefabService,
                           PrefabManager modulatorManager,
                           ModifierModeSelector modifierModeSelector,
                           GenericValue<ColorMode> colorMode,
                           ModulatableFloat hue,
                           ModulatableFloat saturation,
                           ModulatableFloat value,
                           ModulatableFloat alpha)
            : base(prefabService, modulatorManager, modifierModeSelector.Bindables)
        {
            ModifierModeSelector = modifierModeSelector;
            ColorMode = colorMode;
            hue.Label = "Hue";
            saturation.Label = "Saturation";
            value.Label = "Value";
            alpha.Label = "Alpha";
            Bindables = new List<ModulatableFloat> { hue, saturation, value, alpha };
        }

        public ModulatableFloat Hue => Bindables[0];
        public ModulatableFloat Saturation => Bindables[1];
        public ModulatableFloat Value => Bindables[2];
        public ModulatableFloat Alpha => Bindables[3];

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
            ResolveNestedBindables();
            ColorMode.FromModel(m.ColorMode);
        }
    }
}
