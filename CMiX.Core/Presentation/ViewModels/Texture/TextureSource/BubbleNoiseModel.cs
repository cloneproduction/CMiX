// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;

namespace CMiX.Core.Presentation.ViewModels
{
    public class BubbleNoiseModel : IModel
    {
        public BubbleNoiseModel()
        {
            ID = Guid.NewGuid();
            ResolutionX = new CounterModel(512);
            ResolutionY = new CounterModel(512);
            Speed = new SliderModel(0.0f);
            Frequency = new SliderModel(3.5f);
            Contrast = new SliderModel(0.15f);
            BackgroundColor = new ColorSelectorModel("#FF000000");
            BubbleColor = new ColorSelectorModel("#FFFFFFFF");
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }

        public CounterModel ResolutionX { get; set; }
        public CounterModel ResolutionY { get; set; }
        public SliderModel Speed { get; set; }
        public SliderModel Frequency { get; set; }
        public SliderModel Contrast { get; set; }
        public ColorSelectorModel BackgroundColor { get; set; }
        public ColorSelectorModel BubbleColor { get; set; }
    }
}
