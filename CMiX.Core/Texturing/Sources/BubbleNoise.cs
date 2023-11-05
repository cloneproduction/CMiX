// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Sources
{
    public class BubbleNoise : ObservableObject, ITextureSource
    {
        public BubbleNoise(Integer2 resolution, FloatValue speed, FloatValue frequency, FloatValue contrast, ColorValue backgroundColor, ColorValue bubbleColor)
        {
            Resolution = resolution; // new Integer2(512, 512);
            Speed = speed;// new FloatValue(0.0f);
            Frequency = frequency; // new FloatValue(3.5f);
            Contrast = contrast;// new FloatValue(0.15f);
            BackgroundColor = backgroundColor; // new ColorValue(Color.FromArgb(255, 255, 255, 255));
            BubbleColor = bubbleColor; // new ColorValue(Color.FromArgb(255, 0, 0, 0));
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public Integer2 Resolution { get; set; }
        public FloatValue Speed { get; set; }
        public FloatValue Frequency { get; set; }
        public FloatValue Contrast { get; set; }
        public ColorValue BackgroundColor { get; set; }
        public ColorValue BubbleColor { get; set; }
    }
}
