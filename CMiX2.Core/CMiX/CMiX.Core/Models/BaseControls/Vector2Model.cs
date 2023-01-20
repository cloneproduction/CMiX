// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models.BaseControls
{
    public class Vector2Model : IModel
    {
        public Vector2Model()
        {
            ID = Guid.NewGuid();
            Enabled = true;

            X = new FloatValueModel();
            Y = new FloatValueModel();
        }

        public Vector2Model(float x, float y) : this()
        {
            X.Value = x;
            Y.Value = y;
        }

        public Vector2Model(string name) : this()
        {
            Name = name;
        }

        public Vector2Model(string name, float x, float y) : this()
        {
            Name = name;
            X.Value = x;
            Y.Value = y;
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public string Name { get; set; }
        public FloatValueModel X { get; set; }
        public FloatValueModel Y { get; set; }
    }
}
