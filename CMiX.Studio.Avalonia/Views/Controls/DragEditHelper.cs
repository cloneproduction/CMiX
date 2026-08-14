// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views.Controls
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

        public static void PlaceCursorAt(Point screenPoint)
        {
            SetCursorPos((int)screenPoint.X, (int)screenPoint.Y);
        }

        // Screen metrics come from the visual's screen instead of WPF SystemParameters.
        public static PixelRect GetScreenBounds(Visual visual)
        {
            var topLevel = TopLevel.GetTopLevel(visual);
            if (topLevel is WindowBase window)
            {
                var screen = window.Screens.ScreenFromWindow(window) ?? window.Screens.Primary;
                if (screen != null)
                    return screen.Bounds;
            }
            return new PixelRect(0, 0, 1920, 1080);
        }

        public const double ClickThreshold = 3.0;
    }
}
