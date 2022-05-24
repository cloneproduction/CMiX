// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class ComponentStyleSelector : StyleSelector
    {
        public Style ProjectStyle { get; set; }
        public Style LayerStyle { get; set; }
        public Style SceneStyle { get; set; }
        public Style CompositionStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            Style style = null;

            if (item != null)
            {
                if (item is Project)
                    style = ProjectStyle;
                else if (item is Composition)
                    style = CompositionStyle;
                else if (item is Layer)
                    style = LayerStyle;
            }

            return style;
        }
    }
}
