// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.Beat;
using CMiX.Core.Presentations.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.Modifiers.Transform
{
    public partial class LFO : ObservableObject, IControl, IBeatModifiable, ITransformModifier, IDisposable
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


        [ObservableProperty]
        private bool isExpanded;


        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }


        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
