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
            ID = Guid.NewGuid();
            Radius = new GenericValueModel<float>(1.0f);
            Brightness = new GenericValueModel<float>(1.0f);
            Visible = new GenericValueModel<bool>(true);
            Control = new GenericValueModel<float>(1.0f);
        }

        public Guid ID { get; set; }
        public GenericValueModel<bool> Visible { get; set; }
        public GenericValueModel<float> Radius { get; set; }
        public GenericValueModel<float> Brightness { get; set; }
        public GenericValueModel<float> Control { get; set; }
    }
}
