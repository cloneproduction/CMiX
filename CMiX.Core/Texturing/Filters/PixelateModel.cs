// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Texturing.Filters
{
    public class PixelateModel : IPrefabModel
    {
        public PixelateModel()
        {
            PrefabService = new PrefabServiceModel();
            Control = new GenericValueModel<float>(1.0f);
            Factor = new Vector2Model(0.5f, 0.5f);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; }
        public GenericValueModel<float> Control { get; set; }
        public Vector2Model Factor { get; set; }

    }
}
