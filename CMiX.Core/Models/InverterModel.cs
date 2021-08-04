// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class InverterModel : IModel
    {
        public InverterModel()
        {
            ID = Guid.NewGuid();
            Invert = new SliderModel();
            InvertMode = new ComboBoxModel<TextureInvertMode>();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public SliderModel Invert { get; set; }
        public ComboBoxModel<TextureInvertMode> InvertMode { get; set; }
    }
}
