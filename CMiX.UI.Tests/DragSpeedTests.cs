// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using CMiX.UI.Controls.Number;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // Shift slows a drag down to DragEditHelper.FineSpeed, and Ctrl speeds it up.
    public class DragSpeedTests
    {
        private const double FineSpeed = DragEditHelper.FineSpeed;

        // The saturation of the wheel after a press at the given local point and the moves that follow.
        private static double SaturationAfter(Point pressAt, params (Point At, KeyModifiers Modifiers)[] moves)
        {
            var (wheel, window, _) = ColorWheelHost.Show();
            var pointer = PointerInput.NewPointer();
            var origin = wheel.TranslatePoint(new Point(0, 0), window)!.Value;
            Point InWindow(Point local) => new(origin.X + local.X, origin.Y + local.Y);

            PointerInput.Press(wheel, pointer, InWindow(pressAt), window);
            foreach (var move in moves)
                PointerInput.Move(wheel, pointer, InWindow(move.At), window, move.Modifiers);

            return wheel.Color.HSV_S;
        }

        private static double SliderGainWith(RawInputModifiers modifiers)
        {
            var slider = new SliderCMiX { Minimum = 0, Maximum = 1, Value = 0.5, Width = 300, Height = 30, IsEditable = false };
            var window = TestWindow.ShowWithoutClips(slider, 1500, 400);
            var center = slider.TranslatePoint(new Point(150, 15), window)!.Value;

            window.MouseMove(center);
            window.MouseDown(center, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            window.MouseMove(new Point(center.X + 30, center.Y), modifiers);
            window.MouseMove(new Point(center.X + 60, center.Y), modifiers);
            Dispatcher.UIThread.RunJobs();
            var value = slider.Value;
            window.MouseUp(new Point(center.X + 60, center.Y), MouseButton.Left);

            return value - 0.5;
        }

        [AvaloniaFact]
        public void Drag_AWheelWithShift_MovesTheIndicatorByAFractionOfTheMouse()
        {
            var press = new Point(130, 100);
            var to = new Point(170, 100);
            var start = SaturationAfter(press);

            var normal = SaturationAfter(press, (to, KeyModifiers.None)) - start;
            var fine = SaturationAfter(press, (to, KeyModifiers.Shift)) - start;

            Assert.True(normal > 0.3);
            Assert.InRange(fine / normal, FineSpeed * 0.9, FineSpeed * 1.1);
        }

        [AvaloniaFact]
        public void Drag_AWheelWhenShiftIsLetGo_DoesNotJump()
        {
            var (wheel, window, _) = ColorWheelHost.Show();
            var pointer = PointerInput.NewPointer();
            var origin = wheel.TranslatePoint(new Point(0, 0), window)!.Value;
            Point InWindow(double x, double y) => new(origin.X + x, origin.Y + y);

            PointerInput.Press(wheel, pointer, InWindow(130, 100), window);
            var start = wheel.Color.HSV_S;
            PointerInput.Move(wheel, pointer, InWindow(170, 100), window, KeyModifiers.Shift);
            var slowed = wheel.Color.HSV_S;
            Assert.InRange(slowed - start, 0, 0.01);

            // Shift is let go and the mouse has not moved: the indicator stays where it is.
            PointerInput.Move(wheel, pointer, InWindow(170, 100), window);
            Assert.Equal(slowed, wheel.Color.HSV_S, 9);

            // And it follows the mouse at full speed again.
            PointerInput.Move(wheel, pointer, InWindow(180, 100), window);
            Assert.Equal(slowed + 10 / ((wheel.Height - 12) / 2), wheel.Color.HSV_S, 3);
        }

        [AvaloniaFact]
        public void Drag_ASliderWithShiftOrCtrl_ChangesTheSpeed()
        {
            var normal = SliderGainWith(RawInputModifiers.None);
            var fine = SliderGainWith(RawInputModifiers.Shift);
            var fast = SliderGainWith(RawInputModifiers.Control);

            // The first move only starts the drag, so 30 pixels count. The thumb moves as far as the cursor.
            Assert.InRange(normal, 0.05, 0.25);
            Assert.InRange(fine / normal, 0.0099, 0.0101);
            Assert.True(fast > normal * 2, "fast: " + fast);
        }
    }
}
