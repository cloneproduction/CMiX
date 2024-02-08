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
            Visible = new GenericValueModel<bool>(true);
            Strength = new GenericValueModel<float>();
        }

        public Guid ID { get; set; }
        public GenericValueModel<float> Strength { get; set; }
        public GenericValueModel<bool> Visible { get; set; }
    }
}
