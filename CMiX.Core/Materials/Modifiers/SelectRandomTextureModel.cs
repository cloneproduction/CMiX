// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Materials.Modifiers
{
    public record SelectRandomTextureModel : IPrefabModel
    {
        public Guid ID { get; init; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; init; } = new();
        public PrefabManagerModel BeatModifierManager { get; init; } = new();
        public GenericValueModel<TextureFrom> TextureFrom { get; init; } = new(Modifiers.TextureFrom.Diffuse);
    }
}
