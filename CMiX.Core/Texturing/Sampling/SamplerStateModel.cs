// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Texturing
{
    public record SamplerStateModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<string> BorderColor { get; set; } = new("#FFFF00FF");
        public GenericValueModel<TextureAddressMode> AddressU { get; set; } = new(TextureAddressMode.Mirror);
        public GenericValueModel<TextureAddressMode> AddressV { get; set; } = new(TextureAddressMode.Mirror);
    }
}
