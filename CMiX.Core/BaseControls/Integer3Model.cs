// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.BaseControls
{
    public class Integer3Model : IControlModel
    {
        public Integer3Model()
        {
            ID = Guid.NewGuid();
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
