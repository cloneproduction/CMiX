// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation.Modifiers
{
    public record ColorModifierModel : IControlModel, IPrefabModel, IModifierModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public bool IsExpanded { get; set; } = true;
        public ModulatableValueModel<float> Hue { get; set; } = ModulatableValueModel<float>.Of("Hue", 0f);
        public ModulatableValueModel<float> Saturation { get; set; } = ModulatableValueModel<float>.Of("Saturation", 0f);
        public ModulatableValueModel<float> Value { get; set; } = ModulatableValueModel<float>.Of("Value", 1f);
        public ModulatableValueModel<float> Alpha { get; set; } = ModulatableValueModel<float>.Of("Alpha", 1f);
        public PrefabManagerModel ModulatorManager { get; set; } = new();
        public ModifierModeSelectorModel ModifierModeSelector { get; set; } = new(ModifierMode.PerInstance, 1);
        public GenericValueModel<ColorMode> ColorMode { get; set; } = new(Core.ColorMode.HSV);
    }
}
