// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models
{
    public class Integer2Model : IModel
    {
        public Integer2Model()
        {
            this.ID = Guid.NewGuid();
        }

        public Integer2Model(int x, int y) : this()
        {
            X = x;
            Y = y;
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
