// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;

namespace CMiX.Studio.Avalonia.Views
{
    // The row controls that no row shows now. They stay hidden in the panel until a row comes into the window.
    internal sealed class ListTreeSpareRows
    {
        private readonly Stack<RowListTree> rows = new();

        public void KeepAll(IEnumerable<RowListTree> leaving)
        {
            foreach (var row in leaving)
            {
                row.IsVisible = false;
                row.DataContext = null;
                ListTreeRowSlide.Snap(row);
                row.StopFlash();
                rows.Push(row);
            }
        }

        public RowListTree? Take()
        {
            if (!rows.TryPop(out var row))
                return null;

            row.IsVisible = true;
            return row;
        }
    }
}
