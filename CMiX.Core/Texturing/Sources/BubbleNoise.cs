// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

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
            : base(prefabService, textureModifierManager)
        {
            Resolution = resolution;
            UseCompositionResolution = useCompositionResolution;
            Speed = speed;
            Frequency = frequency;
            Contrast = contrast;
            BackgroundColor = backgroundColor;
            BubbleColor = bubbleColor;
        }

        public Integer2 Resolution { get; set; }
        public GenericValue<bool> UseCompositionResolution { get; set; }
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
                UseCompositionResolution = (GenericValueModel<bool>)UseCompositionResolution.ToModel(),
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
            UseCompositionResolution.FromModel(m.UseCompositionResolution);
            Speed.FromModel(m.Speed);
            Frequency.FromModel(m.Frequency);
            Contrast.FromModel(m.Contrast);
            BackgroundColor.FromModel(m.BackgroundColor);
            BubbleColor.FromModel(m.BubbleColor);
        }
    }
}
