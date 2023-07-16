// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class HSCB : ObservableObject, IModifier
    {
        public HSCB()
        {
            ID = Guid.NewGuid();
            Visible = new BooleanValue();
            Hue = new FloatValue();
            Saturation = new FloatValue();
            Contrast = new FloatValue();
            Brightness = new FloatValue();
            isExpanded = true;
        }

        public Guid ID { get; set; }
        public BooleanValue Visible { get; set; }
        public FloatValue Hue { get; set; }
        public FloatValue Saturation { get; set; }
        public FloatValue Contrast { get; set; }
        public FloatValue Brightness { get; set; }
        public FloatValue Control { get; set; }

        [ObservableProperty]
        private bool enabled;

        [ObservableProperty]
        private bool isExpanded;
    }
}
