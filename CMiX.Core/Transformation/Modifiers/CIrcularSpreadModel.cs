// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Transformation.Modifiers
{
    public record CircularSpreadModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public ModifierModeSelectorModel ModifierModeSelector { get; set; } = new();
        public Vector2Model Width { get; set; } = new(1.0f, 1.0f);
        public GenericValueModel<float> Phase { get; set; } = new(0.0f);
        public GenericValueModel<float> Factor { get; set; } = new(1.0f);
    }
}
