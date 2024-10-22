// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
