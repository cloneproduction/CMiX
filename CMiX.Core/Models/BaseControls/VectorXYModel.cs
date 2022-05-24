// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMiX.Core.Models.BaseControls
{
    public class VectorXYModel : IModel
    {
        public VectorXYModel()
        {
            ID = Guid.NewGuid();
            Enabled = true;

            X = new SliderModel();
            Y = new SliderModel();
        }

        public VectorXYModel(float x, float y) : this()
        {
            X.Amount = x;
            Y.Amount = y;
        }

        public VectorXYModel(string name) : this()
        {
            Name = name;
        }

        public VectorXYModel(string name, float x, float y) : this()
        {
            Name = name;
            X.Amount = x;
            Y.Amount = y;
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public string Name { get; set; }
        public SliderModel X { get; set; }
        public SliderModel Y { get; set; }
    }
}
