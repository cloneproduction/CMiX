// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Styling;
using CMiX.UI.AttachedProperties;

namespace CMiX.UI.Controls.Parts
{
    // The menu of a value editor. The owner must have a ResetCommand property.
    // The menu starts with the title of the owner (see ControlTitle) and a line (see TitledContextMenu).
    // An owner that is an IValueText also gets the rows Copy and Paste. They use the clipboard of the system.
    // An owner with a ResetAllCommand also gets a Reset All row while that command is set.
    // The menu opens with the row that the user used last under the pointer. The first time, it is Copy.
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
            var builder = new TitledContextMenu(owner, () => ControlTitle.Of(owner), CopyId);

            MenuItem? copyItem = null;
            MenuItem? pasteItem = null;
            if (owner is IValueText valueText)
            {
                copyItem = builder.Action(CopyId, new MenuItem { Header = "Copy" });
                copyItem.Click += async (_, _) => await CopyAsync(owner, valueText);
                pasteItem = builder.Action(PasteId, new MenuItem { Header = "Paste", IsEnabled = false });
                pasteItem.Click += async (_, _) => await PasteAsync(owner, valueText);
                builder.Line();
            }

            var resetItem = builder.Action(ResetId, new MenuItem { Header = "Reset" });
            resetItem.Bind(MenuItem.CommandProperty, new Binding("ResetCommand") { Source = owner });
            if (withResetAll)
            {
                var resetAllItem = builder.Action(ResetAllId, new MenuItem { Header = "Reset All" });
                resetAllItem.Bind(MenuItem.CommandProperty, new Binding("ResetAllCommand") { Source = owner });
            }

            var menu = builder.Build();

            // The clipboard can change after the menu is built, so it is read each time the menu opens.
            menu.Opening += async (_, _) =>
            {
                if (copyItem != null && pasteItem != null && owner is IValueText target)
                {
                    copyItem.IsEnabled = target.CanCopyValueText();
                    pasteItem.IsEnabled = target.CanPasteValueText(await ReadClipboardAsync(owner));
                }
            };

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
