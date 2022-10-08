// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class LFO : ObservableObject, IControl, IBeatModifiable, ITransformModifier, IDisposable
    {
        public LFO(LFOModel lfoModel, CompositionService compositionService)
        {
            Name = lfoModel.Name;
            ID = lfoModel.ID;
            Enabled = lfoModel.Enabled;

            Mode = new ComboBox<ModifierMode>(lfoModel.Mode);

            Visible = new ToggleButton(lfoModel.Visible);
            BeatModifier = new BeatModifier(lfoModel.BeatModifier, compositionService);

            XAxis = new ToggleButton(lfoModel.XAxis);
            YAxis = new ToggleButton(lfoModel.YAxis);
            ZAxis = new ToggleButton(lfoModel.ZAxis);

            PingPong = new ToggleButton(lfoModel.PingPong);
            TransformType = new ComboBox<TransformType>(lfoModel.TransformType);
            Easing = new Easing(lfoModel.Easing);

            From = new Slider(nameof(From), lfoModel.From);
            To = new Slider(nameof(To), lfoModel.To);

            IsExpanded = true;
        }


        public Guid ID { get; set; }
        public bool Enabled { get; set; }
        public ComboBox<ModifierMode> Mode { get; set; }

        public ToggleButton Visible { get; set; }
        public BeatModifier BeatModifier { get; set; }

        public ToggleButton PingPong { get; set; }

        public ToggleButton XAxis { get; set; }
        public ToggleButton YAxis { get; set; }
        public ToggleButton ZAxis { get; set; }


        public ComboBox<TransformType> TransformType { get; set; }
        public Easing Easing { get; set; }

        public Slider From { get; set; }
        public Slider To { get; set; }


        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public IModel GetModel()
        {
            LFOModel lfoModel = new LFOModel();

            lfoModel.ID = this.ID;
            lfoModel.Enabled = Enabled;

            lfoModel.Visible = (ToggleButtonModel)Visible.GetModel();
            lfoModel.BeatModifier = (BeatModifierModel)BeatModifier.GetModel();
            lfoModel.PingPong = (ToggleButtonModel)PingPong.GetModel();

            lfoModel.XAxis = (ToggleButtonModel)XAxis.GetModel();
            lfoModel.YAxis = (ToggleButtonModel)YAxis.GetModel();
            lfoModel.ZAxis = (ToggleButtonModel)ZAxis.GetModel();

            lfoModel.TransformType = (ComboBoxModel<TransformType>)TransformType.GetModel();
            lfoModel.Mode = (ComboBoxModel<ModifierMode>)Mode.GetModel();

            lfoModel.Easing = (EasingModel)Easing.GetModel();

            lfoModel.From = (SliderModel)From.GetModel();
            lfoModel.To = (SliderModel)To.GetModel();

            return lfoModel;
        }

        public void SetViewModel(IModel model)
        {
            LFOModel lfoModel = model as LFOModel;
            ID = lfoModel.ID;
            Enabled = lfoModel.Enabled;

            Mode.SetViewModel(lfoModel.Mode);
            Visible.SetViewModel(lfoModel.Visible);
            BeatModifier.SetViewModel(lfoModel.BeatModifier);
            PingPong.SetViewModel(lfoModel.PingPong);

            XAxis.SetViewModel(lfoModel.XAxis);
            YAxis.SetViewModel(lfoModel.YAxis);
            ZAxis.SetViewModel(lfoModel.ZAxis);

            TransformType.SetViewModel(lfoModel.TransformType);
            Easing.SetViewModel(lfoModel.Easing);

            From.SetViewModel(lfoModel.From);
            To.SetViewModel(lfoModel.To);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
