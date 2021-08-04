// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models
{
    public class SliderModel : IModel
    {
        public SliderModel()
        {
            ID = Guid.NewGuid();
            Amount = 0.0;
            Enabled = true;
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public double Amount { get; set; }
        public string Address { get; set; }
    }
}
