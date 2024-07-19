// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Texturing.Filters
{
    public class ShadowModel : IPrefabModel
    {
        public ShadowModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            BlendMode = new GenericValueModel<BlendModeEnum>(BlendModeEnum.Normal);
            Control = new GenericValueModel<float>(1.0f);
            LightDirection = new Vector3Model(2.51f, -0.91f, 1.15f);
            Height = new GenericValueModel<float>(0.85f);
            DotTolerance = new GenericValueModel<float>(0.36f);
            RayJitter = new GenericValueModel<float>(0.0f);
            RayLength = new GenericValueModel<float>(-0.07f);
            ShadowFade = new GenericValueModel<float>(0.04f);
            ShadowFallOffPow = new GenericValueModel<float>(0.6f);
            ShadowBlur = new GenericValueModel<float>(0.001f);
            ShadowBlurPow = new GenericValueModel<float>(-0.49f);
            SharpOffset = new GenericValueModel<float>(-0.05f);
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public GenericValueModel<BlendModeEnum> BlendMode { get; set; }
        public Vector3Model LightDirection { get; set; }
        public GenericValueModel<float> Height { get; set; }
        public GenericValueModel<float> DotTolerance { get; set; }
        public GenericValueModel<float> RayJitter { get; set; }
        public GenericValueModel<float> RayLength { get; set; }
        public GenericValueModel<float> ShadowFade { get; set; }
        public GenericValueModel<float> ShadowFallOffPow { get; set; }
        public GenericValueModel<float> ShadowBlur { get; set; }
        public GenericValueModel<float> ShadowBlurPow { get; set; }
        public GenericValueModel<float> SharpOffset { get; set; }
        public GenericValueModel<float> Control { get; set; }
    }
}
