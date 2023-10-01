// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class TriColor : ObservableObject, ITextureModifier
    {
        public TriColor()
        {
            isExpanded = true;

            Visible = new BooleanValue();
            Control = new FloatValue();

            ColorA = new ColorSelector();
            ColorB = new ColorSelector();
            ColorC = new ColorSelector();

            Smooth = new FloatValue();
            Center = new FloatValue();

            SingleChannel = new BooleanValue();
            ClampColor = new BooleanValue();
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public BooleanValue Visible { get; set; }
        public FloatValue Control { get; set; }
        public FloatValue Smooth { get; set; }
        public FloatValue Center { get; set; }
        public ColorSelector ColorA { get; set; }
        public ColorSelector ColorB { get; set; }
        public ColorSelector ColorC { get; set; }
        public BooleanValue SingleChannel { get; set; }
        public BooleanValue ClampColor { get; set; }

        [ObservableProperty]
        private bool isExpanded;

        [ObservableProperty]
        private bool enabled;
    }
}
