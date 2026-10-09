// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;

namespace CMiX.Core.Modifiers
{
    public record ModifierModeSelectorModel : IControlModel
    {
        public ModifierModeSelectorModel()
        {

        }

        public ModifierModeSelectorModel(ModifierMode modifierMode, int count)
        {
            Mode = new GenericValueModel<ModifierMode>(modifierMode);
            Count = new ModulatableValueModel<int> { Value = new GenericValueModel<int>(count) };
        }
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<ModifierMode> Mode { get; set; } = new(ModifierMode.PerInstance);
        public ModulatableValueModel<int> Count { get; set; } = new();
    }
}
