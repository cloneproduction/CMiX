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
                      GenericValue<float> control,
                      PrefabManager modulatorManager)
            : base(prefabService, control, blend, modulatorManager)
        {
            LightDirection = lightDirection;

            Bindables = new List<ModulatableValue<float>>
            {
                height, dotTolerance, rayJitter, rayLength, shadowFade,
                shadowFallOffPow, shadowBlur, shadowBlurPow, sharpOffset,
                lightDirection.X, lightDirection.Y, lightDirection.Z
            };

            height.Label = "Height";
            dotTolerance.Label = "Dot Tolerance";
            rayJitter.Label = "Ray Jitter";
            rayLength.Label = "Ray Length";
            shadowFade.Label = "Shadow Fade";
            shadowFallOffPow.Label = "Shadow FallOff Pow";
            shadowBlur.Label = "Shadow Blur";
            shadowBlurPow.Label = "Shadow Blur Pow";
            sharpOffset.Label = "Sharp Offset";

            height.SetDefault(0.85f);
            dotTolerance.SetDefault(0.36f);
            rayJitter.SetDefault(0.0f);
            rayLength.SetDefault(-0.07f);
            shadowFade.SetDefault(0.04f);
            shadowFallOffPow.SetDefault(0.6f);
            shadowBlur.SetDefault(0.001f);
            shadowBlurPow.SetDefault(-0.49f);
            sharpOffset.SetDefault(-0.05f);
            lightDirection.X.SetDefault(2.51f);
            lightDirection.Y.SetDefault(-0.91f);
            lightDirection.Z.SetDefault(1.15f);
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
            var model = new ShadowModel();
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (ShadowModel)model;
            LoadBaseModel(m);
        }
    }
}
