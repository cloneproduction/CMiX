// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Shadow : TextureFilterBase
    {
        public Shadow(PrefabService prefabService,
                      Vector3 lightDirection,
                      GenericValue<float> height,
                      GenericValue<float> dotTolerance,
                      GenericValue<float> rayJitter,
                      GenericValue<float> rayLength,
                      GenericValue<float> shadowFade,
                      GenericValue<float> shadowFallOffPow,
                      GenericValue<float> shadowBlur,
                      GenericValue<float> shadowBlurPow,
                      GenericValue<float> sharpOffset,
                      Blend blend,
                      GenericValue<float> control,
                      PrefabManager modulatorManager)
            : base(prefabService, control, blend, modulatorManager)
        {
            LightDirection = lightDirection;
            Height = height;
            DotTolerance = dotTolerance;
            RayJitter = rayJitter;
            RayLength = rayLength;
            ShadowFade = shadowFade;
            ShadowFallOffPow = shadowFallOffPow;
            ShadowBlur = shadowBlur;
            ShadowBlurPow = shadowBlurPow;
            SharpOffset = sharpOffset;
        }

        public Vector3 LightDirection { get; set; }
        public GenericValue<float> Height { get; set; }
        public GenericValue<float> DotTolerance { get; set; }
        public GenericValue<float> RayJitter { get; set; }
        public GenericValue<float> RayLength { get; set; }
        public GenericValue<float> ShadowFade { get; set; }
        public GenericValue<float> ShadowFallOffPow { get; set; }
        public GenericValue<float> ShadowBlur { get; set; }
        public GenericValue<float> ShadowBlurPow { get; set; }
        public GenericValue<float> SharpOffset { get; set; }

        public override IControlModel ToModel()
        {
            var model = new ShadowModel
            {
                LightDirection = (Vector3Model)LightDirection.ToModel(),
                Height = (GenericValueModel<float>)Height.ToModel(),
                DotTolerance = (GenericValueModel<float>)DotTolerance.ToModel(),
                RayJitter = (GenericValueModel<float>)RayJitter.ToModel(),
                RayLength = (GenericValueModel<float>)RayLength.ToModel(),
                ShadowFade = (GenericValueModel<float>)ShadowFade.ToModel(),
                ShadowFallOffPow = (GenericValueModel<float>)ShadowFallOffPow.ToModel(),
                ShadowBlur = (GenericValueModel<float>)ShadowBlur.ToModel(),
                ShadowBlurPow = (GenericValueModel<float>)ShadowBlurPow.ToModel(),
                SharpOffset = (GenericValueModel<float>)SharpOffset.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (ShadowModel)model;
            LoadBaseModel(m);
            LightDirection.FromModel(m.LightDirection);
            Height.FromModel(m.Height);
            DotTolerance.FromModel(m.DotTolerance);
            RayJitter.FromModel(m.RayJitter);
            RayLength.FromModel(m.RayLength);
            ShadowFade.FromModel(m.ShadowFade);
            ShadowFallOffPow.FromModel(m.ShadowFallOffPow);
            ShadowBlur.FromModel(m.ShadowBlur);
            ShadowBlurPow.FromModel(m.ShadowBlurPow);
            SharpOffset.FromModel(m.SharpOffset);
        }
    }
}
