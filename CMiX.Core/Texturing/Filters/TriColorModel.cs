// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public record TriColorModel : IPrefabModel, ITextureFilterModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<string> ColorA { get; set; } = new("#FFFF00FF");
        public GenericValueModel<string> ColorB { get; set; } = new("#FFFF00FF");
        public GenericValueModel<string> ColorC { get; set; } = new("#FFFF00FF");
        public GenericValueModel<bool> SingleChannel { get; set; } = new(true);
        public GenericValueModel<bool> ClampColor { get; set; } = new(true);
        public BlendModel Blend { get; set; } = new();
        public ModulatableValueModel<float> Smooth { get; set; } = ModulatableValueModel<float>.Of("Smooth", 0.5f);
        public ModulatableValueModel<float> Center { get; set; } = ModulatableValueModel<float>.Of("Center", 0.5f);
        public PrefabManagerModel ModulatorManager { get; set; } = new();
    }
}
