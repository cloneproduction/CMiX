// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.BaseControls
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
