// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Beat;
using CMiX.Core.Presentations.Modifiers;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.Texturing.Sampling;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CMiX.Core.Texturing.Filters;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.ViewModels
{
    public partial class LFOUV : ObservableObject, ITextureFilter, IBeatModifiable
    {
        public LFOUV(LFOUVModel lfoUVModel, CompositionService compositionService)
        {
            ID = lfoUVModel.ID;
            Name = lfoUVModel.Name;
            BeatModifier = new BeatModifier(lfoUVModel.BeatModifier, compositionService);
            Visible = new BooleanValue(lfoUVModel.Visible, compositionService);
            XAxis = new BooleanValue(lfoUVModel.XAxis, compositionService);
            YAxis = new BooleanValue(lfoUVModel.YAxis, compositionService);
            ZAxis = new BooleanValue(lfoUVModel.ZAxis, compositionService);
            PingPong = new BooleanValue(lfoUVModel.PingPong, compositionService);
            Mode = new GenericValue<ModifierMode>(lfoUVModel.Mode, compositionService);
            TransformType = new GenericValue<TransformType>(lfoUVModel.TransformType, compositionService);
            Easing = new Easing(lfoUVModel.Easing, compositionService);
            From = new FloatValue(lfoUVModel.From, compositionService);
            To = new FloatValue(lfoUVModel.To, compositionService);
            SamplerState = new SamplerState(lfoUVModel.SamplerState, compositionService);
        }

        public Guid ID { get; set; }
        public TextureFilterName Name { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public BooleanValue Visible { get; set; }
        public BooleanValue PingPong { get; set; }
        public BooleanValue XAxis { get; set; }
        public BooleanValue YAxis { get; set; }
        public BooleanValue ZAxis { get; set; }
        public GenericValue<ModifierMode> Mode { get; set; }
        public GenericValue<TransformType> TransformType { get; set; }
        public Easing Easing { get; set; }
        public FloatValue From { get; set; }
        public FloatValue To { get; set; }
        public SamplerState SamplerState { get; set; }

        [ObservableProperty]
        private bool isExpanded;

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
