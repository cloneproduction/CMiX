// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.BaseControls
{
    public class IntegerValueModel : IControlModel
    {
        public IntegerValueModel()
        {
            ID = Guid.NewGuid();
        }

        public IntegerValueModel(int count) : this()
        {
            Value = count;
        }

        public Guid ID { get; set; }
        public int Value { get; set; }
    }
}
