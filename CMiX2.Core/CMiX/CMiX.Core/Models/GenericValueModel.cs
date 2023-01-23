// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models
{
    public class GenericValueModel<T> : IModel
    {
        public GenericValueModel()
        {

        }

        public GenericValueModel(T selected)
        {
            this.ID = Guid.NewGuid();
            Value = selected;
        }

        public Guid ID { get; set; }
        public T Value { get; set; }
    }
}
