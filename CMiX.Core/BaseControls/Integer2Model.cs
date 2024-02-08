// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.BaseControls
{
    public class Integer2Model : IControlModel
    {
        public Integer2Model()
        {
            ID = Guid.NewGuid();
        }

        public Integer2Model(int x, int y) : this()
        {
            X = new GenericValueModel<int>(x);
            Y = new GenericValueModel<int>(y);
        }

        public Guid ID { get; set; }

        public GenericValueModel<int> X { get; set; }
        public GenericValueModel<int> Y { get; set; }
    }
}
