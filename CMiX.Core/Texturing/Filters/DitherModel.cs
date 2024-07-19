// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Texturing.Filters
{
    public class DitherModel : IPrefabModel
    {
        public DitherModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            Control = new GenericValueModel<float>(1.0f);
            Threshold = new GenericValueModel<float>(1.0f);
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public GenericValueModel<float> Threshold { get; set; }
        public GenericValueModel<float> Control { get; set; }
    }
}
