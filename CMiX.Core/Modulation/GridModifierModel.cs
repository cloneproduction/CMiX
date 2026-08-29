// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation
{
    public record GridModifierModel : IControlModel, IPrefabModel, IModifierModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public bool IsExpanded { get; set; } = true;
        public List<ModulatableModel> Channels { get; set; } = new();
        public PrefabManagerModel ModulatorManager { get; set; } = new();
        public ModifierModeSelectorModel ModifierModeSelector { get; set; } = new(ModifierMode.ToSpread, 1);
        public Integer3Model Count { get; set; } = new(1, 1, 1);
    }
}
