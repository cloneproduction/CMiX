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

            Visible = new BooleanValueModel(true);
            Control = new FloatValueModel(1.0f);
            ColorA = new ColorSelectorModel();
            ColorB = new ColorSelectorModel();
            ColorC = new ColorSelectorModel();
            Smooth = new FloatValueModel();
            Center = new FloatValueModel();
            SingleChannel = new BooleanValueModel();
            ClampColor = new BooleanValueModel();
        }

        public TextureFilterName Name { get; set; }
        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public BooleanValueModel Visible { get; set; }

        public FloatValueModel Control { get; set; }
        public ColorSelectorModel ColorA { get; set; }
        public ColorSelectorModel ColorB { get; set; }
        public ColorSelectorModel ColorC { get; set; }
        public FloatValueModel Smooth { get; set; }
        public FloatValueModel Center { get; set; }
        public BooleanValueModel SingleChannel { get; set; }
        public BooleanValueModel ClampColor { get; set; }
    }
}
