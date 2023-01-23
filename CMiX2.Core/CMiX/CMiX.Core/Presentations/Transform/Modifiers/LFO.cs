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

            Mode = new GenericValue<ModifierMode>(lfoModel.Mode);

            Visible = new BooleanValue(lfoModel.Visible);
            BeatModifier = new BeatModifier(lfoModel.BeatModifier, compositionService);

            XAxis = new BooleanValue(lfoModel.XAxis);
            YAxis = new BooleanValue(lfoModel.YAxis);
            ZAxis = new BooleanValue(lfoModel.ZAxis);

            PingPong = new BooleanValue(lfoModel.PingPong);
            TransformType = new GenericValue<TransformType>(lfoModel.TransformType);
            Easing = new Easing(lfoModel.Easing);

            From = new FloatValue(lfoModel.From);
            To = new FloatValue(lfoModel.To);

            IsExpanded = true;
        }


        public Guid ID { get; set; }
        public GenericValue<ModifierMode> Mode { get; set; }

        public BooleanValue Visible { get; set; }
        public BeatModifier BeatModifier { get; set; }

        public BooleanValue PingPong { get; set; }

        public BooleanValue XAxis { get; set; }
        public BooleanValue YAxis { get; set; }
        public BooleanValue ZAxis { get; set; }


        public GenericValue<TransformType> TransformType { get; set; }
        public Easing Easing { get; set; }

        public FloatValue From { get; set; }
        public FloatValue To { get; set; }


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

            lfoModel.Visible = (BooleanValueModel)Visible.GetModel();
            lfoModel.BeatModifier = (BeatModifierModel)BeatModifier.GetModel();
            lfoModel.PingPong = (BooleanValueModel)PingPong.GetModel();

            lfoModel.XAxis = (BooleanValueModel)XAxis.GetModel();
            lfoModel.YAxis = (BooleanValueModel)YAxis.GetModel();
            lfoModel.ZAxis = (BooleanValueModel)ZAxis.GetModel();

            lfoModel.TransformType = (GenericValueModel<TransformType>)TransformType.GetModel();
            lfoModel.Mode = (GenericValueModel<ModifierMode>)Mode.GetModel();

            lfoModel.Easing = (EasingModel)Easing.GetModel();

            lfoModel.From = (FloatValueModel)From.GetModel();
            lfoModel.To = (FloatValueModel)To.GetModel();

            return lfoModel;
        }

        public void SetViewModel(IModel model)
        {
            LFOModel lfoModel = model as LFOModel;
            ID = lfoModel.ID;

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
