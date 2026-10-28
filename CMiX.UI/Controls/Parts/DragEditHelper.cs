// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;

namespace CMiX.UI.Controls.Parts
{
    // The app is Windows only, so the cursor wrap keeps using user32.
    // All coordinates here are physical screen pixels.
    internal static class DragEditHelper
    {
        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        private struct Win32Point { public Int32 X; public Int32 Y; }

        public static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        // What places the cursor. A test replaces it, to see where the cursor goes and to leave the real cursor alone.
        internal static Action<Point> CursorPlacer { get; set; } = point => SetCursorPos((int)point.X, (int)point.Y);

        public static void PlaceCursorAt(Point screenPoint) => CursorPlacer(screenPoint);

        // The bounds of the screen that has the given point, or nothing when the window does not know its screens.
        // A test replaces it.
        internal static Func<Visual, PixelPoint, PixelRect?> ScreenBoundsAt { get; set; } = (visual, point) =>
            (TopLevel.GetTopLevel(visual) as WindowBase)?.Screens.ScreenFromPoint(point)?.Bounds;

        public const double ClickThreshold = 3.0;

        // How much of the mouse movement counts while Shift is held, for the fine adjustment of a slider or a color wheel.
        public const double FineSpeed = 0.01;
    }
}
