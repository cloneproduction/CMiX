// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Layering.Modifiers
{
    public class SelectRandomEntityModel : IPrefabModel
    {
        public SelectRandomEntityModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            BeatModifierManager = new PrefabManagerModel();
            EntityType = new GenericValueModel<EntityType>(Modifiers.EntityType.Entity);
            Control = new GenericValueModel<float>(1.0f);
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public PrefabManagerModel BeatModifierManager { get; set; }
        public GenericValueModel<EntityType> EntityType { get; set; }
        public GenericValueModel<float> Control { get; set; }
    }
}
