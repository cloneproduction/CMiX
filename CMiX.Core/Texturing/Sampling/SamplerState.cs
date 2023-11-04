// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Sampling
{
    public class SamplerState : ObservableRecipient, IControl
    {
        public SamplerState(ColorValue borderColor, GenericValue<TextureAddressMode> addresseU, GenericValue<TextureAddressMode> addresseV)
        {
            BorderColor = borderColor;// new ColorValue();
            AddressU = addresseU;// new GenericValue<TextureAddressMode>();
            AddressV = addresseV;// new GenericValue<TextureAddressMode>();
            IsActive = true;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public ColorValue BorderColor { get; set; }
        public GenericValue<TextureAddressMode> AddressU { get; set; }
        public GenericValue<TextureAddressMode> AddressV { get; set; }
    }
}
