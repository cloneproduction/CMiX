// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;

namespace CMiX.Core.Texturing.Filters
{
    public class SetAlphaModel : IModifierModel
    {
        public SetAlphaModel()
        {
            ID = Guid.NewGuid();
            Visible = new GenericValueModel<bool>(true);
            Invert = new GenericValueModel<bool>(true);
            KeepOriginalAlpha = new GenericValueModel<bool>(true);
            AlphaChannel = new GenericValueModel<AlphaChannel>(Filters.AlphaChannel.Lightness);
            Control = new GenericValueModel<float>(1.0f);
        }

        public Guid ID { get; set; }

        public GenericValueModel<bool> Invert { get; set; }
        public GenericValueModel<bool> KeepOriginalAlpha { get; set; }
        public GenericValueModel<AlphaChannel> AlphaChannel { get; set; }
        public GenericValueModel<float> Control { get; set; }
        public GenericValueModel<bool> Visible { get; set; }
    }
}
