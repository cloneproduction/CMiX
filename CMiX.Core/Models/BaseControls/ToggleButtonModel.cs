// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models
{
    public class ToggleButtonModel : IModel
    {
        public ToggleButtonModel()
        {
            this.ID = Guid.NewGuid();
            IsChecked = false;
        }

        public ToggleButtonModel(bool isChecked) : this()
        {
            IsChecked = isChecked;
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public bool IsChecked { get; set; }
    }
}
