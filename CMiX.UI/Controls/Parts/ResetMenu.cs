// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Styling;

namespace CMiX.UI.Controls.Parts
{
    // The Reset menu of a value editor. The owner must have a ResetCommand property.
    // An owner with a ResetAllCommand also gets a Reset All row while that command is set.
    // The binding has an explicit Source because a menu popup cannot reach back to its owner through TemplatedParent.
    internal static class ResetMenu
    {
        public static ContextMenu Create(Control owner, bool withResetAll = false)
        {
            var resetItem = new MenuItem { Header = "Reset" };
            resetItem.Bind(MenuItem.CommandProperty, new Binding("ResetCommand") { Source = owner });
            var items = new List<MenuItem> { resetItem };
            if (withResetAll)
            {
                var resetAllItem = new MenuItem { Header = "Reset All" };
                resetAllItem.Bind(MenuItem.CommandProperty, new Binding("ResetAllCommand") { Source = owner });
                items.Add(resetAllItem);
            }

            var menu = new ContextMenu { ItemsSource = items };
            if (owner.TryFindResource("ContextMenuDefault", out var theme) && theme is ControlTheme controlTheme)
                menu.Theme = controlTheme;

            return menu;
        }

        // Keeps the default menu of the owner in step with its reset commands, once the owner is in the visual tree.
        // Call it once in the constructor of the owner. The reset all property is optional.
        public static void Follow(Control owner, AvaloniaProperty resetCommand, AvaloniaProperty? resetAllCommand = null)
        {
            ContextMenu? ownMenu = null;
            var attached = false;

            void Refresh() => Update(
                owner,
                owner.GetValue(resetCommand) as ICommand,
                resetAllCommand == null ? null : owner.GetValue(resetAllCommand) as ICommand,
                ref ownMenu);

            owner.AttachedToVisualTree += (_, _) =>
            {
                attached = true;
                Refresh();
            };
            owner.PropertyChanged += (_, e) =>
            {
                if (attached && (e.Property == resetCommand || e.Property == resetAllCommand))
                    Refresh();
            };
        }

        // The owner has the default menu only while it has a reset command. A menu set in XAML stays.
        private static void Update(Control owner, ICommand? resetCommand, ref ContextMenu? ownMenu)
        {
            Update(owner, resetCommand, null, ref ownMenu);
        }

        private static void Update(Control owner, ICommand? resetCommand, ICommand? resetAllCommand, ref ContextMenu? ownMenu)
        {
            if (resetCommand == null)
            {
                if (ownMenu != null && owner.ContextMenu == ownMenu)
                    owner.ContextMenu = null;
                ownMenu = null;
                return;
            }

            var withResetAll = resetAllCommand != null;
            if (ownMenu != null && owner.ContextMenu == ownMenu && (ownMenu.ItemCount == 2) != withResetAll)
                owner.ContextMenu = null;

            if (owner.ContextMenu == null)
                owner.ContextMenu = ownMenu = Create(owner, withResetAll);
        }
    }
}
