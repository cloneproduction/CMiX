// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Styling;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    // The Reset menu of a value editor. The owner must have a ResetCommand property.
    // The binding has an explicit Source because a menu popup cannot reach back to its owner through TemplatedParent.
    internal static class DefaultResetMenu
    {
        public static ContextMenu Create(Control owner)
        {
            var resetItem = new MenuItem { Header = "Reset" };
            resetItem.Bind(MenuItem.CommandProperty, new Binding("ResetCommand") { Source = owner });

            var menu = new ContextMenu { ItemsSource = new[] { resetItem } };
            if (owner.TryFindResource("ContextMenuDefault", out var theme) && theme is ControlTheme controlTheme)
                menu.Theme = controlTheme;

            return menu;
        }
    }
}
