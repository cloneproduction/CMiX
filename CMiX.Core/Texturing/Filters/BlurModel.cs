// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;

namespace CMiX.Core.Texturing.Filters
{
    public class BlurModel : IModifierModel
    {
        public BlurModel()
        {
            ID = Guid.NewGuid();
            Visible = new BooleanValueModel(true);
            Strength = new FloatValueModel();
        }

        public Guid ID { get; set; }
        public FloatValueModel Strength { get; set; }
        public BooleanValueModel Visible { get; set; }
    }
}
