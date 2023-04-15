// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Sources
{
    public class BubbleNoise : ObservableObject, ITextureSource
    {
        public BubbleNoise(BubbleNoiseModel bubbleNoiseModel)
        {
            ID = bubbleNoiseModel.ID;
            Resolution = new Integer2(bubbleNoiseModel.Resolution);
            Speed = new FloatValue(bubbleNoiseModel.Speed);
            Frequency = new FloatValue(bubbleNoiseModel.Frequency);
            Contrast = new FloatValue(bubbleNoiseModel.Contrast);
            BackgroundColor = new ColorSelector(bubbleNoiseModel.BackgroundColor);
            BubbleColor = new ColorSelector(bubbleNoiseModel.BubbleColor);
        }

        public Guid ID { get; set; }
        public Integer2 Resolution { get; set; }
        public FloatValue Speed { get; set; }
        public FloatValue Frequency { get; set; }
        public FloatValue Contrast { get; set; }
        public ColorSelector BackgroundColor { get; set; }
        public ColorSelector BubbleColor { get; set; }

        public void Dispose()
        {

        }
    }
}
