// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing
{
    public class SamplerState : ObservableObject, IControl
    {
        public SamplerState(GenericValue<string> borderColor, 
                            GenericValue<TextureAddressMode> addresseU, 
                            GenericValue<TextureAddressMode> addresseV)
        {
            BorderColor = borderColor;
            AddressU = addresseU;
            AddressV = addresseV;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<string> BorderColor { get; set; }
        public GenericValue<TextureAddressMode> AddressU { get; set; }
        public GenericValue<TextureAddressMode> AddressV { get; set; }

        public IControlModel ToModel() => new SamplerStateModel
        {
            ID = ID,
            BorderColor = (GenericValueModel<string>)BorderColor.ToModel(),
            AddressU = (GenericValueModel<TextureAddressMode>)AddressU.ToModel(),
            AddressV = (GenericValueModel<TextureAddressMode>)AddressV.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (SamplerStateModel)model;
            ID = m.ID;
            BorderColor.FromModel(m.BorderColor);
            AddressU.FromModel(m.AddressU);
            AddressV.FromModel(m.AddressV);
        }
    }
}
