// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Security.Policy;
using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering.Cameras.Modifiers
{
    public partial class CameraLFO : ObservableObject, IModifier, IBeatModifiable
    {
        public CameraLFO(
                    BooleanValue visible, 
                    BeatModifier beatModifier, 
                    Easing easing, 
                    BooleanValue pingPong, 
                    GenericValue<CameraAxis> axis, 
                    FloatValue from, 
                    FloatValue to
                    )
        {
            Visible = visible;
            BeatModifier = beatModifier;
            Easing = easing;

            PingPong = pingPong;
            Axis = axis;
            From = from;// new FloatValue(0.0f);
            To = to;// new FloatValue(1.0f);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public BooleanValue Visible { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public BooleanValue PingPong { get; set; }
        public GenericValue<CameraAxis> Axis { get; set; }
        public Easing Easing { get; set; }
        public FloatValue From { get; set; }
        public FloatValue To { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
