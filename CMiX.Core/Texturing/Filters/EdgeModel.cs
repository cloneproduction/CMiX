// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Texturing.Filters
{
    public class EdgeModel : IPrefabModel
    {
        public EdgeModel()
        {
            ID = Guid.NewGuid();
            Radius = new GenericValueModel<float>(1.0f);
            Brightness = new GenericValueModel<float>(1.0f);
            Control = new GenericValueModel<float>(1.0f);
            PrefabService = new PrefabServiceModel();
        }

        public Guid ID { get; set; }
        public GenericValueModel<float> Radius { get; set; }
        public GenericValueModel<float> Brightness { get; set; }
        public GenericValueModel<float> Control { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
    }
}
