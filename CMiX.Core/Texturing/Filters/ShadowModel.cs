// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public record ShadowModel : IPrefabModel, ITextureFilterModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public BlendModel Blend { get; set; } = new();
        public ModulatableValueModel<float> Height { get; set; } = ModulatableValueModel<float>.Of("Height", 0.85f);
        public ModulatableValueModel<float> DotTolerance { get; set; } = ModulatableValueModel<float>.Of("Dot Tolerance", 0.36f);
        public ModulatableValueModel<float> RayJitter { get; set; } = ModulatableValueModel<float>.Of("Ray Jitter", 0.0f);
        public ModulatableValueModel<float> RayLength { get; set; } = ModulatableValueModel<float>.Of("Ray Length", -0.07f);
        public ModulatableValueModel<float> ShadowFade { get; set; } = ModulatableValueModel<float>.Of("Shadow Fade", 0.04f);
        public ModulatableValueModel<float> ShadowFallOffPow { get; set; } = ModulatableValueModel<float>.Of("Shadow FallOff Pow", 0.6f);
        public ModulatableValueModel<float> ShadowBlur { get; set; } = ModulatableValueModel<float>.Of("Shadow Blur", 0.001f);
        public ModulatableValueModel<float> ShadowBlurPow { get; set; } = ModulatableValueModel<float>.Of("Shadow Blur Pow", -0.49f);
        public ModulatableValueModel<float> SharpOffset { get; set; } = ModulatableValueModel<float>.Of("Sharp Offset", -0.05f);
        public ModulatableVector3Model LightDirection { get; set; } = ModulatableVector3Model.Of(2.51f, -0.91f, 1.15f);
        public PrefabManagerModel ModulatorManager { get; set; } = new();
    }
}
