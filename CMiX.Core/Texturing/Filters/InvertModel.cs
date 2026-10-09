// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public record InvertModel : IPrefabModel, ITextureFilterModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<InvertChannel> InvertChannelSelector { get; set; } = new(InvertChannel.Value);
        public GenericValueModel<bool> InvertAlpha { get; set; } = new(false);
        public BlendModel Blend { get; set; } = new();
        public ModulatableValueModel<float> Factor { get; set; } = ModulatableValueModel<float>.Of("Factor", 1.0f);
        public PrefabManagerModel ModulatorManager { get; set; } = new();
    }
}
