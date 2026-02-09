// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Animations
{
    public record BeatModifierModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public EasingModel Easing { get; set; } = new();
        public GenericValueModel<int> BeatIndex { get; set; } = new(0);
        public GenericValueModel<float> ChanceToHit { get; set; } = new(100f);
        public PrefabServiceModel PrefabService { get; set; } = new();
    }
}
