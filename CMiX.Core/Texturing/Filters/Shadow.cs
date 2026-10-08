// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Shadow : TextureFilterBase
    {
        public Shadow(PrefabService prefabService,
                      ModulatableVector3 lightDirection,
                      ModulatableValue<float> height,
                      ModulatableValue<float> dotTolerance,
                      ModulatableValue<float> rayJitter,
                      ModulatableValue<float> rayLength,
                      ModulatableValue<float> shadowFade,
                      ModulatableValue<float> shadowFallOffPow,
                      ModulatableValue<float> shadowBlur,
                      ModulatableValue<float> shadowBlurPow,
                      ModulatableValue<float> sharpOffset,
                      Blend blend,
                      PrefabManager modulatorManager)
            : base(prefabService, blend, modulatorManager)
        {
            LightDirection = lightDirection;

            Bindables = new List<ModulatableValue<float>>
            {
                height, dotTolerance, rayJitter, rayLength, shadowFade,
                shadowFallOffPow, shadowBlur, shadowBlurPow, sharpOffset,
                lightDirection.X, lightDirection.Y, lightDirection.Z
            };
        }

        public ModulatableVector3 LightDirection { get; }
        public ModulatableValue<float> Height => Bindables[0];
        public ModulatableValue<float> DotTolerance => Bindables[1];
        public ModulatableValue<float> RayJitter => Bindables[2];
        public ModulatableValue<float> RayLength => Bindables[3];
        public ModulatableValue<float> ShadowFade => Bindables[4];
        public ModulatableValue<float> ShadowFallOffPow => Bindables[5];
        public ModulatableValue<float> ShadowBlur => Bindables[6];
        public ModulatableValue<float> ShadowBlurPow => Bindables[7];
        public ModulatableValue<float> SharpOffset => Bindables[8];

        public override IControlModel ToModel()
        {
            var model = new ShadowModel
            {
                Height = (ModulatableValueModel<float>)Height.ToModel(),
                DotTolerance = (ModulatableValueModel<float>)DotTolerance.ToModel(),
                RayJitter = (ModulatableValueModel<float>)RayJitter.ToModel(),
                RayLength = (ModulatableValueModel<float>)RayLength.ToModel(),
                ShadowFade = (ModulatableValueModel<float>)ShadowFade.ToModel(),
                ShadowFallOffPow = (ModulatableValueModel<float>)ShadowFallOffPow.ToModel(),
                ShadowBlur = (ModulatableValueModel<float>)ShadowBlur.ToModel(),
                ShadowBlurPow = (ModulatableValueModel<float>)ShadowBlurPow.ToModel(),
                SharpOffset = (ModulatableValueModel<float>)SharpOffset.ToModel(),
                LightDirection = (ModulatableVector3Model)LightDirection.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (ShadowModel)model;
            LoadBaseModel(m);
            Height.FromModel(m.Height);
            DotTolerance.FromModel(m.DotTolerance);
            RayJitter.FromModel(m.RayJitter);
            RayLength.FromModel(m.RayLength);
            ShadowFade.FromModel(m.ShadowFade);
            ShadowFallOffPow.FromModel(m.ShadowFallOffPow);
            ShadowBlur.FromModel(m.ShadowBlur);
            ShadowBlurPow.FromModel(m.ShadowBlurPow);
            SharpOffset.FromModel(m.SharpOffset);
            LightDirection.FromModel(m.LightDirection);
        }
    }
}
