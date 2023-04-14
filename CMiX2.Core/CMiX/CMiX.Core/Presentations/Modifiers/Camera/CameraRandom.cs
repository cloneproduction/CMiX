// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Beat;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.Modifiers.Camera
{
    public partial class CameraRandom : ObservableObject, ICameraModifier, IBeatModifiable
    {
        public CameraRandom(CameraRandomModel randomModel, CompositionService compositionService)
        {
            ID = randomModel.ID;
            Visible = new BooleanValue(randomModel.Visible, compositionService);
            BeatModifier = new BeatModifier(randomModel.BeatModifier, compositionService);
            PingPong = new BooleanValue(randomModel.PingPong, compositionService);
            Axis = new GenericValue<CameraAxis>(randomModel.Axis, compositionService);
            Easing = new Easing(randomModel.Easing, compositionService);
            Width = new FloatValue(randomModel.Width, compositionService);
            IsExpanded = true;
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
