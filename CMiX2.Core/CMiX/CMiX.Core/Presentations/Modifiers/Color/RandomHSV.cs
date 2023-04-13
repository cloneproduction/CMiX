// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Beat;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.Modifiers.Color
{
    public partial class RandomHSV : ObservableObject, IBeatModifiable, IColorModifier
    {
        public RandomHSV(RandomHSVModel randomHSVModel, CompositionService compositionService)
        {
            ID = randomHSVModel.ID;
            Visible = new BooleanValue(randomHSVModel.Visible, compositionService);
            Hue = new FloatValue(randomHSVModel.Hue, compositionService);
            Saturation = new FloatValue(randomHSVModel.Saturation, compositionService);
            Value = new FloatValue(randomHSVModel.Value, compositionService);
            Alpha = new FloatValue(randomHSVModel.Alpha, compositionService);
            BeatModifier = new BeatModifier(randomHSVModel.BeatModifier, compositionService);
            Easing = new Easing(randomHSVModel.Easing, compositionService);
            Mode = new GenericValue<ModifierMode>(randomHSVModel.Mode, compositionService);
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
