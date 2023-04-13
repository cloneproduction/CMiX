// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Beat;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.Modifiers.Transform
{
    public partial class LFO : ObservableObject, IControl, IBeatModifiable, ITransformModifier, IDisposable
    {
        public LFO(LFOModel lfoModel, CompositionService compositionService)
        {
            name = lfoModel.Name;
            ID = lfoModel.ID;

            Mode = new GenericValue<ModifierMode>(lfoModel.Mode, compositionService);

            Visible = new BooleanValue(lfoModel.Visible, compositionService);
            BeatModifier = new BeatModifier(lfoModel.BeatModifier, compositionService);

            XAxis = new BooleanValue(lfoModel.XAxis, compositionService);
            YAxis = new BooleanValue(lfoModel.YAxis, compositionService);
            ZAxis = new BooleanValue(lfoModel.ZAxis, compositionService);

            PingPong = new BooleanValue(lfoModel.PingPong, compositionService);
            TransformType = new GenericValue<TransformType>(lfoModel.TransformType, compositionService);
            Easing = new Easing(lfoModel.Easing, compositionService);

            From = new FloatValue(lfoModel.From, compositionService);
            To = new FloatValue(lfoModel.To, compositionService);

            isExpanded = true;
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

        [ObservableProperty]
        private string name;

        public void Dispose()
        {

        }
    }
}
