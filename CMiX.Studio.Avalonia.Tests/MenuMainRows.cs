// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using Avalonia.VisualTree;

namespace CMiX.Studio.Avalonia.Tests
{
    // Finds the items of the main menu of a window by their header.
    public static class MenuMainRows
    {
        public static MenuItem TopItem(Window window, string header) =>
            window.GetVisualDescendants().OfType<Menu>().First().Items.OfType<MenuItem>().Single(item => (string?)item.Header == header);

        // Opens the top item and returns one of its rows.
        public static MenuItem OpenRow(Window window, string top, string row)
        {
            var item = TopItem(window, top);
            item.Open();
            TestServiceProviderFactory.Pump();
            return item.Items.OfType<MenuItem>().Single(menuItem => (string?)menuItem.Header == row);
        }
    }
}
