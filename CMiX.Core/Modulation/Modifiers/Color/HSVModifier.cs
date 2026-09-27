// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
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
                           ControlRepository controlRepository,
                           ModifierModeSelector modifierModeSelector,
                           GenericValue<ColorMode> colorMode,
                           ModulatableValue<float> hue,
                           ModulatableValue<float> saturation,
                           ModulatableValue<float> value,
                           ModulatableValue<float> alpha)
            : base(prefabService, modulatorManager, controlRepository, modifierModeSelector.Bindables)
        {
            ModifierModeSelector = modifierModeSelector;
            ColorMode = colorMode;

            hue.Label = "Hue";
            saturation.Label = "Saturation";
            value.Label = "Value";
            alpha.Label = "Alpha";

            Bindables = new List<ModulatableValue<float>> { hue, saturation, value, alpha };

            for (int i = 0; i < Bindables.Count; i++)
                Bindables[i].SetDefault(1f);
        }

        public ModulatableValue<float> Hue => Bindables[0];
        public ModulatableValue<float> Saturation => Bindables[1];
        public ModulatableValue<float> Value => Bindables[2];
        public ModulatableValue<float> Alpha => Bindables[3];

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
