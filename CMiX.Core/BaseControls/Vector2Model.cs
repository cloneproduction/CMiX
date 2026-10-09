// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.BaseControls
{
    public class Vector2Model : IControlModel
    {
        public Vector2Model()
        {
            ID = Guid.NewGuid();

            X = new GenericValueModel<float>(0.0f);
            Y = new GenericValueModel<float>(0.0f);
        }

        public Vector2Model(float x, float y) : this()
        {
            X = new GenericValueModel<float>(x);
            Y = new GenericValueModel<float>(y);
        }


        public Guid ID { get; set; }
        public GenericValueModel<float> X { get; set; }
        public GenericValueModel<float> Y { get; set; }
    }
}
