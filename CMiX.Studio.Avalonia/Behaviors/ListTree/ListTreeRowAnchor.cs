// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using CMiX.Studio.Avalonia.Views;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // The places of a row where a popup opens.
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
