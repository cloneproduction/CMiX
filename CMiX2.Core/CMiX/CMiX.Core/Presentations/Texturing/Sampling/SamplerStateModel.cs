// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.ViewModels;

namespace CMiX.Core.Presentations.Texturing.Sampling
{
    public class SamplerStateModel : IModel
    {
        public SamplerStateModel()
        {
            ID = Guid.NewGuid();
            AddressU = new GenericValueModel<TextureAddressMode>(TextureAddressMode.Mirror);
            AddressV = new GenericValueModel<TextureAddressMode>(TextureAddressMode.Mirror);
            BorderColor = new ColorSelectorModel();
        }

        public Guid ID { get; set; }
        public ColorSelectorModel BorderColor { get; set; }
        public GenericValueModel<TextureAddressMode> AddressU { get; set; }
        public GenericValueModel<TextureAddressMode> AddressV { get; set; }
    }
}
