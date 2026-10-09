// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;

namespace CMiX.Core.Texturing
{
    public record SamplerStateModel : IControlModel
    {
        public Guid ID { get; init; } = Guid.NewGuid();
        public GenericValueModel<string> BorderColor { get; init; } = new("#FFFF00FF");
        public GenericValueModel<TextureAddressMode> AddressU { get; init; } = new(TextureAddressMode.Mirror);
        public GenericValueModel<TextureAddressMode> AddressV { get; init; } = new(TextureAddressMode.Mirror);
    }
}
