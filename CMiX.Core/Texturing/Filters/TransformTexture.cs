// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Transformation;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class TransformTexture : TextureFilterBase
    {
        public TransformTexture(PrefabService prefabService,
                                SamplerState samplerState,
                                Transform2D transform2D,
                                GenericValue<float> control,
                                Blend blend)
            : base(prefabService, control, blend)
        {
            SamplerState = samplerState;
            Transform2D = transform2D;
        }

        public SamplerState SamplerState { get; set; }
        public Transform2D Transform2D { get; set; }

        public override IControlModel ToModel()
        {
            var model = new TransformTextureModel
            {
                SamplerState = (SamplerStateModel)SamplerState.ToModel(),
                Transform2D = (Transform2DModel)Transform2D.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (TransformTextureModel)model;
            LoadBaseModel(m);
            SamplerState.FromModel(m.SamplerState);
            Transform2D.FromModel(m.Transform2D);
        }
    }
}
