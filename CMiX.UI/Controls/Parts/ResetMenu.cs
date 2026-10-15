// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Styling;
using CMiX.UI.AttachedProperties;

namespace CMiX.UI.Controls.Parts
{
    // The Reset menu of a value editor. The owner must have a ResetCommand property.
    // The menu starts with the title of the owner (see ControlTitle) and a line. It hides both while the title is empty.
    // An owner with a ResetAllCommand also gets a Reset All row while that command is set.
    // The binding has an explicit Source because a menu popup cannot reach back to its owner through TemplatedParent.
    internal static class ResetMenu
    {
        // The title, the line, Reset and Reset All.
        private const int ItemCountWithResetAll = 4;

        public static ContextMenu Create(Control owner, bool withResetAll = false)
        {
            var titleItem = new MenuItem { IsEnabled = false };
            var lineItem = new MenuItem { Header = "-" };
            var resetItem = new MenuItem { Header = "Reset" };
            resetItem.Bind(MenuItem.CommandProperty, new Binding("ResetCommand") { Source = owner });
            var items = new List<MenuItem> { titleItem, lineItem, resetItem };
            if (withResetAll)
            {
                var resetAllItem = new MenuItem { Header = "Reset All" };
                resetAllItem.Bind(MenuItem.CommandProperty, new Binding("ResetAllCommand") { Source = owner });
                items.Add(resetAllItem);
            }

            var menu = new ContextMenu { ItemsSource = items };

            // The caption can change after the menu is built, so the title is read each time the menu opens.
            void RefreshTitle()
            {
                var title = ControlTitle.Of(owner);
                titleItem.Header = title;
                titleItem.IsVisible = lineItem.IsVisible = title.Length > 0;
            }

            RefreshTitle();
            menu.Opening += (_, _) => RefreshTitle();
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
            if (ownMenu != null && owner.ContextMenu == ownMenu && (ownMenu.ItemCount == ItemCountWithResetAll) != withResetAll)
                owner.ContextMenu = null;

            if (owner.ContextMenu == null)
                owner.ContextMenu = ownMenu = Create(owner, withResetAll);
        }
    }
}
