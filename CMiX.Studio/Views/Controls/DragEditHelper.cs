// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace CMiX.Studio.Views.Controls
{
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

        public static void WrapCursorX(Point currentPoint, int screenWidth, int screenHeight)
        {
            if (currentPoint.X >= screenWidth - 1)
                SetCursorPos(0, (int)currentPoint.Y);
            else if (currentPoint.X <= 0)
                SetCursorPos(screenWidth - 1, (int)currentPoint.Y);
        }

        public static readonly int ScreenWidth = (int)SystemParameters.PrimaryScreenWidth;
        public static readonly int ScreenHeight = (int)SystemParameters.PrimaryScreenHeight;

        public const double ClickThreshold = 3.0;
    }
}
