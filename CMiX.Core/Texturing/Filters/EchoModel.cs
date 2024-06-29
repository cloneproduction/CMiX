// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Texturing.Filters
{
    public class EchoModel : IPrefabModel
    {
        public EchoModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            Factor = new GenericValueModel<float>(0.9f);
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public GenericValueModel<float> Factor { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
    }
}
