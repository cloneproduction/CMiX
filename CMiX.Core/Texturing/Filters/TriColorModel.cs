// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;

namespace CMiX.Core.Texturing.Filters
{
    public class TriColorModel : IModifierModel
    {
        public TriColorModel()
        {
            ID = Guid.NewGuid();

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
