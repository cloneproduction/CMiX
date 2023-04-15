// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Transformation;
using CMiX.Core.Texturing.Sampling;

namespace CMiX.Core.Texturing.Filters
{
    public class TransformTextureModel : ITextureFilterModel
    {
        public TransformTextureModel()
        {
            ID = Guid.NewGuid();
            Name = TextureFilterName.TransformTexture;
            Visible = new BooleanValueModel(true);
            SamplerState = new SamplerStateModel();
            Transform2D = new Transform2DModel();
        }

        public Guid ID { get; set; }
        public TextureFilterName Name { get; set; }
        public BooleanValueModel Visible { get; set; }
        public SamplerStateModel SamplerState { get; set; }
        public Transform2DModel Transform2D { get; set; }
    }
}
