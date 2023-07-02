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
        public RandomHSV(RandomHSVModel randomHSVModel)
        {
            ID = randomHSVModel.ID;
            Visible = new BooleanValue(randomHSVModel.Visible);
            Hue = new FloatValue(randomHSVModel.Hue);
            Saturation = new FloatValue(randomHSVModel.Saturation);
            Value = new FloatValue(randomHSVModel.Value);
            Alpha = new FloatValue(randomHSVModel.Alpha);
            BeatModifier = new BeatModifier(randomHSVModel.BeatModifier);
            Easing = new Easing(randomHSVModel.Easing);
            Mode = new GenericValue<ModifierMode>(randomHSVModel.Mode);
        }

        public BooleanValue Visible { get; set; }
        public Guid ID { get; set; }
        public FloatValue Hue { get; set; }
        public FloatValue Saturation { get; set; }
        public FloatValue Value { get; set; }
        public FloatValue Alpha { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public Easing Easing { get; set; }
        public GenericValue<ModifierMode> Mode { get; set; }

        [ObservableProperty]
        private bool isExpanded;

        public void Dispose()
        {
            BeatModifier.Dispose();
        }
    }
}
