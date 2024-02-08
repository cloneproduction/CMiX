// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation.Modifiers
{
    public partial class LFO : ObservableObject, IBeatModifiable, IModifier
    {
        public LFO(GenericValue<bool> visible,
            BeatModifier beatModifier,
            GenericValue<bool> pingPong,
            DirectionXYZ directionXYZ,
            GenericValue<TransformType> transformType,
            Easing easing,
            GenericValue<float> from,
            GenericValue<float> to
            )
        {
            Visible = visible; // new GenericValue<bool>(true);
            BeatModifier = beatModifier;// new BeatModifier();
            PingPong = pingPong;// new GenericValue<bool>();
            DirectionXYZ = directionXYZ;// new DirectionXYZ();
            TransformType = transformType;// new GenericValue<TransformType>();
            Easing = easing;// new Easing();
            From = from;// new GenericValue<float>(0.0f);
            To = to; // new GenericValue<float>(1.0f);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<bool> Visible { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public GenericValue<bool> PingPong { get; set; }
        public DirectionXYZ DirectionXYZ { get; set; }
        public GenericValue<TransformType> TransformType { get; set; }
        public Easing Easing { get; set; }
        public GenericValue<float> From { get; set; }
        public GenericValue<float> To { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
