// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Styling;
using CMiX.UI.AttachedProperties;

namespace CMiX.UI.Controls.Parts
{
    // The menu of a value editor. The owner must have a ResetCommand property.
    // The menu starts with the title of the owner (see ControlTitle) and a line. It hides both while the title is empty.
    // An owner that is an IValueText also gets the rows Copy and Paste. They use the clipboard of the system.
    // An owner with a ResetAllCommand also gets a Reset All row while that command is set.
    // The menu opens with the row that the user used last under the pointer, as in Blender (see MenuAnchor).
    // The first time, it is Copy.
    // The binding has an explicit Source because a menu popup cannot reach back to its owner through TemplatedParent.
    internal static class ValueMenu
    {
        // The ids of the rows (see MenuAnchor.Id). The labels can change without touching them.
        private const string CopyId = "value.copy";
        private const string PasteId = "value.paste";
        private const string ResetId = "value.reset";
        private const string ResetAllId = "value.reset-all";

        public static ContextMenu Create(Control owner, bool withResetAll = false)
        {
            var titleItem = new MenuItem { IsEnabled = false };
            var lineItem = new MenuItem { Header = "-" };
            var items = new List<MenuItem> { titleItem, lineItem };

            // The rows that can open under the pointer. A click on one of them is remembered by the window.
            // A row with a command marks its click as handled when it runs the command, so the handler also takes handled clicks.
            var actionRows = new List<MenuItem>();
            MenuItem Action(string id, MenuItem row)
            {
                MenuAnchor.SetId(row, id);
                row.AddHandler(MenuItem.ClickEvent, (_, _) =>
                {
                    if (TopLevel.GetTopLevel(owner) is { } window)
                        MenuAnchor.SetLastUsed(window, id);
                }, RoutingStrategies.Bubble, handledEventsToo: true);
                actionRows.Add(row);
                return row;
            }

            MenuItem? copyItem = null;
            MenuItem? pasteItem = null;
            if (owner is IValueText valueText)
            {
                copyItem = Action(CopyId, new MenuItem { Header = "Copy" });
                copyItem.Click += async (_, _) => await CopyAsync(owner, valueText);
                pasteItem = Action(PasteId, new MenuItem { Header = "Paste", IsEnabled = false });
                pasteItem.Click += async (_, _) => await PasteAsync(owner, valueText);
                items.Add(copyItem);
                items.Add(pasteItem);
                items.Add(new MenuItem { Header = "-" });
            }

            var resetItem = Action(ResetId, new MenuItem { Header = "Reset" });
            resetItem.Bind(MenuItem.CommandProperty, new Binding("ResetCommand") { Source = owner });
            items.Add(resetItem);
            if (withResetAll)
            {
                var resetAllItem = Action(ResetAllId, new MenuItem { Header = "Reset All" });
                resetAllItem.Bind(MenuItem.CommandProperty, new Binding("ResetAllCommand") { Source = owner });
                items.Add(resetAllItem);
            }

            var menu = new ContextMenu { ItemsSource = items };

            // The caption and the clipboard can change after the menu is built, so both are read each time the menu opens.
            void Refresh()
            {
                var title = ControlTitle.Of(owner);
                titleItem.Header = title;
                titleItem.IsVisible = lineItem.IsVisible = title.Length > 0;
            }

            // The row with the id used last, if this menu has it. Else Copy. Else the first row.
            void MarkAnchor()
            {
                var last = TopLevel.GetTopLevel(owner) is { } window ? MenuAnchor.GetLastUsed(window) : null;
                var anchor = actionRows.FirstOrDefault(row => MenuAnchor.GetId(row) == last)
                             ?? actionRows.FirstOrDefault(row => MenuAnchor.GetId(row) == CopyId)
                             ?? actionRows.FirstOrDefault();
                foreach (var row in actionRows)
                    MenuAnchor.SetIsAnchor(row, row == anchor);
            }

            Refresh();
            MarkAnchor();
            menu.Opening += async (_, _) =>
            {
                Refresh();
                MarkAnchor();
                if (copyItem != null && pasteItem != null && owner is IValueText target)
                {
                    copyItem.IsEnabled = target.CanCopyValueText();
                    pasteItem.IsEnabled = target.CanPasteValueText(await ReadClipboardAsync(owner));
                }
            };
            if (owner.TryFindResource("ContextMenuDefault", out var theme) && theme is ControlTheme controlTheme)
                menu.Theme = controlTheme;

            return menu;
        }

        private static async System.Threading.Tasks.Task CopyAsync(Control owner, IValueText valueText)
        {
            var clipboard = TopLevel.GetTopLevel(owner)?.Clipboard;
            if (clipboard != null)
                await clipboard.SetTextAsync(valueText.CopyValueText());
        }

        private static async System.Threading.Tasks.Task PasteAsync(Control owner, IValueText valueText)
        {
            var text = await ReadClipboardAsync(owner);
            if (valueText.CanPasteValueText(text))
                valueText.PasteValueText(text);
        }

        private static async System.Threading.Tasks.Task<string?> ReadClipboardAsync(Control owner)
        {
            var clipboard = TopLevel.GetTopLevel(owner)?.Clipboard;
            return clipboard == null ? null : await clipboard.GetTextAsync();
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
            if (ownMenu != null && owner.ContextMenu == ownMenu && HasResetAll(ownMenu) != withResetAll)
                owner.ContextMenu = null;

            if (owner.ContextMenu == null)
                owner.ContextMenu = ownMenu = Create(owner, withResetAll);
        }

        private static bool HasResetAll(ContextMenu menu) =>
            menu.Items.OfType<MenuItem>().Any(item => MenuAnchor.GetId(item) == ResetAllId);
    }
}
