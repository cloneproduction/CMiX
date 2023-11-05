// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class TriColor : ObservableObject, ITextureModifier
    {
        public TriColor(BooleanValue visible, FloatValue control, ColorValue colorA, ColorValue colorB, ColorValue colorC, FloatValue smooth, FloatValue center, BooleanValue singleChannel, BooleanValue clampColor)
        {
            isExpanded = true;

            Visible = visible;
            Control = control;

            ColorA = colorA;
            ColorB = colorB;
            ColorC = colorC;

            Smooth = smooth;
            Center = center;

            SingleChannel = singleChannel;
            ClampColor = clampColor;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public BooleanValue Visible { get; set; }
        public FloatValue Control { get; set; }
        public FloatValue Smooth { get; set; }
        public FloatValue Center { get; set; }
        public ColorValue ColorA { get; set; }
        public ColorValue ColorB { get; set; }
        public ColorValue ColorC { get; set; }
        public BooleanValue SingleChannel { get; set; }
        public BooleanValue ClampColor { get; set; }

        [ObservableProperty]
        private bool isExpanded;

        [ObservableProperty]
        private bool enabled;
    }
}
