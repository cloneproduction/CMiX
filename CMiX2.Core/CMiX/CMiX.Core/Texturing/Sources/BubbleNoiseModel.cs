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
            Resolution = new Integer2Model(512, 512);
            Speed = new FloatValueModel(0.0f);
            Frequency = new FloatValueModel(3.5f);
            Contrast = new FloatValueModel(0.15f);
            BackgroundColor = new ColorSelectorModel("#FF000000");
            BubbleColor = new ColorSelectorModel("#FFFFFFFF");
        }

        public Guid ID { get; set; }

        public Integer2Model Resolution { get; set; }
        public FloatValueModel Speed { get; set; }
        public FloatValueModel Frequency { get; set; }
        public FloatValueModel Contrast { get; set; }
        public ColorSelectorModel BackgroundColor { get; set; }
        public ColorSelectorModel BubbleColor { get; set; }
    }
}
