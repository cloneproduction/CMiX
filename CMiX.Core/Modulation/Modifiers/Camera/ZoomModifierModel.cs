// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation.Modifiers
{
    public record ZoomModifierModel : IControlModel, IPrefabModel, IModifierModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public bool IsExpanded { get; set; } = true;
        public ModulatableValueModel<float> Distance { get; set; } = ModulatableValueModel<float>.Of("Distance", 0f);
        public ModulatableValueModel<float> FOV { get; set; } = ModulatableValueModel<float>.Of("FOV", 0f);
        public PrefabManagerModel ModulatorManager { get; set; } = new();
    }
}
