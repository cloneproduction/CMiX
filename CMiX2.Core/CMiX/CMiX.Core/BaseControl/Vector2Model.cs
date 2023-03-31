// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.BaseControl
{
    public class Vector2Model : IModel
    {
        public Vector2Model()
        {
            ID = Guid.NewGuid();

            X = new FloatValueModel();
            Y = new FloatValueModel();
        }

        public Vector2Model(float x, float y) : this()
        {
            X.Value = x;
            Y.Value = y;
        }


        public Guid ID { get; set; }
        public FloatValueModel X { get; set; }
        public FloatValueModel Y { get; set; }
    }
}
