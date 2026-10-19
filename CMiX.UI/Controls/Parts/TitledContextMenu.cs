// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using CMiX.UI.AttachedProperties;

namespace CMiX.UI.Controls.Parts
{
    // Builds a context menu in the look of the app: the title of the owner, a line, then the rows.
    // The title and the line are hidden while the title is empty.
    // The menu opens with the row that the user used last under the pointer, as in Blender (see MenuAnchor).
    // Before the first use it opens on the default row, else on the first row.
    // The title and the anchor are read each time the menu opens, because both can change after the menu is built.
    public sealed class TitledContextMenu
    {
        private readonly Control owner;
        private readonly Func<string> title;
        private readonly string? defaultAnchorId;
        private readonly MenuItem titleItem = new() { IsEnabled = false };
        private readonly MenuItem lineItem = new() { Header = "-" };
        private readonly List<object> items = new();
        private readonly List<MenuItem> actionRows = new();

        // The title is a function, so a rename of the owner shows in the next menu.
        public TitledContextMenu(Control owner, Func<string> title, string? defaultAnchorId = null)
        {
            this.owner = owner;
            this.title = title;
            this.defaultAnchorId = defaultAnchorId;
            items.Add(titleItem);
            items.Add(lineItem);
        }

        // Adds a row that can open under the pointer. The id has a prefix for its menu, such as "value.copy".
        // A click on the row is remembered by the window. A row with a command marks its click as handled
        // when it runs the command, so the handler also takes handled clicks.
        public MenuItem Action(string id, MenuItem row)
        {
            MenuAnchor.SetId(row, id);
            row.AddHandler(MenuItem.ClickEvent, (_, _) =>
            {
                if (TopLevel.GetTopLevel(owner) is { } window)
                    MenuAnchor.SetLastUsed(window, id);
            }, RoutingStrategies.Bubble, handledEventsToo: true);
            actionRows.Add(row);
            items.Add(row);
            return row;
        }

        public MenuItem Action(string id, string header, Action onClick)
        {
            var row = Action(id, new MenuItem { Header = header });
            row.Click += (_, _) => onClick();
            return row;
        }

        // A thin line between two groups of rows.
        public void Line() => items.Add(new MenuItem { Header = "-" });

        public ContextMenu Build()
        {
            var menu = new ContextMenu { ItemsSource = items };

            void Refresh()
            {
                var text = title();
                titleItem.Header = text;
                titleItem.IsVisible = lineItem.IsVisible = text.Length > 0;
            }

            // The row with the id used last, if this menu has it. Else the default row. Else the first row.
            void MarkAnchor()
            {
                var last = TopLevel.GetTopLevel(owner) is { } window ? MenuAnchor.GetLastUsed(window) : null;
                var anchor = actionRows.FirstOrDefault(row => MenuAnchor.GetId(row) == last)
                             ?? actionRows.FirstOrDefault(row => MenuAnchor.GetId(row) == defaultAnchorId)
                             ?? actionRows.FirstOrDefault();
                foreach (var row in actionRows)
                    MenuAnchor.SetIsAnchor(row, row == anchor);
            }

            Refresh();
            MarkAnchor();
            menu.Opening += (_, _) =>
            {
                Refresh();
                MarkAnchor();
            };
            if (owner.TryFindResource("ContextMenuDefault", out var theme) && theme is ControlTheme controlTheme)
                menu.Theme = controlTheme;

            return menu;
        }
    }
}
