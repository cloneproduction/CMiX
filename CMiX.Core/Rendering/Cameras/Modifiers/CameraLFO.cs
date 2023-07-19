// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering.Cameras.Modifiers
{
    public partial class CameraLFO : ObservableObject, IModifier, IBeatModifiable
    {
        public CameraLFO()
        {
            Visible = new BooleanValue(true);
            BeatModifier = new BeatModifier();
            PingPong = new BooleanValue();
            Axis = new GenericValue<CameraAxis>();
            Easing = new Easing();
            From = new FloatValue(0.0f);
            To = new FloatValue(1.0f);
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
