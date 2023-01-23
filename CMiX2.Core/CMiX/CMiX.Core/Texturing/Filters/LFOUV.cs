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

            Name = lfoUVModel.Name;
            Visible = new BooleanValue(lfoUVModel.Visible);
            BeatModifier = new BeatModifier(lfoUVModel.BeatModifier, compositionService);

            XAxis = new BooleanValue(lfoUVModel.XAxis);
            YAxis = new BooleanValue(lfoUVModel.YAxis);
            ZAxis = new BooleanValue(lfoUVModel.ZAxis);

            PingPong = new BooleanValue(lfoUVModel.PingPong);
            TransformType = new GenericValue<TransformType>(lfoUVModel.TransformType);
            Easing = new Easing(lfoUVModel.Easing);

            From = new FloatValue(lfoUVModel.From);
            To = new FloatValue(lfoUVModel.To);

            SamplerState = new SamplerState(lfoUVModel.SamplerState, compositionService);
        }


        public Guid ID { get; set; }

        public BooleanValue Visible { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public TextureFilterName Name { get; set; }

        public BooleanValue PingPong { get; set; }

        public BooleanValue XAxis { get; set; }
        public BooleanValue YAxis { get; set; }
        public BooleanValue ZAxis { get; set; }

        public GenericValue<TransformType> TransformType { get; set; }
        public Easing Easing { get; set; }

        public FloatValue From { get; set; }
        public FloatValue To { get; set; }
        public SamplerState SamplerState { get; set; }


        public IModel GetModel()
        {
            LFOUVModel lfoModel = new LFOUVModel();

            lfoModel.ID = this.ID;

            lfoModel.Visible = (BooleanValueModel)Visible.GetModel();
            lfoModel.BeatModifier = (BeatModifierModel)BeatModifier.GetModel();
            lfoModel.PingPong = (BooleanValueModel)PingPong.GetModel();

            lfoModel.XAxis = (BooleanValueModel)XAxis.GetModel();
            lfoModel.YAxis = (BooleanValueModel)YAxis.GetModel();
            lfoModel.ZAxis = (BooleanValueModel)ZAxis.GetModel();

            lfoModel.TransformType = (GenericValueModel<TransformType>)TransformType.GetModel();

            lfoModel.Easing = (EasingModel)Easing.GetModel();

            lfoModel.From = (FloatValueModel)From.GetModel();
            lfoModel.To = (FloatValueModel)To.GetModel();
            lfoModel.SamplerState = (SamplerStateModel)SamplerState.GetModel();

            return lfoModel;
        }

        public void SetViewModel(IModel model)
        {
            LFOUVModel lfoModel = model as LFOUVModel;
            ID = lfoModel.ID;

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
