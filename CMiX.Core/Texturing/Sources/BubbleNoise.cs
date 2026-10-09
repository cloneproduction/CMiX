// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Sources
{
    public partial class BubbleNoise : TextureSourceBase, ITextureSource
    {
        public BubbleNoise(PrefabService prefabService,
                           PrefabManager textureModifierManager,
                           Integer2 resolution,
                           GenericValue<bool> useCompositionResolution,
                           GenericValue<float> speed,
                           GenericValue<float> frequency,
                           GenericValue<float> contrast,
                           GenericValue<string> backgroundColor,
                           GenericValue<string> bubbleColor)
            : base(prefabService, textureModifierManager, useCompositionResolution)
        {
            Resolution = resolution;
            Speed = speed;
            Frequency = frequency;
            Contrast = contrast;
            BackgroundColor = backgroundColor;
            BubbleColor = bubbleColor;
        }

        public Integer2 Resolution { get; set; }
        public GenericValue<float> Speed { get; set; }
        public GenericValue<float> Frequency { get; set; }
        public GenericValue<float> Contrast { get; set; }
        public GenericValue<string> BackgroundColor { get; set; }
        public GenericValue<string> BubbleColor { get; set; }

        public override IControlModel ToModel()
        {
            var model = new BubbleNoiseModel
            {
                Resolution = (Integer2Model)Resolution.ToModel(),
                Speed = (GenericValueModel<float>)Speed.ToModel(),
                Frequency = (GenericValueModel<float>)Frequency.ToModel(),
                Contrast = (GenericValueModel<float>)Contrast.ToModel(),
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
            Speed.FromModel(m.Speed);
            Frequency.FromModel(m.Frequency);
            Contrast.FromModel(m.Contrast);
            BackgroundColor.FromModel(m.BackgroundColor);
            BubbleColor.FromModel(m.BubbleColor);
        }
    }
}
