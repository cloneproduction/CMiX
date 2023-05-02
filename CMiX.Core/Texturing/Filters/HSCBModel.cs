// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Texturing.Filters
{
    public class HSCBModel : ITextureFilterModel
    {
        public HSCBModel()
        {
            ID = Guid.NewGuid();
            Name = TextureFilterName.HSCB;

            Visible = new BooleanValueModel(true);
            Hue = new FloatValueModel();
            Saturation = new FloatValueModel(1.0f);
            Contrast = new FloatValueModel();
            Brightness = new FloatValueModel();
            Control = new FloatValueModel();
        }

        public Guid ID { get; set; }
        public BooleanValueModel Visible { get; set; }
        public FloatValueModel Hue { get; set; }
        public FloatValueModel Saturation { get; set; }
        public FloatValueModel Contrast { get; set; }
        public FloatValueModel Brightness { get; set; }
        public FloatValueModel Control { get; set; }
        public TextureFilterName Name { get; set; }
    }
}
