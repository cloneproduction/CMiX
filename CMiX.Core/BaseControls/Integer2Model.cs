// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
