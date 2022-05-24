// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class PixelateModel : ITextureFilterModel
    {
        public PixelateModel()
        {
            ID = Guid.NewGuid();
            Visible = new ToggleButtonModel(true);
            Control = new SliderModel(1.0f);
            FactorX = new SliderModel(0.5f);
            FactorY = new SliderModel(0.5f);

            Name = TextureFilterName.Pixelate;
        }

        public ToggleButtonModel Visible { get; set ; }
        public bool Enabled { get; set ; }
        public Guid ID { get; set; }
        public TextureFilterName Name { get; set; }
        public SliderModel Control { get; set; }
        public SliderModel FactorX { get; set; }
        public SliderModel FactorY { get; set; }
    }
}
