// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.BaseControls
{
    public partial class Vector3 : ObservableObject, IControl
    {
        public Vector3(FloatValue x, FloatValue y, FloatValue z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        //public Vector3(float x, float y, float z) : this()
        //{
        //    X.Value = x;
        //    Y.Value = y;
        //    Z.Value = z;
        //}

        public Guid ID { get; set; } = Guid.NewGuid();
        public FloatValue X { get; set; }
        public FloatValue Y { get; set; }
        public FloatValue Z { get; set; }
    }
}
