// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Layering.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation.Modifiers
{
    public record RenderSequenceEntityModifierModel : IControlModel, IPrefabModel, IModifierModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public bool IsExpanded { get; set; } = true;
        public ModulatableValueModel<float> Control { get; set; } = ModulatableValueModel<float>.Of("Control", 1f);
        public PrefabManagerModel ModulatorManager { get; set; } = new();
        public GenericValueModel<EntityType> EntityType { get; set; } = new();
    }
}
