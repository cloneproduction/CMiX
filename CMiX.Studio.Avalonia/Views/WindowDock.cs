// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views
{
    // Puts a window on the right edge of the screen, as high as the working area (the screen without the task bar).
    public static class WindowDock
    {
        // The place of a window of the given width (in device independent pixels) on a working area (in screen pixels).
        public static WindowDockPlace Compute(PixelRect workingArea, double scaling, double width)
        {
            var fitted = Math.Min(width, workingArea.Width / scaling);
            var x = workingArea.Right - (int)Math.Round(fitted * scaling);
            return new WindowDockPlace(new PixelPoint(x, workingArea.Y), fitted, workingArea.Height / scaling);
        }

        // Docks the window on the screen where it is. A window with no screen stays as it is.
        public static void DockRight(Window window)
        {
            if (window.Screens.ScreenFromWindow(window) is not { } screen)
                return;

            var place = Compute(screen.WorkingArea, screen.Scaling, window.Width);
            window.Width = place.Width;
            window.Height = place.Height;
            window.Position = place.Position;
        }
    }
}
