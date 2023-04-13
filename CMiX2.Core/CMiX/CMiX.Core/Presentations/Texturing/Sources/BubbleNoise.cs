// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.Texturing.Sources
{
    public class BubbleNoise : ObservableObject, ITextureSource
    {
        public BubbleNoise(BubbleNoiseModel bubbleNoiseModel, CompositionService compositionService)
        {
            ID = bubbleNoiseModel.ID;
            CompositionService = compositionService;
            Resolution = new Integer2(bubbleNoiseModel.Resolution, compositionService);
            Speed = new FloatValue(bubbleNoiseModel.Speed, compositionService);
            Frequency = new FloatValue(bubbleNoiseModel.Frequency, compositionService);
            Contrast = new FloatValue(bubbleNoiseModel.Contrast, compositionService);
            BackgroundColor = new ColorSelector(bubbleNoiseModel.BackgroundColor, compositionService);
            BubbleColor = new ColorSelector(bubbleNoiseModel.BubbleColor, compositionService);
        }

        public Guid ID { get; set; }
        public CompositionService CompositionService { get; set; }
        public Integer2 Resolution { get; set; }
        public FloatValue Speed { get; set; }
        public FloatValue Frequency { get; set; }
        public FloatValue Contrast { get; set; }
        public ColorSelector BackgroundColor { get; set; }
        public ColorSelector BubbleColor { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
