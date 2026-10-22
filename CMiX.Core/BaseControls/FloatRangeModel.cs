// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.BaseControls
{
    public class FloatRangeModel : IControlModel
    {
        public FloatRangeModel()
        {
            ID = Guid.NewGuid();
            Minimum = new GenericValueModel<float>(0f);
            Maximum = new GenericValueModel<float>(0f);
        }

        public FloatRangeModel(float minimum, float maximum) : this()
        {
            Minimum = new GenericValueModel<float>(minimum);
            Maximum = new GenericValueModel<float>(maximum);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<float> Minimum { get; set; }
        public GenericValueModel<float> Maximum { get; set; }
    }
}
