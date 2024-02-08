// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;

namespace CMiX.Core.Texturing.Filters
{
    public class EchoModel : IModifierModel
    {
        public EchoModel()
        {
            ID = Guid.NewGuid();
            Visible = new GenericValueModel<bool>(true);
            Factor = new GenericValueModel<float>(0.9f);
        }

        public GenericValueModel<bool> Visible { get; set; }
        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public GenericValueModel<float> Factor { get; set; }
    }
}
