// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;

namespace CMiX.Core.Transformation
{
    public record TransformSRTModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<float> Uniform { get; set; } = new(1.0f);
        public Vector3Model Scale { get; set; } = new(1.0f, 1.0f, 1.0f);
        public Vector3Model Translate { get; set; } = new(0.0f, 0.0f, 0.0f);
        public Vector3Model Rotation { get; set; } = new(0.0f, 0.0f, 0.0f);
    }
}
