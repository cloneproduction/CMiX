// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Texturing.Filters
{
    public class InvertModel : IPrefabModel
    {
        public InvertModel()
        {
            ID = Guid.NewGuid();
            Factor = new GenericValueModel<float>(1.0f);
            PrefabService = new PrefabServiceModel();
            InvertChannelSelector = new GenericValueModel<InvertChannel>(InvertChannel.Value);
            InvertAlpha = new GenericValueModel<bool>(false);
            Control = new GenericValueModel<float>(1.0f);
        }

        public Guid ID { get; set; }
        public GenericValueModel<float> Factor { get; set; }
        public GenericValueModel<InvertChannel> InvertChannelSelector { get; set; }
        public GenericValueModel<bool> InvertAlpha { get; internal set; }
        public GenericValueModel<float> Control { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
    }
}
