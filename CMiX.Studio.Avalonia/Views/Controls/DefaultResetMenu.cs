// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Windows.Input;
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

        // The owner has the default menu only while it has a reset command. A menu set in XAML stays.
        public static void Update(Control owner, ICommand? resetCommand, ref ContextMenu? ownMenu)
        {
            if (resetCommand == null)
            {
                if (ownMenu != null && owner.ContextMenu == ownMenu)
                    owner.ContextMenu = null;
                ownMenu = null;
                return;
            }

            if (owner.ContextMenu == null)
                owner.ContextMenu = ownMenu = Create(owner);
        }
    }
}
