// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.Texturing.Sampling
{
    public class SamplerState : ObservableRecipient, IControl
    {
        public SamplerState(SamplerStateModel samplerStateModel, CompositionService compositionService)
        {
            ID = samplerStateModel.ID;
            BorderColor = new ColorSelector(samplerStateModel.BorderColor);
            AddressU = new GenericValue<TextureAddressMode>(samplerStateModel.AddressU);
            AddressV = new GenericValue<TextureAddressMode>(samplerStateModel.AddressV);
            IsActive = true;
        }

        public Guid ID { get; set; }
        public ColorSelector BorderColor { get; set; }
        public GenericValue<TextureAddressMode> AddressU { get; set; }
        public GenericValue<TextureAddressMode> AddressV { get; set; }
    }
}
