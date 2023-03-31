// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.Beat;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.Modifiers.Transform;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.Modifiers.Color
{
    public class RandomHSV : ObservableObject, IBeatModifiable, IColorModifier
    {
        public RandomHSV(RandomHSVModel randomHSVModel, CompositionService compositionService)
        {
            ID = randomHSVModel.ID;
            Visible = new BooleanValue(randomHSVModel.Visible);

            Hue = new FloatValue(randomHSVModel.Hue);
            Saturation = new FloatValue(randomHSVModel.Saturation);
            Value = new FloatValue(randomHSVModel.Value);
            Alpha = new FloatValue(randomHSVModel.Alpha);

            BeatModifier = new BeatModifier(randomHSVModel.BeatModifier, compositionService);
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


        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }



        public void Dispose()
        {
            BeatModifier.Dispose();
        }
    }
}
