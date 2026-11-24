// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;

namespace CMiX.Studio.Avalonia.Tests.Support
{
    // The mouse and the keys of a headless window, as the user uses them. Each action settles the window after it.
    internal static class Screen
    {
        // Runs the jobs, lets the animations end, runs the layout and renders.
        public static void Settle(this Window window)
        {
            Dispatcher.UIThread.RunJobs();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick(60);
            window.UpdateLayout();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            Dispatcher.UIThread.RunJobs();
        }

        // The middle of the control, in the window.
        public static Point CenterOf(this Window window, Control control) =>
            control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;

        public static void Hover(this Window window, Point point)
        {
            TreeDrag.Render(window);
            window.MouseMove(point);
            window.Settle();
        }

        public static void Hover(this Window window, Control control)
        {
            TreeDrag.Render(window);
            window.Hover(window.CenterOf(control));
        }

        public static void Click(this Window window, Point point, MouseButton button = MouseButton.Left,
            RawInputModifiers modifiers = RawInputModifiers.None)
        {
            TreeDrag.Render(window);
            window.MouseMove(point, modifiers);
            window.MouseDown(point, button, modifiers);
            window.MouseUp(point, button, modifiers);
            window.Settle();
        }

        public static void Click(this Window window, Control control, MouseButton button = MouseButton.Left,
            RawInputModifiers modifiers = RawInputModifiers.None)
        {
            TreeDrag.Render(window);
            window.Click(window.CenterOf(control), button, modifiers);
        }

        public static void RightClick(this Window window, Point point) => window.Click(point, MouseButton.Right);

        public static void RightClick(this Window window, Control control) => window.Click(control, MouseButton.Right);

        public static void DoubleClick(this Window window, Point point)
        {
            TreeDrag.Render(window);
            window.MouseMove(point);
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            window.Settle();
        }

        // Presses the key and lets it go.
        public static void Press(this Window window, PhysicalKey key, RawInputModifiers modifiers = RawInputModifiers.None)
        {
            window.KeyPressQwerty(key, modifiers);
            window.KeyReleaseQwerty(key, modifiers);
            window.Settle();
        }
    }
}
