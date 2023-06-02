// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.BaseControls
{
    public class GenericValueModel<T> : IControlModel
    {
        public GenericValueModel()
        {
            ID = Guid.NewGuid();
        }

        public GenericValueModel(T selected)
        {
            ID = Guid.NewGuid();
            Value = selected;
        }

        public Guid ID { get; set; }
        public T Value { get; set; }
    }
}
