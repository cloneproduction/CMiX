// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models
{
    public class CounterModel : IModel
    {
        public CounterModel()
        {
            this.ID = Guid.NewGuid();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public int Count { get; set; }
    }
}
