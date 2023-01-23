// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models.BaseControls
{
    public class Vector3Model : IModel
    {
        public Vector3Model()
        {
            ID = Guid.NewGuid();

            X = new FloatValueModel();
            Y = new FloatValueModel();
            Z = new FloatValueModel();
        }

        public Vector3Model(string name) : this()
        {
            Name = name;
        }

        public Vector3Model(float x, float y, float z) : this()
        {
            X.Value = x;
            Y.Value = y;
            Z.Value = z;
        }

        public Vector3Model(string name, float x, float y, float z) : this()
        {
            Name = name;
            X.Value = x;
            Y.Value = y;
            Z.Value = z;
        }

        public Guid ID { get; set; }
        public string Name { get; set; }

        public FloatValueModel X { get; set; }
        public FloatValueModel Y { get; set; }
        public FloatValueModel Z { get; set; }
    }
}
