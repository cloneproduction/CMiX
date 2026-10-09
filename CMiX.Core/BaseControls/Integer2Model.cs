// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.BaseControls
{
    public class Integer2Model : IControlModel
    {
        public Integer2Model()
        {
            ID = Guid.NewGuid();
            X = new GenericValueModel<int>(0);
            Y = new GenericValueModel<int>(0);
        }

        public Integer2Model(int x, int y) : this()
        {
            X = new GenericValueModel<int>(x);
            Y = new GenericValueModel<int>(y);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<int> X { get; set; }
        public GenericValueModel<int> Y { get; set; }
    }
}
