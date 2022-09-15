// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models
{
    public class ComboBoxModel<T> : IModel
    {
        public ComboBoxModel()
        {

        }

        public ComboBoxModel(T selected)
        {
            this.ID = Guid.NewGuid();
            Selection = selected;
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public T Selection { get; set; }
    }
}
