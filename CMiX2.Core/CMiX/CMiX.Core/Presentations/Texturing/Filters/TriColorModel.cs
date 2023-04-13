// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.BaseControl;

namespace CMiX.Core.Texturing.Filters
{
    public class TriColorModel : ITextureFilterModel
    {
        public TriColorModel()
        {
            ID = Guid.NewGuid();
            Name = TextureFilterName.TriColor;

            Visible = new BooleanValueModel(true);
            Control = new FloatValueModel(1.0f);
            ColorA = new ColorSelectorModel("#FFFF00FF");
            ColorB = new ColorSelectorModel("#FFFF00FF");
            ColorC = new ColorSelectorModel("#FFFF00FF");
            Smooth = new FloatValueModel();
            Center = new FloatValueModel();
            SingleChannel = new BooleanValueModel();
            ClampColor = new BooleanValueModel();
        }

        public TextureFilterName Name { get; set; }
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
