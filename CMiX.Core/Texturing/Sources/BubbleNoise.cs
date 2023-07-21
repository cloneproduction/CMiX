// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Media;
using CMiX.Core.BaseControls;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Sources
{
    public class BubbleNoise : ObservableObject, ITextureSource
    {
        public BubbleNoise()
        {
            Resolution = new Integer2(512, 512);
            Speed = new FloatValue(0.0f);
            Frequency = new FloatValue(3.5f);
            Contrast = new FloatValue(0.15f);
            BackgroundColor = new ColorSelector(Color.FromArgb(255, 255, 255, 255));
            BubbleColor = new ColorSelector(Color.FromArgb(255, 0, 0, 0));
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public Integer2 Resolution { get; set; }
        public FloatValue Speed { get; set; }
        public FloatValue Frequency { get; set; }
        public FloatValue Contrast { get; set; }
        public ColorSelector BackgroundColor { get; set; }
        public ColorSelector BubbleColor { get; set; }
    }
}
