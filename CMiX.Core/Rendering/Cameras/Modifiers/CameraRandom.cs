// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering.Cameras.Modifiers
{
    public partial class CameraRandom : ObservableObject, ICameraModifier, IBeatModifiable
    {
        public CameraRandom(CameraRandomModel randomModel)
        {
            ID = randomModel.ID;
            Visible = new BooleanValue(randomModel.Visible);
            BeatModifier = new BeatModifier(randomModel.BeatModifier);
            PingPong = new BooleanValue(randomModel.PingPong);
            Axis = new GenericValue<CameraAxis>(randomModel.Axis);
            Easing = new Easing(randomModel.Easing);
            Width = new FloatValue(randomModel.Width);
            isExpanded = true;
        }

        public Guid ID { get; set; }
        public BooleanValue Visible { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public BooleanValue PingPong { get; set; }
        public GenericValue<CameraAxis> Axis { get; set; }
        public Easing Easing { get; set; }
        public FloatValue Width { get; set; }

        [ObservableProperty]
        private bool isExpanded;

        public void Dispose()
        {

        }
    }
}
