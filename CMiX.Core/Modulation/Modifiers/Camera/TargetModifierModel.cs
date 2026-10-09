// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation.Modifiers
{
    public record TargetModifierModel : IControlModel, IPrefabModel, IModifierModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public bool IsExpanded { get; set; } = true;
        public ModulatableValueModel<float> X { get; set; } = ModulatableValueModel<float>.Of("X", 0f);
        public ModulatableValueModel<float> Y { get; set; } = ModulatableValueModel<float>.Of("Y", 0f);
        public ModulatableValueModel<float> Z { get; set; } = ModulatableValueModel<float>.Of("Z", 0f);
        public PrefabManagerModel ModulatorManager { get; set; } = new();
    }
}
