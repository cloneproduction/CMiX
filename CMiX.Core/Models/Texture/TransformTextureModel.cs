// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class TransformTextureModel : ITextureFilterModel
    {
        public TransformTextureModel()
        {
            ID = Guid.NewGuid();
            Enabled = true;
            Name = TextureFilterName.TransformTexture;

            Visible = new ToggleButtonModel(true);
            SamplerStateModel = new SamplerStateModel();
            TranslateXModel = new SliderModel();
            TranslateYModel = new SliderModel();
            ScaleXModel = new SliderModel(1.0f);
            ScaleYModel = new SliderModel(1.0f);
            RotateModel = new SliderModel();
            Control = new SliderModel();
        }

        public TextureFilterName Name { get; set; }
        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public ToggleButtonModel Visible { get; set; }
        public SamplerStateModel SamplerStateModel { get; set; }
        public SliderModel TranslateXModel { get; set; }
        public SliderModel TranslateYModel { get; set; }
        public SliderModel ScaleXModel { get; set; }
        public SliderModel ScaleYModel { get; set; }
        public SliderModel RotateModel { get; set; }
        public SliderModel Control { get; set; }
    }
}
