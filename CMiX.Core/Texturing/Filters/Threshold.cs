// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Threshold : TextureFilterBase
    {
        public Threshold(PrefabService prefabService,
                        GenericValue<float> control,
                        GenericValue<string> foreground,
                        GenericValue<string> background,
                        ModulatableValue<float> smooth,
                        ModulatableValue<float> thresholdValue,
                        GenericValue<bool> antialiasing,
                        GenericValue<bool> invert,
                        Blend blend,
                        PrefabManager modulatorManager)
            : base(prefabService, control, blend, modulatorManager)
        {
            Foreground = foreground;
            Background = background;
            Antialiasing = antialiasing;
            Invert = invert;

            Bindables = new List<ModulatableValue<float>> { smooth, thresholdValue };

            smooth.Label = "Smooth";
            thresholdValue.Label = "Threshold";
            smooth.SetDefault(0.5f);
            thresholdValue.SetDefault(0.5f);
        }

        public ModulatableValue<float> Smooth => Bindables[0];
        public ModulatableValue<float> ThresholdValue => Bindables[1];
        public GenericValue<string> Foreground { get; set; }
        public GenericValue<string> Background { get; set; }
        public GenericValue<bool> Antialiasing { get; set; }
        public GenericValue<bool> Invert { get; set; }

        public override IControlModel ToModel()
        {
            var model = new ThresholdModel
            {
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
            Foreground.FromModel(m.Foreground);
            Background.FromModel(m.Background);
            Antialiasing.FromModel(m.Antialiasing);
            Invert.FromModel(m.Invert);
        }
    }
}
