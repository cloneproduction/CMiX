// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Materials.Modifiers
{
    public record SelectRandomTextureModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public PrefabManagerModel BeatModifierManager { get; set; } = new();
        public GenericValueModel<TextureFrom> TextureFrom { get; set; } = new(Modifiers.TextureFrom.Diffuse);
    }
}
