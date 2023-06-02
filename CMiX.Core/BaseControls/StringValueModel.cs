// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.BaseControls
{
    public class StringValueModel : IControlModel
    {
        public StringValueModel()
        {
            ID = Guid.NewGuid();
            Value = "";
        }

        public StringValueModel(string value) : this()
        {
            Value = value;
        }

        public Guid ID { get; set; }
        public string Value { get; internal set; }
    }
}
