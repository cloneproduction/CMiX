// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Models
{

    public class FloatValueModel : IModel
    {
        public FloatValueModel()
        {
            ID = Guid.NewGuid();
        }

        public FloatValueModel(float value) : this()
        {
            Value = value;
        }

        public Guid ID { get; set; }
        public float Value { get; set; }
    }
}
