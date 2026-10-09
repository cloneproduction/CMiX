// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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
