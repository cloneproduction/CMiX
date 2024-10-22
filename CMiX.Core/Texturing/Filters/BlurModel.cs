// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Texturing.Filters
{
    public class BlurModel : IPrefabModel
    {
        public BlurModel()
        {
            ID = Guid.NewGuid();
            Strength = new GenericValueModel<float>(0.0f);
            PrefabService = new PrefabServiceModel();
        }

        public Guid ID { get; set; }
        public GenericValueModel<float> Strength { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
    }
}
