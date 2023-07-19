// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.BaseControls
{
    public class Vector3Model : IControlModel
    {
        public Vector3Model()
        {
            X = new FloatValueModel();
            Y = new FloatValueModel();
            Z = new FloatValueModel();
        }

        public Vector3Model(float x, float y, float z) : this()
        {
            X.Value = x;
            Y.Value = y;
            Z.Value = z;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public FloatValueModel X { get; set; }
        public FloatValueModel Y { get; set; }
        public FloatValueModel Z { get; set; }
    }
}
