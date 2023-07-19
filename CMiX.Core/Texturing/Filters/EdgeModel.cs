// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;

namespace CMiX.Core.Texturing.Filters
{
    public class EdgeModel : IModifierModel
    {
        public EdgeModel()
        {
            Radius = new FloatValueModel(1.0f);
            Brightness = new FloatValueModel(1.0f);
            Visible = new BooleanValueModel(true);
            Control = new FloatValueModel(1.0f);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public BooleanValueModel Visible { get; set; }
        public FloatValueModel Radius { get; set; }
        public FloatValueModel Brightness { get; set; }
        public FloatValueModel Control { get; set; }
    }
}
