// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models.BaseControls
{
    public class StringValueModel : IModel
    {
        public StringValueModel()
        {
            ID = Guid.NewGuid();
            Enabled = true;
            Value = "";
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public string Value { get; internal set; }
    }
}
