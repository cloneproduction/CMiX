// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
