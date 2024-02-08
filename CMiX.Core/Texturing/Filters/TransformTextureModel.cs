// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Transformation;

namespace CMiX.Core.Texturing.Filters
{
    public class TransformTextureModel : IModifierModel
    {
        public TransformTextureModel()
        {
            Visible = new GenericValueModel<bool>(true);
            SamplerState = new SamplerStateModel();
            Transform2D = new Transform2DModel();
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<bool> Visible { get; set; }
        public SamplerStateModel SamplerState { get; set; }
        public Transform2DModel Transform2D { get; set; }
    }
}
