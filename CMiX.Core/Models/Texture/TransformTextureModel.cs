// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models.BaseControls;
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

            Visible = new BooleanValueModel(true);
            SamplerStateModel = new SamplerStateModel();
            Translate = new Vector2Model();
            Scale = new Vector2Model();
            Rotate = new FloatValueModel();
            Control = new FloatValueModel();
        }

        public TextureFilterName Name { get; set; }
        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public BooleanValueModel Visible { get; set; }
        public SamplerStateModel SamplerStateModel { get; set; }
        public Vector2Model Translate { get; set; }
        public Vector2Model Scale { get; set; }
        public FloatValueModel Rotate { get; set; }
        public FloatValueModel Control { get; set; }
    }
}
