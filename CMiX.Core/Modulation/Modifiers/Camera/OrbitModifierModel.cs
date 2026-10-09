// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation.Modifiers
{
    public record OrbitModifierModel : IControlModel, IPrefabModel, IModifierModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public bool IsExpanded { get; set; } = true;
        public ModulatableValueModel<float> Yaw { get; set; } = ModulatableValueModel<float>.Of("Yaw", 0f);
        public ModulatableValueModel<float> Pitch { get; set; } = ModulatableValueModel<float>.Of("Pitch", 0f);
        public PrefabManagerModel ModulatorManager { get; set; } = new();
    }
}
