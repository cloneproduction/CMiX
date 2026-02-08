// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Layering.Modifiers
{
    public record RenderSequenceEntityModel : IPrefabModel
    {
        public Guid ID { get; init; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; init; } = new();
        public PrefabManagerModel BeatModifierManager { get; init; } = new();
        public GenericValueModel<EntityType> EntityType { get; init; } = new(Modifiers.EntityType.Entity);
        public GenericValueModel<float> Control { get; init; } = new(1.0f);
    }
}
