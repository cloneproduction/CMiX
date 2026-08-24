// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Transformation.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Colors.Modifiers
{
    [ModifierPanel(typeof(LightEntity))]
    [ModifierPanel(typeof(Entity))]
    public partial class RandomHSV : BeatModifiableModifierBase, ISpreadableModifier
    {
        public RandomHSV(PrefabManager beatModifierManager,
                         PrefabService prefabService,
                         ModifierModeSelector modifierModeSelector,
                         GenericValue<ColorMode> colorMode,
                         GenericValue<float> hue,
                         GenericValue<float> saturation,
                         GenericValue<float> value,
                         GenericValue<float> alpha)
            : base(prefabService, beatModifierManager)
        {
            ModifierModeSelector = modifierModeSelector;
            ColorMode = colorMode;
            Hue = hue;
            Saturation = saturation;
            Value = value;
            Alpha = alpha;
        }

        public GenericValue<ColorMode> ColorMode { get; set; }
        public GenericValue<float> Hue { get; set; }
        public GenericValue<float> Saturation { get; set; }
        public GenericValue<float> Value { get; set; }
        public GenericValue<float> Alpha { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }

        public override IControlModel ToModel()
        {
            var model = new RandomHSVModel
            {
                ColorMode = (GenericValueModel<ColorMode>)ColorMode.ToModel(),
                Hue = (GenericValueModel<float>)Hue.ToModel(),
                Saturation = (GenericValueModel<float>)Saturation.ToModel(),
                Value = (GenericValueModel<float>)Value.ToModel(),
                Alpha = (GenericValueModel<float>)Alpha.ToModel(),
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (RandomHSVModel)model;
            LoadBaseModel(m);
            ColorMode.FromModel(m.ColorMode);
            Hue.FromModel(m.Hue);
            Saturation.FromModel(m.Saturation);
            Value.FromModel(m.Value);
            Alpha.FromModel(m.Alpha);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
        }
    }
}
