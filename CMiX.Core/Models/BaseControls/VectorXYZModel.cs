// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models.BaseControls
{
    public class VectorXYZModel : IModel
    {
        public VectorXYZModel()
        {
            ID = Guid.NewGuid();
            Enabled = true;

            X = new SliderModel();
            Y = new SliderModel();
            Z = new SliderModel();
        }

        public VectorXYZModel(string name) : this()
        {
            Name = name;
        }

        public VectorXYZModel(float x, float y, float z) : this()
        {
            X.Amount = x;
            Y.Amount = y;
            Z.Amount = z;
        }

        public VectorXYZModel(string name, float x, float y, float z) : this()
        {
            Name = name;
            X.Amount = x;
            Y.Amount = y;
            Z.Amount = z;
        }

        public Guid ID { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }

        public SliderModel X { get; set; }
        public SliderModel Y { get; set; }
        public SliderModel Z { get; set; }
    }
}
