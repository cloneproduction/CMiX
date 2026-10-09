// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public record EdgeModel : IPrefabModel, ITextureFilterModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public BlendModel Blend { get; set; } = new();
        public ModulatableValueModel<float> Radius { get; set; } = ModulatableValueModel<float>.Of("Radius", 1.0f);
        public ModulatableValueModel<float> Brightness { get; set; } = ModulatableValueModel<float>.Of("Brightness", 1.0f);
        public PrefabManagerModel ModulatorManager { get; set; } = new();
    }
}
