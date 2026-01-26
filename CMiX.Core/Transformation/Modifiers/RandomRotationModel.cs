// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Transformation.Modifiers
{
    public record RandomRotationModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public Vector3Model Rotation { get; set; } = new();
        public ModifierModeSelectorModel ModifierModeSelector { get; set; } = new();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public PrefabManagerModel BeatModifierManager { get; set; } = new();
    }
}
