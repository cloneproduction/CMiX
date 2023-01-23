// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models
{
    public class ColorPickerModel : IModel
    {
        public ColorPickerModel()
        {
            ID = Guid.NewGuid();
            SelectedColor = "#ff00ff";
        }

        public Guid ID { get; set; }
        public string SelectedColor { get; set; }
    }
}
