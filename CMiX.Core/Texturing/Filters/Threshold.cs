// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Threshold : ObservableObject, IPrefab, ITextureFilter
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
        {
            PrefabService = prefabService;
            Control = control;
            Foreground = foreground; 
            Background = background;
            Smooth = smooth;
            ThresholdValue = thresholdValue;
            Antialiasing = antialiasing;
            Invert = invert;
            Blend = blend;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Control { get; set; }
        public GenericValue<float> Smooth { get; set; }
        public GenericValue<float> ThresholdValue { get; set; }
        public GenericValue<string> Foreground { get; set; }
        public GenericValue<string> Background { get; set; }
        public GenericValue<bool> Antialiasing { get; set; }
        public GenericValue<bool> Invert { get; set; }
        public Blend Blend { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new ThresholdModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Control = (GenericValueModel<float>)Control.ToModel(),
            Smooth = (GenericValueModel<float>)Smooth.ToModel(),
            ThresholdValue = (GenericValueModel<float>)ThresholdValue.ToModel(),
            Foreground = (GenericValueModel<string>)Foreground.ToModel(),
            Background = (GenericValueModel<string>)Background.ToModel(),
            Antialiasing = (GenericValueModel<bool>)Antialiasing.ToModel(),
            Invert = (GenericValueModel<bool>)Invert.ToModel(),
            Blend = (BlendModel)Blend.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (ThresholdModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Control.FromModel(m.Control);
            Smooth.FromModel(m.Smooth);
            ThresholdValue.FromModel(m.ThresholdValue);
            Foreground.FromModel(m.Foreground);
            Background.FromModel(m.Background);
            Antialiasing.FromModel(m.Antialiasing);
            Invert.FromModel(m.Invert);
            Blend.FromModel(m.Blend);
        }
    }
}
