// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.BaseControls
{
    public class Vector3Model : IControlModel
    {
        public Vector3Model()
        {
            X = new GenericValueModel<float>(0.0f);
            Y = new GenericValueModel<float>(0.0f);
            Z = new GenericValueModel<float>(0.0f);
        }

        public Vector3Model(float x, float y, float z)
        {
            X = new GenericValueModel<float>(x);
            Y = new GenericValueModel<float>(y);
            Z = new GenericValueModel<float>(z);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<float> X { get; set; }
        public GenericValueModel<float> Y { get; set; }
        public GenericValueModel<float> Z { get; set; }
    }
}
