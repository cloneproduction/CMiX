// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class BlurModel : ITextureFilterModel
    {
        public BlurModel()
        {
            ID = Guid.NewGuid();

            Name = TextureFilterName.Blur;
            Visible = new ToggleButtonModel(true);
            Strength = new SliderModel();
            Enabled = true;
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public SliderModel Strength { get; set; }
        public ToggleButtonModel Visible { get; set; }
        public TextureFilterName Name { get; set; }
    }
}
