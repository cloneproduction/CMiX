// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The grab offset is the pointer y minus the top of the pressed row. The depth is the depth of the pressed row.
    public sealed record ListTreePress(ListTreePoint Pointer, double GrabOffsetY, int Depth)
    {
        public static ListTreePress At(ListTreePoint pointer, ListTreePlace place) => new(pointer, pointer.Y - place.Y, place.Depth);

        // The press of a grab is at the middle of the row, so the row follows the pointer to the place under it.
        public static ListTreePress AtMiddleOf(ListTreePlace place, double pointerX) =>
            new(new ListTreePoint(pointerX, place.Y + place.RowHeight / 2), place.RowHeight / 2, place.Depth);
    }
}
