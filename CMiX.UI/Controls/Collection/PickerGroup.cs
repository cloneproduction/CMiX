// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;

namespace CMiX.UI.Controls.Collection
{
    // One row that can be picked in a ListGroupedSearch. Value is what the owner gets back. Kind is any object that the list
    // gives to its KindBrush function, to color the square of the row. It is null for a row with no color.
    // A row that is not enabled shows greyed. It cannot be picked, and the keys skip it.
    public sealed record PickerItem(string Text, object? Value = null, object? Kind = null, bool IsEnabled = true);

    // A group of rows under one header. Kind is the same as the kind of a row.
    public sealed record PickerGroup(string Header, IReadOnlyList<PickerItem> Items, object? Kind = null)
    {
        // True when at least one row of the group can be picked. A group with none starts closed and its header is greyed.
        public bool HasEnabledItems => Items.Any(item => item.IsEnabled);
    }

    // The row that the user picked, and its group.
    public sealed class PickerPickedEventArgs : EventArgs
    {
        public PickerPickedEventArgs(PickerGroup group, PickerItem item)
        {
            Group = group;
            Item = item;
        }

        public PickerGroup Group { get; }
        public PickerItem Item { get; }
    }
}
