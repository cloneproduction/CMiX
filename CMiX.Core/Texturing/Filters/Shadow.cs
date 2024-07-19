// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public class Shadow : ObservableObject, ITextureModifier, IPrefab
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
                      GenericValue<BlendModeEnum> blendMode,
                      GenericValue<float> control)
        {
            PrefabService = prefabService;
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
            BlendMode = blendMode;
            Control = control;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public GenericValue<BlendModeEnum> BlendMode { get; set; }
        public GenericValue<float> Control { get; set; }
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
    }
}
