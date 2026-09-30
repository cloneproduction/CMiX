// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public partial class TransformTexture : TextureFilterBase
    {
        public TransformTexture(PrefabService prefabService,
                        PrefabManager beatModifierManager,
                        SamplerState samplerState,
                        Vector2 location,
                        Vector2 scale,
                        GenericValue<float> rotation,
                        GenericValue<float> uniform,
                        GenericValue<float> control,
                        Blend blend,
                        PrefabManager modulatorManager)
            : base(prefabService, control, blend, modulatorManager)
        {
            SamplerState = samplerState;
            Location = location;
            Scale = scale;
            Rotation = rotation;
            Uniform = uniform;
        }

        public Vector2 Location { get; set; }
        public Vector2 Scale { get; set; }
        public GenericValue<float> Uniform { get; set; }
        public GenericValue<float> Rotation { get; set; }
        public SamplerState SamplerState { get; set; }

        public override IControlModel ToModel()
        {
            var model = new TransformTextureModel
            {
                Location = (Vector2Model)Location.ToModel(),
                Scale = (Vector2Model)Scale.ToModel(),
                Uniform = (GenericValueModel<float>)Uniform.ToModel(),
                Rotation = (GenericValueModel<float>)Rotation.ToModel(),
                SamplerState = (SamplerStateModel)SamplerState.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (TransformTextureModel)model;
            LoadBaseModel(m);
            Location.FromModel(m.Location);
            Scale.FromModel(m.Scale);
            Uniform.FromModel(m.Uniform);
            Rotation.FromModel(m.Rotation);
            SamplerState.FromModel(m.SamplerState);
        }
    }
}
