// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Texturing.Sampling;
using CMiX.Core.Transformation;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class LFOUV : ObservableObject, ITextureFilter, IBeatModifiable
    {
        public LFOUV(LFOUVModel lfoUVModel)
        {
            ID = lfoUVModel.ID;
            Name = lfoUVModel.Name;
            BeatModifier = new BeatModifier(lfoUVModel.BeatModifier);
            Visible = new BooleanValue(lfoUVModel.Visible);
            XAxis = new BooleanValue(lfoUVModel.XAxis);
            YAxis = new BooleanValue(lfoUVModel.YAxis);
            ZAxis = new BooleanValue(lfoUVModel.ZAxis);
            PingPong = new BooleanValue(lfoUVModel.PingPong);
            Mode = new GenericValue<ModifierMode>(lfoUVModel.Mode);
            TransformType = new GenericValue<TransformType>(lfoUVModel.TransformType);
            Easing = new Easing(lfoUVModel.Easing);
            From = new FloatValue(lfoUVModel.From);
            To = new FloatValue(lfoUVModel.To);
            SamplerState = new SamplerState(lfoUVModel.SamplerState);
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
