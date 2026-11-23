// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The band of the panel where the rows have a control: the view, and half a view above and below it.
    public sealed record ListTreeRowWindow(double Top, double Bottom)
    {
        public static ListTreeRowWindow All { get; } = new(double.NegativeInfinity, double.PositiveInfinity);

        public static ListTreeRowWindow Around(double viewTop, double viewHeight) =>
            new(viewTop - viewHeight / 2, viewTop + viewHeight * 1.5);

        // A quarter of a view stays at each side, so a small scroll keeps the controls.
        public bool Holds(double viewTop, double viewHeight) =>
            Top <= viewTop - viewHeight / 4 && Bottom >= viewTop + viewHeight * 1.25;

        public bool Overlaps(double top, double height) => top < Bottom && top + height > Top;
    }
}
