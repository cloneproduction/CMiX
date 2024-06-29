// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Layering.Modifiers
{
    public class SelectRandomEntityModel : IControlModel, IPrefabModel
    {
        public SelectRandomEntityModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            Easing = new EasingModel();
            BeatModifier = new BeatModifierModel();
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public EasingModel Easing { get; set; }
        public BeatModifierModel BeatModifier { get; set; }
    }
}
