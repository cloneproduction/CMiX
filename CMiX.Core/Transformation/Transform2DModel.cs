// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;

namespace CMiX.Core.Transformation
{
    public class Transform2DModel : IControlModel
    {
        public Guid ID { get; init; } = Guid.NewGuid();
        public Vector2Model Translate { get; init; } = new Vector2Model(0.0f, 0.0f);
        public Vector2Model Scale { get; init; } = new Vector2Model(1.0f, 1.0f);
        public GenericValueModel<float> Rotate { get; init; } = new(0.0f);
        public GenericValueModel<float> UniformScale { get; init; } = new(1.0f);
    }
}
