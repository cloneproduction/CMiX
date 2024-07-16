// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public class DisplaceModel : IPrefabModel
    {
        public DisplaceModel()
        {
            ID = Guid.NewGuid();

            Control = new GenericValueModel<float>(1.0f);
            PrefabService = new PrefabServiceModel();
            TextureSelector = new PrefabManagerModel();
            Offset = new Vector2Model(0.5f, 0.5f);
            OffsetScale = new Vector2Model(0.1f, 0.1f);
        }

        public Guid ID { get; set; }
        public PrefabManagerModel TextureSelector { get; set; }
        public Vector2Model Offset { get; set; }
        public Vector2Model OffsetScale { get; set; }
        public GenericValueModel<float> Control { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
    }
}
