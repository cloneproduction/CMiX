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
                    GenericValue<bool> visible, 
                    BeatModifier beatModifier, 
                    Easing easing, 
                    GenericValue<bool> pingPong, 
                    GenericValue<CameraAxis> axis, 
                    GenericValue<float> from, 
                    GenericValue<float> to
                    )
        {
            Visible = visible;
            BeatModifier = beatModifier;
            Easing = easing;

            PingPong = pingPong;
            Axis = axis;
            From = from;// new GenericValue<float>(0.0f);
            To = to;// new GenericValue<float>(1.0f);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<bool> Visible { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public GenericValue<bool> PingPong { get; set; }
        public GenericValue<CameraAxis> Axis { get; set; }
        public Easing Easing { get; set; }
        public GenericValue<float> From { get; set; }
        public GenericValue<float> To { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
