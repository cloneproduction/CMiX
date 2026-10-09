// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public record ShiftRGBModel : IPrefabModel, ITextureFilterModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public BlendModel Blend { get; set; } = new();
        public ModulatableValueModel<float> Direction { get; set; } = ModulatableValueModel<float>.Of("Direction", 0.25f);
        public ModulatableValueModel<float> Shift { get; set; } = ModulatableValueModel<float>.Of("Shift", 0.2f);
        public ModulatableValueModel<float> Hue { get; set; } = ModulatableValueModel<float>.Of("Hue", 0.0f);
        public PrefabManagerModel ModulatorManager { get; set; } = new();
    }
}
