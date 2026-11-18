// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using CMiX.UI.Controls.String;

namespace CMiX.Studio.Avalonia.Views
{
    internal static class TreeRowAnchor
    {
        public static Point? CenterOf(TreeViewItem item, Visual relativeTo) =>
            TreeRows.RowBorderOf(item) is { } row ? row.TranslatePoint(new Point(row.Bounds.Width / 2, row.Bounds.Height / 2), relativeTo) : null;

        // A popup placed under it starts at the name. A row with no name box gives the whole row.
        public static Rect? FromNameOf(TreeViewItem item, Visual relativeTo)
        {
            if (TreeRows.RowBorderOf(item) is not { } row || row.TranslatePoint(default, relativeTo) is not { } topLeft)
                return null;

            var rect = new Rect(topLeft, row.Bounds.Size);
            var name = row.GetVisualDescendants().OfType<TextBoxEditable>().FirstOrDefault();
            return name?.TranslatePoint(default, relativeTo) is { } left ? rect.WithX(left.X).WithWidth(rect.Right - left.X) : rect;
        }
    }
}
