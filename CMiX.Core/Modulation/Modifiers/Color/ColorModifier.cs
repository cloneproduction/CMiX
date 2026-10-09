// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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
    public partial class ColorModifier : Modifier, ISpreadableModifier
    {
        public ColorModifier(PrefabService prefabService,
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

            Bindables = new List<ModulatableValue<float>> { hue, saturation, value, alpha };

            // Reacts to a mode switch: converts the three channels so the perceived
            // color stays the same, then notifies the captions describing them.
            ColorMode.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName != nameof(GenericValue<ColorMode>.Value)) return;

                // TODO: convert Hue/Saturation/Value's current numbers into the new
                // mode's channels here, before the UI re-reads them.

                OnPropertyChanged(nameof(FirstCaption));
                OnPropertyChanged(nameof(SecondCaption));
                OnPropertyChanged(nameof(ThirdCaption));
            };
        }

        public ModulatableValue<float> Hue => Bindables[0];
        public ModulatableValue<float> Saturation => Bindables[1];
        public ModulatableValue<float> Value => Bindables[2];
        public ModulatableValue<float> Alpha => Bindables[3];

        public ModifierModeSelector ModifierModeSelector { get; set; }
        public GenericValue<ColorMode> ColorMode { get; set; }

        public string FirstCaption => ColorMode.Value switch
        {
            Core.ColorMode.RGB => "Red",
            Core.ColorMode.HSL => "Hue",
            Core.ColorMode.HSV => "Hue",
            _ => "Hue",
        };

        public string SecondCaption => ColorMode.Value switch
        {
            Core.ColorMode.RGB => "Green",
            Core.ColorMode.HSL => "Saturation",
            Core.ColorMode.HSV => "Saturation",
            _ => "Saturation",
        };

        public string ThirdCaption => ColorMode.Value switch
        {
            Core.ColorMode.RGB => "Blue",
            Core.ColorMode.HSL => "Lightness",
            Core.ColorMode.HSV => "Value",
            _ => "Value",
        };

        public override IControlModel ToModel()
        {
            var model = new ColorModifierModel
            {
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel(),
                ColorMode = (GenericValueModel<ColorMode>)ColorMode.ToModel(),
                Hue = (ModulatableValueModel<float>)Hue.ToModel(),
                Saturation = (ModulatableValueModel<float>)Saturation.ToModel(),
                Value = (ModulatableValueModel<float>)Value.ToModel(),
                Alpha = (ModulatableValueModel<float>)Alpha.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (ColorModifierModel)model;
            LoadBaseModel(m);
            Hue.FromModel(m.Hue);
            Saturation.FromModel(m.Saturation);
            Value.FromModel(m.Value);
            Alpha.FromModel(m.Alpha);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            ResolveNestedBindables();
            ColorMode.FromModel(m.ColorMode);
        }
    }
}
