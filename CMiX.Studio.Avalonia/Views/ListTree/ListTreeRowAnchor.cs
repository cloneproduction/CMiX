// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;

namespace CMiX.Studio.Avalonia.Views
{
    // The places of a row of the new tree where a popup opens, as TreeRowAnchor gives them for the old tree.
    internal static class ListTreeRowAnchor
    {
        public static Point? CenterOf(RowListTree row, Visual relativeTo)
        {
            var band = row.Band;
            return band.TranslatePoint(new Point(band.Bounds.Width / 2, band.Bounds.Height / 2), relativeTo);
        }

        // A popup placed under it starts at the name.
        public static Rect? FromNameOf(RowListTree row, Visual relativeTo)
        {
            var band = row.Band;
            if (band.TranslatePoint(default, relativeTo) is not { } topLeft)
                return null;

            var rect = new Rect(topLeft, band.Bounds.Size);
            return row.NameText.TranslatePoint(default, relativeTo) is { } left ? rect.WithX(left.X).WithWidth(rect.Right - left.X) : rect;
        }
    }
}
