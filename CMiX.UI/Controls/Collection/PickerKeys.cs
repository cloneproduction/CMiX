// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia.Input;

namespace CMiX.UI.Controls.Collection
{
    // What the keys of a search list need from the list: its rows, its highlight and its search.
    public interface IPickerRows
    {
        int Count { get; }
        int Highlighted { get; }
        bool Searching { get; }
        bool IsUsable(int index);
        void Highlight(int index);
        bool TryActivate(int index);
        void ClearSearch();
    }

    // The cursor over the rows of a search list and the keys of its search field. The list keeps the highlighted row.
    public sealed class PickerKeys
    {
        private readonly IPickerRows rows;

        public PickerKeys(IPickerRows rows)
        {
            this.rows = rows;
        }

        // Highlights the row at the index, kept inside the rows, or no row when there are none.
        public void Reset(int index) => rows.Highlight(rows.Count == 0 ? -1 : Math.Clamp(index, 0, rows.Count - 1));

        public void Next() => Reset(UsableFrom(1));

        public void Previous() => Reset(UsableFrom(-1));

        public void OnKeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down:
                    Next();
                    e.Handled = true;
                    break;

                case Key.Up:
                    Previous();
                    e.Handled = true;
                    break;

                case Key.Enter:
                    e.Handled = rows.TryActivate(rows.Highlighted);
                    break;

                case Key.Escape when rows.Searching:
                    rows.ClearSearch();
                    e.Handled = true;
                    break;
            }
        }

        // The first usable row after the highlighted one in the direction, or the highlighted row when there is none.
        private int UsableFrom(int direction)
        {
            var start = rows.Highlighted;
            for (var index = start + direction; index >= 0 && index < rows.Count; index += direction)
            {
                if (rows.IsUsable(index))
                    return index;
            }

            return start;
        }
    }
}
