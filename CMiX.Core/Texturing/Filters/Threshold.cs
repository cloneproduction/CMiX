// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Threshold : TextureFilterBase
    {
        public Threshold(PrefabService prefabService,
                        GenericValue<float> control,
                        GenericValue<string> foreground,
                        GenericValue<string> background,
                        GenericValue<float> smooth,
                        GenericValue<float> thresholdValue,
                        GenericValue<bool> antialiasing,
                        GenericValue<bool> invert,
                        Blend blend)
            : base(prefabService, control, blend)
        {
            Foreground = foreground;
            Background = background;
            Smooth = smooth;
            ThresholdValue = thresholdValue;
            Antialiasing = antialiasing;
            Invert = invert;
        }

        public GenericValue<float> Smooth { get; set; }
        public GenericValue<float> ThresholdValue { get; set; }
        public GenericValue<string> Foreground { get; set; }
        public GenericValue<string> Background { get; set; }
        public GenericValue<bool> Antialiasing { get; set; }
        public GenericValue<bool> Invert { get; set; }

        public override IControlModel ToModel()
        {
            var model = new ThresholdModel
            {
                Smooth = (GenericValueModel<float>)Smooth.ToModel(),
                ThresholdValue = (GenericValueModel<float>)ThresholdValue.ToModel(),
                Foreground = (GenericValueModel<string>)Foreground.ToModel(),
                Background = (GenericValueModel<string>)Background.ToModel(),
                Antialiasing = (GenericValueModel<bool>)Antialiasing.ToModel(),
                Invert = (GenericValueModel<bool>)Invert.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (ThresholdModel)model;
            LoadBaseModel(m);
            Smooth.FromModel(m.Smooth);
            ThresholdValue.FromModel(m.ThresholdValue);
            Foreground.FromModel(m.Foreground);
            Background.FromModel(m.Background);
            Antialiasing.FromModel(m.Antialiasing);
            Invert.FromModel(m.Invert);
        }
    }
}
