// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels.Modifiers;

namespace CMiX.Core.Models
{
    public class TextureModel : IModel
    {
        public TextureModel()
        {
            this.ID = Guid.NewGuid();

            TextureSelectorModel = new ImageSelectorModel();
            InverterModel = new InvertModel();
            Brightness = new SliderModel();
            Contrast = new SliderModel();
            Hue = new SliderModel();
            Saturation = new SliderModel();
            Luminosity = new SliderModel();
            Keying = new SliderModel();

            TranslateU = new SliderModel();
            TranslateV = new SliderModel();

            ScaleU = new SliderModel();
            ScaleU.Amount = 1.0f;

            ScaleV = new SliderModel();
            ScaleV.Amount = 1.0f;

            Rotate = new SliderModel();
            VideoPlayerModel = new VideoPlayerModel();
            TextureFilterManagerModel = new TextureFilterManagerModel();
            ModifierManagerModel = new ModifierManagerModel();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public TextureFilterManagerModel TextureFilterManagerModel { get; set; }
        public ModifierManagerModel ModifierManagerModel { get; set; }
        public VideoPlayerModel VideoPlayerModel { get; set; }
        public ImageSelectorModel TextureSelectorModel { get; set; }
        public InvertModel InverterModel { get; set; }

        public SliderModel Brightness { get; set; }
        public SliderModel Contrast { get; set; }
        public SliderModel Hue { get; set; }
        public SliderModel Saturation { get; set; }
        public SliderModel Luminosity { get; set; }
        public SliderModel Keying { get; set; }

        public SliderModel TranslateU { get; set; }
        public SliderModel TranslateV { get; set; }

        public SliderModel ScaleU { get; set; }
        public SliderModel ScaleV { get; set; }

        public SliderModel Rotate { get; set; }

    }
}
