// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Sources
{
    public partial class BubbleNoise : TextureSourceBase, ITextureSource
    {
        public BubbleNoise(PrefabService prefabService,
                           PrefabManager textureModifierManager,
                           ModulatorHost modulators,
                           Integer2 resolution,
                           GenericValue<bool> useCompositionResolution,
                           ModulatableValue<float> time,
                           ModulatableValue<float> frequency,
                           ModulatableValue<float> contrast,
                           GenericValue<string> backgroundColor,
                           GenericValue<string> bubbleColor)
            : base(prefabService, textureModifierManager, modulators, useCompositionResolution)
        {
            Resolution = resolution;
            Time = time;
            Frequency = frequency;
            Contrast = contrast;
            BackgroundColor = backgroundColor;
            BubbleColor = bubbleColor;

            Modulators.Add(Time, Frequency, Contrast);
        }

        public Integer2 Resolution { get; set; }
        public ModulatableValue<float> Time { get; set; }
        public ModulatableValue<float> Frequency { get; set; }
        public ModulatableValue<float> Contrast { get; set; }
        public GenericValue<string> BackgroundColor { get; set; }
        public GenericValue<string> BubbleColor { get; set; }

        public override IControlModel ToModel()
        {
            var model = new BubbleNoiseModel
            {
                Resolution = (Integer2Model)Resolution.ToModel(),
                Time = (ModulatableValueModel<float>)Time.ToModel(),
                Frequency = (ModulatableValueModel<float>)Frequency.ToModel(),
                Contrast = (ModulatableValueModel<float>)Contrast.ToModel(),
                BackgroundColor = (GenericValueModel<string>)BackgroundColor.ToModel(),
                BubbleColor = (GenericValueModel<string>)BubbleColor.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (BubbleNoiseModel)model;
            LoadBaseModel(m);
            Resolution.FromModel(m.Resolution);
            Time.FromModel(m.Time);
            Frequency.FromModel(m.Frequency);
            Contrast.FromModel(m.Contrast);
            BackgroundColor.FromModel(m.BackgroundColor);
            BubbleColor.FromModel(m.BubbleColor);
        }
    }
}
