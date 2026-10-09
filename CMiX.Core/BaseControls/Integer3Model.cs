// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.BaseControls
{
    public class Integer3Model : IControlModel
    {
        public Integer3Model()
        {
            ID = Guid.NewGuid();
            X = new GenericValueModel<int>(0);
            Y = new GenericValueModel<int>(0);
            Z = new GenericValueModel<int>(0);
        }

        public Integer3Model(int x, int y, int z) : this()
        {
            X = new GenericValueModel<int>(x);
            Y = new GenericValueModel<int>(y);
            Z = new GenericValueModel<int>(z);
        }

        public Guid ID { get; set; }
        public GenericValueModel<int> X { get; set; }
        public GenericValueModel<int> Y { get; set; }
        public GenericValueModel<int> Z { get; set; }
    }
}
