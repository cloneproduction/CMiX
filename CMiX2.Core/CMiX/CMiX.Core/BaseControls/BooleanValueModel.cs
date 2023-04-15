// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.BaseControls
{
    public class BooleanValueModel : IModel
    {
        public BooleanValueModel()
        {
            ID = Guid.NewGuid();
            Value = false;
        }

        public BooleanValueModel(bool isChecked) : this()
        {
            Value = isChecked;
        }

        public Guid ID { get; set; }
        public bool Value { get; set; }
    }
}
