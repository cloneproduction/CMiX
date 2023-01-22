// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Media;

namespace CMiX.Core.Models
{
    public class ColorSelectorModel : IModel
    {
        public ColorSelectorModel()
        {
            ID = Guid.NewGuid();
            SelectedColor = "#FFFFFFFF";
        }

        public ColorSelectorModel(string ColorHEX) : this()
        {
            SelectedColor = ColorHEX;
        }

        public Guid ID { get; set; }
        public string SelectedColor { get; set; }
    }
}
