// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class RandomHSV : ObservableObject, IBeatModifiable, IColorModifier
    {
        public RandomHSV(RandomHSVModel randomHSVModel, CompositionService compositionService)
        {
            ID = randomHSVModel.ID;
            Visible = new ToggleButton(randomHSVModel.Visible);

            Hue = new Slider(nameof(Hue), randomHSVModel.Hue);
            Saturation = new Slider(nameof(Saturation), randomHSVModel.Saturation);
            Value = new Slider(nameof(Value), randomHSVModel.Value);
            Alpha = new Slider(nameof(Alpha), randomHSVModel.Alpha);

            BeatModifier = new BeatModifier(randomHSVModel.BeatModifier, compositionService);
            Easing = new Easing(randomHSVModel.Easing);

            Mode = new ComboBox<ModifierMode>(randomHSVModel.Mode);
        }


        public bool Enabled { get; set; }
        public ToggleButton Visible { get; set; }
        public Guid ID { get; set; }
        public Slider Hue { get; set; }
        public Slider Saturation { get; set; }
        public Slider Value { get; set; }
        public Slider Alpha { get; set; }

        public BeatModifier BeatModifier { get; set; }
        public Easing Easing { get; set; }
        public ComboBox<ModifierMode> Mode { get; set; }


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

        public IModel GetModel()
        {
            RandomHSVModel randomHSVModel = new RandomHSVModel();

            randomHSVModel.Enabled = Enabled;
            randomHSVModel.ID = ID;
            randomHSVModel.Hue = (SliderModel)Hue.GetModel();
            randomHSVModel.Saturation = (SliderModel)Saturation.GetModel();
            randomHSVModel.Value = (SliderModel)Value.GetModel();
            randomHSVModel.Alpha = (SliderModel)Alpha.GetModel();

            randomHSVModel.BeatModifier = (BeatModifierModel)BeatModifier.GetModel();
            randomHSVModel.Easing = (EasingModel)Easing.GetModel();
            randomHSVModel.Mode = (ComboBoxModel<ModifierMode>)Mode.GetModel();

            return randomHSVModel;
        }

        public void SetViewModel(IModel model)
        {
            RandomHSVModel randomHSVModel = model as RandomHSVModel;

            Enabled = randomHSVModel.Enabled;
            ID = randomHSVModel.ID;
            Hue.SetViewModel(randomHSVModel.Hue);
            Saturation.SetViewModel(randomHSVModel.Saturation);
            Value.SetViewModel(randomHSVModel.Value);
            Alpha.SetViewModel(randomHSVModel.Alpha);

            BeatModifier.SetViewModel(randomHSVModel.BeatModifier);
            Easing.SetViewModel(randomHSVModel.Easing);
            Mode.SetViewModel(randomHSVModel.Mode);
        }
    }
}
