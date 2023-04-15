// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Texturing.Filters
{
    public class PixelateModel : ITextureFilterModel
    {
        public PixelateModel()
        {
            ID = Guid.NewGuid();
            Visible = new BooleanValueModel(true);
            Control = new FloatValueModel(1.0f);
            Factor = new Vector2Model(0.5f, 0.5f);
            Name = TextureFilterName.Pixelate;
        }

        public BooleanValueModel Visible { get; set; }
        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public TextureFilterName Name { get; set; }
        public FloatValueModel Control { get; set; }
        public Vector2Model Factor { get; set; }
    }
}
