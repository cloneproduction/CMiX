// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;

namespace CMiX.Core.Rendering.Lights
{
    public record LightSettingsModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<LightType> LightTypeSelector { get; set; } = new(LightType.AmbientLight);
        public GenericValueModel<string> LightColor { get; set; } = new("#FFFFFFFF");
        public Vector3Model Position { get; set; } = new(0.0f, 2.0f, 0.0f);
        public Vector3Model Target { get; set; } = new(0.001f, 0.0f, 0.0f);
        public GenericValueModel<float> Radius { get; set; } = new(5.0f);
        public GenericValueModel<bool> LightHelper { get; set; } = new(false);
        public GenericValueModel<float> Angle { get; set; } = new(0.25f);
        public GenericValueModel<float> Softness { get; set; } = new(0.01f);
        public GenericValueModel<float> Intensity { get; set; } = new(1.0f);
    }
}
