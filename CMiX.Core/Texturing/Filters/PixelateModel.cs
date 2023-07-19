// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;

namespace CMiX.Core.Texturing.Filters
{
    public class PixelateModel : IModifierModel
    {
        public PixelateModel()
        {
            Visible = new BooleanValueModel(true);
            Control = new FloatValueModel(1.0f);
            Factor = new Vector2Model(0.5f, 0.5f);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public BooleanValueModel Visible { get; set; }
        public FloatValueModel Control { get; set; }
        public Vector2Model Factor { get; set; }
    }
}
