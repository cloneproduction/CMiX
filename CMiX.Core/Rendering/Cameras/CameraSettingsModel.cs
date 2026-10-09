// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;

namespace CMiX.Core.Rendering.Cameras
{
    public class CameraSettingsModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<float> FOV { get; set; } = new(0.09f);
        public GenericValueModel<float> Distance { get; set; } = new(10.0f);
        public GenericValueModel<float> Yaw { get; set; } = new(0.0f);
        public GenericValueModel<float> Pitch { get; set; } = new(0.0f);
        public Vector3Model Target { get; set; } = new(0.0f, 0.0f, 0.0f);
        public GenericValueModel<float> NearClip { get; set; } = new(0.05f);
        public GenericValueModel<float> FarClip { get; set; } = new(100.0f);
        public GenericValueModel<bool> Projection { get; set; } = new(false);
        public GenericValueModel<bool> IsOrthographic { get; set; } = new(false);
        public GenericValueModel<float> OrthographicSize { get; set; } = new(5.0f);
    }
}
