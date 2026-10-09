// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation.Modifiers
{
    public record GridModifierModel : IControlModel, IPrefabModel, IModifierModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public bool IsExpanded { get; set; } = true;
        public ModulatableVector3Model Width { get; set; } = ModulatableVector3Model.Of(0f, 0f, 0f);
        public ModulatableVector3Model Phase { get; set; } = ModulatableVector3Model.Of(0f, 0f, 0f);
        public PrefabManagerModel ModulatorManager { get; set; } = new();
        public ModulatableValueModel<int> CountX { get; set; } = new() { Value = new GenericValueModel<int>(1) };
        public ModulatableValueModel<int> CountY { get; set; } = new() { Value = new GenericValueModel<int>(1) };
        public ModulatableValueModel<int> CountZ { get; set; } = new() { Value = new GenericValueModel<int>(1) };
    }
}
