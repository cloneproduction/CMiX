// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Texturing.Filters
{
    public class SetAlphaModel : IPrefabModel
    {
        public SetAlphaModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            Invert = new GenericValueModel<bool>(true);
            KeepOriginalAlpha = new GenericValueModel<bool>(true);
            AlphaChannel = new GenericValueModel<AlphaChannel>(Filters.AlphaChannel.Lightness);
            Control = new GenericValueModel<float>(1.0f);
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public GenericValueModel<bool> Invert { get; set; }
        public GenericValueModel<bool> KeepOriginalAlpha { get; set; }
        public GenericValueModel<AlphaChannel> AlphaChannel { get; set; }
        public GenericValueModel<float> Control { get; set; }
    }
}
