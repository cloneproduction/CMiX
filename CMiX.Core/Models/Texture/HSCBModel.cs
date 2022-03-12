// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class HSCBModel : ITextureFilterModel
    {
        public HSCBModel()
        {
            ID = Guid.NewGuid();
            Enabled = true;
            Name = TextureFilterName.HSCB;

            Visible = new ToggleButtonModel(true);
            HueModel = new SliderModel();
            SaturationModel = new SliderModel();
            ConstrastModel = new SliderModel();
            BrightnessModel = new SliderModel();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }

        public ToggleButtonModel Visible { get; set; }
        public SliderModel HueModel { get; set; }
        public SliderModel SaturationModel { get; set; }
        public SliderModel ConstrastModel { get; set; }
        public SliderModel BrightnessModel { get; set; }
        public TextureFilterName Name { get; set; }
    }
}
