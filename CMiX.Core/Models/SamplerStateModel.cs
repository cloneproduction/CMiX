// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class SamplerStateModel : IModel
    {
        public SamplerStateModel()
        {
            ID = Guid.NewGuid();
            AddressU = TextureAddressMode.Clamp.ToString();
            AddressV = TextureAddressMode.Clamp.ToString();
            ColorSelectorModel = new ColorSelectorModel();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public ColorSelectorModel ColorSelectorModel { get; set; }
        public string AddressU { get; set; }
        public string AddressV { get; set; }
    }
}
