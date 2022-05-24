// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class TriColorModel : ITextureFilterModel
    {
        public TriColorModel()
        {
            ID = Guid.NewGuid();
            Enabled = true;
            Name = TextureFilterName.TriColor;

            Visible = new ToggleButtonModel(true);
            Control = new SliderModel(1.0f);
            ColorA = new ColorSelectorModel();
            ColorB = new ColorSelectorModel();
            ColorC = new ColorSelectorModel();
            Smooth = new SliderModel();
            Center = new SliderModel();
            SingleChannel = new ToggleButtonModel();
            ClampColor = new ToggleButtonModel();
        }

        public TextureFilterName Name { get; set; }
        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public ToggleButtonModel Visible { get; set; }

        public SliderModel Control { get; set; }
        public ColorSelectorModel ColorA { get; set; }
        public ColorSelectorModel ColorB { get; set; }
        public ColorSelectorModel ColorC { get; set; }
        public SliderModel Smooth { get; set; }
        public SliderModel Center { get; set; }
        public ToggleButtonModel SingleChannel { get; set; }
        public ToggleButtonModel ClampColor { get; set; }
    }
}
