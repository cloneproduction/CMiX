// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Colors.Modifiers
{
    public partial class RandomHSV : ObservableObject, IBeatModifiable, IColorModifier
    {
        public RandomHSV(GenericValue<bool> visible, BeatModifier beatModifier, Easing easing, GenericValue<ModifierMode> mode, GenericValue<float> hue, GenericValue<float> saturation, GenericValue<float> value, GenericValue<float> alpha)
        {
            Visible = visible;
            BeatModifier = beatModifier;
            Easing = easing;
            Mode = mode;

            Hue = hue;
            Saturation = saturation;
            Value = value;
            Alpha = alpha;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<bool> Visible { get; set; }
        public GenericValue<float> Hue { get; set; }
        public GenericValue<float> Saturation { get; set; }
        public GenericValue<float> Value { get; set; }
        public GenericValue<float> Alpha { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public Easing Easing { get; set; }
        public GenericValue<ModifierMode> Mode { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
