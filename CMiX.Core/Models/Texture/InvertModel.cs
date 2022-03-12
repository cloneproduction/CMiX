// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class InvertModel : ITextureFilterModel
    {
        public InvertModel()
        {
            ID = Guid.NewGuid();

            Name = TextureFilterName.Invert;
            Factor = new SliderModel();
            InvertMode = new ComboBoxModel<TextureInvertMode>();
            Visible = new ToggleButtonModel(true);
            Enabled = true;
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public SliderModel Factor { get; set; }
        public ToggleButtonModel Visible { get; set; }
        public ComboBoxModel<TextureInvertMode> InvertMode { get; set; }
        public TextureFilterName Name { get; set; }
    }
}
