// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models
{
    public class TextureModel : IModel
    {
        public TextureModel()
        {
            this.ID = Guid.NewGuid();

            TextureSelectorModel = new TextureSelectorModel();
            InverterModel = new InverterModel();
            Brightness = new SliderModel();
            Contrast = new SliderModel();
            Hue = new SliderModel();
            Saturation = new SliderModel();
            Luminosity = new SliderModel();
            Keying = new SliderModel();
            Pan = new SliderModel();
            Tilt = new SliderModel();
            Scale = new SliderModel();
            Rotate = new SliderModel();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public TextureSelectorModel TextureSelectorModel { get; set; }
        public InverterModel InverterModel { get; set; }

        public SliderModel Brightness { get; set; }
        public SliderModel Contrast { get; set; }
        public SliderModel Hue { get; set; }
        public SliderModel Saturation { get; set; }
        public SliderModel Luminosity { get; set; }
        public SliderModel Keying { get; set; }
        public SliderModel Pan { get; set; }
        public SliderModel Tilt { get; set; }
        public SliderModel Scale { get; set; }
        public SliderModel Rotate { get; set; }

    }
}
