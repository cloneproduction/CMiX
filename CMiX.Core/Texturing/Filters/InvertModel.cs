// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;

namespace CMiX.Core.Texturing.Filters
{
    public class InvertModel : IModifierModel
    {
        public InvertModel()
        {
            ID = Guid.NewGuid();
            Factor = new FloatValueModel(1.0f);
            InvertChannelSelector = new GenericValueModel<InvertChannel>(InvertChannel.Value);
            InvertAlpha = new BooleanValueModel();
            Visible = new BooleanValueModel(true);
            Control = new FloatValueModel();
        }

        public Guid ID { get; set; }
        public FloatValueModel Factor { get; set; }
        public BooleanValueModel Visible { get; set; }
        public GenericValueModel<InvertChannel> InvertChannelSelector { get; set; }
        public BooleanValueModel InvertAlpha { get; internal set; }
        public FloatValueModel Control { get; set; }
    }
}
