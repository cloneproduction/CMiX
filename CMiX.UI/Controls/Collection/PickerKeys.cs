// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia.Input;

namespace CMiX.UI.Controls.Collection
{
    // The cursor over the rows of a search list and the keys of its search field. The list keeps the highlighted row.
    public sealed class PickerKeys
    {
        private readonly Func<int> count;
        private readonly Func<int, bool> isUsable;
        private readonly Func<int> highlighted;
        private readonly Action<int> highlight;
        private readonly Func<int, bool> tryActivate;
        private readonly Func<bool> searching;
        private readonly Action clearSearch;

        public PickerKeys(Func<int> count, Func<int, bool> isUsable, Func<int> highlighted, Action<int> highlight,
            Func<int, bool> tryActivate, Func<bool> searching, Action clearSearch)
        {
            this.count = count;
            this.isUsable = isUsable;
            this.highlighted = highlighted;
            this.highlight = highlight;
            this.tryActivate = tryActivate;
            this.searching = searching;
            this.clearSearch = clearSearch;
        }

        // Highlights the row at the index, kept inside the rows, or no row when there are none.
        public void Reset(int index)
        {
            var rows = count();
            highlight(rows == 0 ? -1 : Math.Clamp(index, 0, rows - 1));
        }

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
                    e.Handled = tryActivate(highlighted());
                    break;

                case Key.Escape when searching():
                    clearSearch();
                    e.Handled = true;
                    break;
            }
        }

        // The first usable row after the highlighted one in the direction, or the highlighted row when there is none.
        private int UsableFrom(int direction)
        {
            var start = highlighted();
            var rows = count();
            for (var index = start + direction; index >= 0 && index < rows; index += direction)
            {
                if (isUsable(index))
                    return index;
            }

            return start;
        }
    }
}
