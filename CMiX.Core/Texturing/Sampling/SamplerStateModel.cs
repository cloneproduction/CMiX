// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Texturing
{
    public class SamplerStateModel : IControlModel
    {
        public SamplerStateModel()
        {
            ID = Guid.NewGuid();
            AddressU = new GenericValueModel<TextureAddressMode>(TextureAddressMode.Mirror);
            AddressV = new GenericValueModel<TextureAddressMode>(TextureAddressMode.Mirror);
            BorderColor = new GenericValueModel<string>("#FFFF00FF");
        }

        public Guid ID { get; set; }
        public GenericValueModel<string> BorderColor { get; set; }
        public GenericValueModel<TextureAddressMode> AddressU { get; set; }
        public GenericValueModel<TextureAddressMode> AddressV { get; set; }
    }
}
