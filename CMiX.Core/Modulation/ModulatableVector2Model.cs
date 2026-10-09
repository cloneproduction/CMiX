// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Modulation
{
    public record ModulatableVector2Model : IControlModel
    {
        public Guid ID { get; init; } = Guid.NewGuid();
        public ModulatableValueModel<float> X { get; init; } = ModulatableValueModel<float>.Of("X", 0f);
        public ModulatableValueModel<float> Y { get; init; } = ModulatableValueModel<float>.Of("Y", 0f);

        public static ModulatableVector2Model Of(float x, float y) => new()
        {
            X = ModulatableValueModel<float>.Of("X", x),
            Y = ModulatableValueModel<float>.Of("Y", y)
        };
    }
}
