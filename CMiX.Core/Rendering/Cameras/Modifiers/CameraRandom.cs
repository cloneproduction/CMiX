// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering.Cameras.Modifiers
{
    public partial class CameraRandom : ObservableObject, ICameraModifier, IBeatModifiable
    {
        public CameraRandom(GenericValue<bool> visible, BeatModifier beatModifier, Easing easing, GenericValue<bool> pingPong, GenericValue<CameraAxis> axis, GenericValue<float> width)
        {
            Visible = visible;
            BeatModifier = beatModifier;
            Easing = easing;

            PingPong = pingPong;
            Axis = axis;
            Width = width; // new GenericValue<float>(0.0f);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<bool> Visible { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public GenericValue<bool> PingPong { get; set; }
        public GenericValue<CameraAxis> Axis { get; set; }
        public Easing Easing { get; set; }
        public GenericValue<float> Width { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
