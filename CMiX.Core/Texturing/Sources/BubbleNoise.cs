// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Sources
{
    public class BubbleNoise : ObservableObject, ITextureSource
    {
        public BubbleNoise()
        {
            ID = Guid.NewGuid();
            Resolution = new Integer2();
            Speed = new FloatValue();
            Frequency = new FloatValue();
            Contrast = new FloatValue();
            BackgroundColor = new ColorSelector();
            BubbleColor = new ColorSelector();
        }

        public Guid ID { get; set; }
        public Integer2 Resolution { get; set; }
        public FloatValue Speed { get; set; }
        public FloatValue Frequency { get; set; }
        public FloatValue Contrast { get; set; }
        public ColorSelector BackgroundColor { get; set; }
        public ColorSelector BubbleColor { get; set; }
    }
}
