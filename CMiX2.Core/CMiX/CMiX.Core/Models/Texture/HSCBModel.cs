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
            Name = TextureFilterName.HSCB;

            Visible = new BooleanValueModel(true);
            HueModel = new FloatValueModel();
            SaturationModel = new FloatValueModel(1.0f);
            ConstrastModel = new FloatValueModel();
            BrightnessModel = new FloatValueModel();
            Control = new FloatValueModel();
        }

        public Guid ID { get; set; }

        public BooleanValueModel Visible { get; set; }
        public FloatValueModel HueModel { get; set; }
        public FloatValueModel SaturationModel { get; set; }
        public FloatValueModel ConstrastModel { get; set; }
        public FloatValueModel BrightnessModel { get; set; }
        public FloatValueModel Control { get; set; }
        public TextureFilterName Name { get; set; }
    }
}
