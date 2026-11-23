// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The closed card next to the landing place, read from the layout of the candidate, because the rows on the screen still slide.
    public static class ListTreeNearCard
    {
        // Null when no closed card that can take the row is less than one row height from the dragged row.
        public static TreeNode? Of(ListTreeDragLook look, ListTreePress press, ListTreePoint pointer)
        {
            var center = pointer.Y - press.GrabOffsetY + look.Candidate.Place.RowHeight / 2;
            TreeNode? near = null;
            var nearest = look.Candidate.Place.RowHeight;
            foreach (var row in look.Rows)
            {
                if (!IsClosedCardWithATarget(look, row) || look.Layout.PlaceOf(row.Key) is not { } place)
                    continue;

                var distance = Math.Abs(place.Y + place.RowHeight / 2 - center);
                if (distance < nearest)
                {
                    nearest = distance;
                    near = row.Node;
                }
            }

            return near;
        }

        // A key puts the dragged row at the place of the candidate, with no pointer.
        public static TreeNode? OfKey(ListTreeDragLook look, ListTreePress press) =>
            Of(look, press, new ListTreePoint(press.Pointer.X, look.Candidate.Place.Y + press.GrabOffsetY));

        private static bool IsClosedCardWithATarget(ListTreeDragLook look, ListTreeRow row) =>
            !row.IsExpanded && row.Children.Count > 0 && !row.Key.Equals(look.Dragged) && row.Node is { HasDropTargetBelow: true };
    }
}
