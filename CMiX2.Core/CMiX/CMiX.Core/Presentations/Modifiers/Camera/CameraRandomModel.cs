// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;
using CMiX.Core.Beat;
using CMiX.Core.Presentations.Animation;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.BaseControl;

namespace CMiX.Core.Presentations.Modifiers.Camera
{
    public class CameraRandomModel : IModifierModel
    {
        public CameraRandomModel()
        {
            ID = Guid.NewGuid();
            Visible = new BooleanValueModel(true);
            PingPong = new BooleanValueModel(false);
            BeatModifier = new BeatModifierModel();
            Easing = new EasingModel();
            Width = new FloatValueModel(0.0f);
            To = new FloatValueModel(1.0f);
            Axis = new GenericValueModel<CameraAxis>(CameraAxis.Zoom);
        }

        public Guid ID { get; set; }
        public BooleanValueModel Visible { get; set; }

        public BooleanValueModel PingPong { get; set; }
        public BeatModifierModel BeatModifier { get; set; }
        public EasingModel Easing { get; set; }
        public FloatValueModel Width { get; set; }
        public FloatValueModel To { get; set; }
        public GenericValueModel<CameraAxis> Axis { get; set; }

    }
}
