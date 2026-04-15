// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering;

namespace CMiX.Core.Compositing
{
    public record CompositionModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; init; } = new();
        //public MasterBeatModel MasterBeat { get; init; } = new();
        public OutputSettingsModel OutputSettings { get; init; } = new();
        public PrefabManagerModel TextureModifierManager { get; init; } = new();
        public PrefabManagerModel LayerManager { get; init; } = new();
        public PrefabManagerModel ModifierManager { get; init; } = new();
    }
}
