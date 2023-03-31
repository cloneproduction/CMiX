// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.Texturing.Sampling;
using CMiX.Core.Presentations.Transform;

namespace CMiX.Core.Texturing.Filters
{
    public class TransformTextureModel : ITextureFilterModel
    {
        public TransformTextureModel()
        {
            ID = Guid.NewGuid();
            Name = TextureFilterName.TransformTexture;

            Visible = new BooleanValueModel(true);
            SamplerStateModel = new SamplerStateModel();
            Transform2D = new Transform2DModel();
        }

        public TextureFilterName Name { get; set; }
        public Guid ID { get; set; }
        public BooleanValueModel Visible { get; set; }
        public SamplerStateModel SamplerStateModel { get; set; }
        public Transform2DModel Transform2D { get; set; }
    }
}
