// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models
{
    public class RangeControlModel : IModel
    {
        public RangeControlModel()
        {
            this.ID = Guid.NewGuid();
            Range = new SliderModel();
            //Modifier = ((RangeModifier)0).ToString();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public SliderModel Range { get; set; }
        public string Modifier { get; set; }
    }
}
