// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public partial class TransformTexture : TextureFilterBase
    {
        public TransformTexture(PrefabService prefabService,
                        SamplerState samplerState,
                        ModulatableVector2 location,
                        ModulatableVector2 scale,
                        ModulatableValue<float> rotation,
                        ModulatableValue<float> uniform,
                        Blend blend,
                        PrefabManager modulatorManager)
            : base(prefabService, blend, modulatorManager)
        {
            SamplerState = samplerState;
            Location = location;
            Scale = scale;

            Bindables = new List<ModulatableValue<float>>
            {
                rotation, uniform,
                location.X, location.Y,
                scale.X, scale.Y
            };

            rotation.Label = "Rotation";
            uniform.Label = "Uniform";

            rotation.SetDefault(0.0f);
            uniform.SetDefault(1.0f);
            location.X.SetDefault(0.0f);
            location.Y.SetDefault(0.0f);
            scale.X.SetDefault(1.0f);
            scale.Y.SetDefault(1.0f);
        }

        public ModulatableVector2 Location { get; }
        public ModulatableVector2 Scale { get; }
        public ModulatableValue<float> Rotation => Bindables[0];
        public ModulatableValue<float> Uniform => Bindables[1];
        public SamplerState SamplerState { get; set; }

        public override IControlModel ToModel()
        {
            var model = new TransformTextureModel
            {
                SamplerState = (SamplerStateModel)SamplerState.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (TransformTextureModel)model;
            LoadBaseModel(m);
            SamplerState.FromModel(m.SamplerState);
        }
    }
}
