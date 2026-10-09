// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public record SetAlphaModel : IPrefabModel, ITextureFilterModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<bool> Invert { get; set; } = new(true);
        public GenericValueModel<bool> KeepOriginalAlpha { get; set; } = new(true);
        public GenericValueModel<AlphaChannel> AlphaChannel { get; set; } = new(Filters.AlphaChannel.Lightness);
        public BlendModel Blend { get; set; } = new();
        public PrefabManagerModel ModulatorManager { get; set; } = new();
    }
}
