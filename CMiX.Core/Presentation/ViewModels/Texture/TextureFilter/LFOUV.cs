// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class LFOUV : ObservableObject, IControl, IBeatModifiable, ITextureFilter, IDisposable
    {
        public LFOUV(LFOUVModel lfoUVModel, CompositionService compositionService)
        {
            ID = lfoUVModel.ID;
            Enabled = lfoUVModel.Enabled;

            Name = lfoUVModel.Name;
            Visible = new ToggleButton(lfoUVModel.Visible);
            BeatModifier = new BeatModifier(lfoUVModel.BeatModifier, compositionService);

            XAxis = new ToggleButton(lfoUVModel.XAxis);
            YAxis = new ToggleButton(lfoUVModel.YAxis);
            ZAxis = new ToggleButton(lfoUVModel.ZAxis);

            PingPong = new ToggleButton(lfoUVModel.PingPong);
            TransformType = new ComboBox<TransformType>(lfoUVModel.TransformType);
            Easing = new Easing(lfoUVModel.Easing);

            From = new Slider(nameof(From), lfoUVModel.From);
            To = new Slider(nameof(To), lfoUVModel.To);

            SamplerState = new SamplerState(lfoUVModel.SamplerState, compositionService);
        }


        public Guid ID { get; set; }
        public bool Enabled { get; set; }

        public ToggleButton Visible { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public TextureFilterName Name { get; set; }

        public ToggleButton PingPong { get; set; }

        public ToggleButton XAxis { get; set; }
        public ToggleButton YAxis { get; set; }
        public ToggleButton ZAxis { get; set; }

        public ComboBox<TransformType> TransformType { get; set; }
        public Easing Easing { get; set; }

        public Slider From { get; set; }
        public Slider To { get; set; }
        public SamplerState SamplerState { get; set; }


        public IModel GetModel()
        {
            LFOUVModel lfoModel = new LFOUVModel();

            lfoModel.ID = this.ID;
            lfoModel.Enabled = Enabled;

            lfoModel.Visible = (ToggleButtonModel)Visible.GetModel();
            lfoModel.BeatModifier = (BeatModifierModel)BeatModifier.GetModel();
            lfoModel.PingPong = (ToggleButtonModel)PingPong.GetModel();

            lfoModel.XAxis = (ToggleButtonModel)XAxis.GetModel();
            lfoModel.YAxis = (ToggleButtonModel)YAxis.GetModel();
            lfoModel.ZAxis = (ToggleButtonModel)ZAxis.GetModel();

            lfoModel.TransformType = (ComboBoxModel<TransformType>)TransformType.GetModel();

            lfoModel.Easing = (EasingModel)Easing.GetModel();

            lfoModel.From = (SliderModel)From.GetModel();
            lfoModel.To = (SliderModel)To.GetModel();
            lfoModel.SamplerState = (SamplerStateModel)SamplerState.GetModel();

            return lfoModel;
        }

        public void SetViewModel(IModel model)
        {
            LFOUVModel lfoModel = model as LFOUVModel;
            ID = lfoModel.ID;
            Enabled = lfoModel.Enabled;

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
            SamplerState.SetViewModel(lfoModel.SamplerState);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
