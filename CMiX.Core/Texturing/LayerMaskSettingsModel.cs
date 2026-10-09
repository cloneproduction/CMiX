// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;

namespace CMiX.Core.Texturing
{
    public record LayerMaskSettingsModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<bool> IsMask { get; init; } = new();
        public GenericValueModel<MaskChannel> MaskChannel { get; init; } = new();
        public GenericValueModel<MaskMode> MaskMode { get; init; } = new();
        public GenericValueModel<bool> Invert { get; init; } = new();
    }
}
