// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation.Modifiers
{
    public record RandomModifierModel : IControlModel, IPrefabModel, IModifierModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public bool IsExpanded { get; set; } = true;
        public ModulatableValueModel<float> Seed { get; set; } = ModulatableValueModel<float>.Of("Seed", 0f);
        public ModulatableValueModel<float> Center { get; set; } = ModulatableValueModel<float>.Of("Center", 0f);
        public ModulatableValueModel<float> Width { get; set; } = ModulatableValueModel<float>.Of("Width", 1.0f);
        public PrefabManagerModel ModulatorManager { get; set; } = new();
        public ModifierModeSelectorModel ModifierModeSelector { get; set; } = new(ModifierMode.PerInstance, 1);
    }
}
