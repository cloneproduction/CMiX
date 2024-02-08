// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
            X.Value = x;
            Y.Value = y;
        }


        public Guid ID { get; set; }
        public GenericValueModel<float> X { get; set; }
        public GenericValueModel<float> Y { get; set; }
    }
}
