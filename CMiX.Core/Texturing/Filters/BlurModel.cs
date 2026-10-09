// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public record BlurModel : IPrefabModel, ITextureFilterModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public BlendModel Blend { get; set; } = new();
        public ModulatableValueModel<float> Strength { get; set; } = ModulatableValueModel<float>.Of("Strength", 0.5f);
        public PrefabManagerModel ModulatorManager { get; set; } = new();
    }
}
