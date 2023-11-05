// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class HSCB : ObservableObject, ITextureModifier
    {
        public HSCB(BooleanValue visible, FloatValue hue, FloatValue saturation, FloatValue contrast, FloatValue brightness)
        {
            Visible = visible; // new BooleanValue(true);
            Hue = hue; // new FloatValue(0.0f);
            Saturation = saturation; // new FloatValue(1.0f);
            Contrast = contrast; // new FloatValue(0.0f);
            Brightness = brightness; // new FloatValue(0.0f);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public BooleanValue Visible { get; set; }
        public FloatValue Hue { get; set; }
        public FloatValue Saturation { get; set; }
        public FloatValue Contrast { get; set; }
        public FloatValue Brightness { get; set; }
        public FloatValue Control { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
