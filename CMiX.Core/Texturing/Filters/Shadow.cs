// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Shadow : ObservableObject, IPrefab, ITextureFilter
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
            Blend = blend;
            Control = control;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public Blend Blend { get; set; }
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

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new ShadowModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Blend = (BlendModel)Blend.ToModel(),
            LightDirection = (Vector3Model)LightDirection.ToModel(),
            Height = (GenericValueModel<float>)Height.ToModel(),
            DotTolerance = (GenericValueModel<float>)DotTolerance.ToModel(),
            RayJitter = (GenericValueModel<float>)RayJitter.ToModel(),
            RayLength = (GenericValueModel<float>)RayLength.ToModel(),
            ShadowFade = (GenericValueModel<float>)ShadowFade.ToModel(),
            ShadowFallOffPow = (GenericValueModel<float>)ShadowFallOffPow.ToModel(),
            ShadowBlur = (GenericValueModel<float>)ShadowBlur.ToModel(),
            ShadowBlurPow = (GenericValueModel<float>)ShadowBlurPow.ToModel(),
            SharpOffset = (GenericValueModel<float>)SharpOffset.ToModel(),
            Control = (GenericValueModel<float>)Control.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (ShadowModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Blend.FromModel(m.Blend);
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
            Control.FromModel(m.Control);
        }
    }
}
