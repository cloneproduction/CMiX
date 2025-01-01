// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Texturing.Filters
{
    public class KuwaharaModel : IPrefabModel
    {
        public KuwaharaModel()
        {
            ID = Guid.NewGuid();
            Radius = new GenericValueModel<float>(1.0f);
            Type = new GenericValueModel<KuwaharaType>(KuwaharaType.Standard);
            Control = new GenericValueModel<float>(1.0f);
            PrefabService = new PrefabServiceModel();
        }

        public Guid ID { get; set; }
        public GenericValueModel<float> Radius { get; set; }
        public GenericValueModel<KuwaharaType> Type { get; set; }
        public GenericValueModel<float> Control { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
    }
}
