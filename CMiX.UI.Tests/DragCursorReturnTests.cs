// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.ColorPicker;
using CMiX.UI.Controls.Number;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // The cursor is hidden for a drag. At the release it goes back on the thumb of a slider, on the indicator of a wheel,
    // or where the press was on a drag value, and never on a place where the next press does not start a drag.
    public class DragCursorReturnTests
    {
        private static void DragAndRelease(Window window, SliderCMiX slider, Point target)
        {
            var center = slider.TranslatePoint(new Point(slider.Bounds.Width / 2, slider.Bounds.Height / 2), window)!.Value;
            window.MouseMove(center);
            Dispatcher.UIThread.RunJobs();
            window.MouseDown(center, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            for (var i = 1; i <= 4; i++)
            {
                window.MouseMove(new Point(center.X + (target.X - center.X) * i / 4, center.Y + (target.Y - center.Y) * i / 4));
                Dispatcher.UIThread.RunJobs();
            }
            window.MouseUp(target, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
        }

        private static void AssertOnTheSlider(SliderCMiX slider, Point placed)
        {
            var topLeft = slider.PointToScreen(new Point(0, 0));
            var bottomRight = slider.PointToScreen(new Point(slider.Bounds.Width, slider.Bounds.Height));

            Assert.InRange(placed.X, topLeft.X, bottomRight.X);
            Assert.InRange(placed.Y, topLeft.Y, bottomRight.Y);
        }

        private static bool TrackIsUnderThePointer(SliderCMiX slider) => Parts.Find<Border>(slider, "sliderBorder").IsPointerOver;

        // A press starts a drag only when the pointer is over the track. This moves the mouse to the place where the
        // cursor was put back, as the person does, and asks the track.
        private static bool OverTheTrackAt(Window window, SliderCMiX slider, Point placedOnScreen)
        {
            PointerInput.Move(window, window.PointToClient(new PixelPoint((int)placedOnScreen.X, (int)placedOnScreen.Y)));
            return TrackIsUnderThePointer(slider);
        }

        // The color selector with its popup open, without the clips for the hit test.
        private static (Window Window, Popup Popup) ShowColorPickerPopup()
        {
            var (window, _, _, popup) = SelectorColorHost.ShowOpen(300, 1000, 600);
            foreach (var visual in window.GetVisualDescendants().Where(v => v.Clip != null))
                visual.Clip = null;
            Dispatcher.UIThread.RunJobs();
            return (window, popup);
        }

        // The color picker stacks its modes (RGB, HSV and others) on top of each other, and only the visible mode takes
        // the mouse. This is the horizontal slider that is really under the pointer at its center.
        private static SliderCMiX HorizontalSliderUnderTheMouse(Window window, Popup popup)
        {
            foreach (var slider in popup.Child!.GetVisualDescendants().OfType<SliderCMiX>().Where(s => s.IsEffectivelyVisible && s.Orientation == Orientation.Horizontal))
            {
                PointerInput.Move(window, slider.TranslatePoint(new Point(slider.Bounds.Width / 2, slider.Bounds.Height / 2), window)!.Value);
                if (TrackIsUnderThePointer(slider))
                    return slider;
            }

            throw new InvalidOperationException("No horizontal slider of the popup is under the mouse.");
        }

        private static SliderCMiX HorizontalSlider() => new()
        {
            Minimum = 0, Maximum = 1, Value = 0.5, Width = 300, Height = 30, IsEditable = false, LargeChange = 1,
        };

        // The slider next to the color wheel: vertical, the maximum at the top.
        private static SliderCMiX BrightnessSlider() => new()
        {
            Theme = Tokens.Theme("SliderCMiXBrightness"),
            Minimum = 0, Maximum = 100, Value = 50, Width = 16, Height = 200, IsEditable = false, LargeChange = 1,
        };

        // Where the indicator of the color is, on the screen: the same formula as the wheel uses.
        private static Point IndicatorOnScreen(ColorPickerWheel wheel)
        {
            var ellipse = Parts.Find<Ellipse>(wheel, "PART_CursorEllipse");
            var angle = wheel.Color.HSV_H * Math.PI / 180;
            var radius = (wheel.Height - ellipse.Height) / 2 * wheel.Color.HSV_S;
            var location = new Point(wheel.Width / 2 + Math.Cos(angle) * radius, wheel.Height / 2 - Math.Sin(angle) * radius);
            var onScreen = wheel.PointToScreen(location);

            return new Point(onScreen.X, onScreen.Y);
        }

        // The drag value.

        [AvaloniaFact]
        public void Release_AfterADragValueDrag_PutsTheCursorBackWhereThePressWas()
        {
            using var spy = new CursorSpy();
            var drag = new DragValue { Width = 300, Height = 30, Value = 0 };
            var window = TestWindow.ShowWithoutClips(drag, 400, 200);
            var border = Parts.Find<Border>(drag, "borderValueDisplay");
            var center = border.TranslatePoint(new Point(border.Bounds.Width / 2, border.Bounds.Height / 2), window)!.Value;

            window.MouseMove(center);
            window.MouseDown(center, MouseButton.Left);
            window.MouseMove(new Point(center.X + 20, center.Y));
            window.MouseMove(new Point(center.X + 40, center.Y));
            window.MouseUp(new Point(center.X + 40, center.Y), MouseButton.Left);
            Dispatcher.UIThread.RunJobs();

            Assert.Single(spy.Placed);
            Assert.NotEqual(StandardCursorType.None.ToString(), border.Cursor?.ToString());
            Assert.False(drag.IsEditing);
        }

        // The slider.

        [AvaloniaTheory]
        [InlineData(40, 0)]
        [InlineData(200, 150)]
        public void Release_AfterASliderDrag_PutsTheCursorBackOnTheThumb(double dx, double dy)
        {
            using var spy = new CursorSpy();
            var slider = new SliderCMiX { Minimum = 0, Maximum = 1, Value = 0.5, Width = 300, Height = 30, IsEditable = false };
            var window = TestWindow.ShowWithoutClips(slider, 700, 400);
            var center = slider.TranslatePoint(new Point(150, 15), window)!.Value;

            DragAndRelease(window, slider, new Point(center.X + dx, center.Y + dy));

            Assert.Single(spy.Placed);
            AssertOnTheSlider(slider, spy.Placed[0]);
        }

        [AvaloniaTheory]
        [InlineData(30, 0)]
        [InlineData(420, -150)]
        public void Release_AfterADragInThePopup_PutsTheCursorBackOnTheThumb(double dx, double dy)
        {
            using var spy = new CursorSpy();
            var (window, popup) = ShowColorPickerPopup();
            var slider = HorizontalSliderUnderTheMouse(window, popup);
            var center = slider.TranslatePoint(new Point(slider.Bounds.Width / 2, slider.Bounds.Height / 2), window)!.Value;

            DragAndRelease(window, slider, new Point(center.X + dx, center.Y + dy));

            Assert.Single(spy.Placed);
            AssertOnTheSlider(slider, spy.Placed[0]);
            // The cursor is back next to the popup, so the next move keeps it open.
            Assert.True(popup.IsOpen);
        }

        [AvaloniaTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void Release_AtAnEndOfAHorizontalSlider_PutsTheCursorOverTheTrack(bool toMaximum)
        {
            using var spy = new CursorSpy();
            var slider = HorizontalSlider();
            var window = TestWindow.ShowWithoutClips(slider, 1500, 1200);
            var center = slider.TranslatePoint(new Point(150, 15), window)!.Value;

            DragAndRelease(window, slider, new Point(center.X + (toMaximum ? 600 : -600), center.Y));

            Assert.Equal(toMaximum ? 1.0 : 0.0, slider.Value);
            Assert.True(OverTheTrackAt(window, slider, spy.Placed[^1]));

            // Dragging again from there moves the value.
            var before = slider.Value;
            var at = window.PointToClient(new PixelPoint((int)spy.Placed[^1].X, (int)spy.Placed[^1].Y));
            var direction = toMaximum ? -1 : 1;
            window.MouseDown(at, MouseButton.Left);
            for (var i = 1; i <= 3; i++)
            {
                window.MouseMove(new Point(at.X + direction * 25 * i, at.Y));
                Dispatcher.UIThread.RunJobs();
            }
            window.MouseUp(new Point(at.X + direction * 75, at.Y), MouseButton.Left);
            Assert.NotEqual(before, slider.Value);
        }

        [AvaloniaTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void Release_AtAnEndOfAVerticalSlider_PutsTheCursorOverTheTrack(bool toMaximum)
        {
            using var spy = new CursorSpy();
            var slider = BrightnessSlider();
            var window = TestWindow.ShowWithoutClips(slider, 1500, 1200);
            var center = slider.TranslatePoint(new Point(8, 100), window)!.Value;

            // The maximum is at the top.
            DragAndRelease(window, slider, new Point(center.X, center.Y + (toMaximum ? -500 : 500)));

            Assert.Equal(toMaximum ? 100.0 : 0.0, slider.Value);
            Assert.True(OverTheTrackAt(window, slider, spy.Placed[^1]));
        }

        [AvaloniaFact]
        public void Release_AtTheMaximumInTheColorPickerPopup_PutsTheCursorOverTheTrack()
        {
            using var spy = new CursorSpy();
            var (window, popup) = ShowColorPickerPopup();
            var slider = HorizontalSliderUnderTheMouse(window, popup);
            slider.LargeChange = 1;
            var center = slider.TranslatePoint(new Point(slider.Bounds.Width / 2, slider.Bounds.Height / 2), window)!.Value;

            DragAndRelease(window, slider, new Point(center.X + 300, center.Y));

            // The cursor is at the right end of the slider, and the popup is still open.
            var right = slider.PointToScreen(new Point(slider.Bounds.Width, slider.Bounds.Height / 2));
            Assert.InRange(spy.Placed[^1].X, right.X - 12, right.X);
            Assert.True(popup.IsOpen);
            Assert.True(OverTheTrackAt(window, slider, spy.Placed[^1]));
        }

        [AvaloniaFact]
        public void Hover_TheValueOfASlider_LeavesThePointerToTheTrack()
        {
            var slider = HorizontalSlider();
            var window = TestWindow.ShowWithoutClips(slider, 1500, 1200);
            var label = Parts.Find<Label>(slider, "textDisplay");

            PointerInput.Move(window, label.TranslatePoint(new Point(label.Bounds.Width - 12, label.Bounds.Height / 2), window)!.Value);

            Assert.False(label.IsHitTestVisible);
            Assert.True(TrackIsUnderThePointer(slider));
        }

        // The color wheel.

        [AvaloniaTheory]
        [InlineData(120, 0)]
        [InlineData(450, -80)]
        [InlineData(-250, 300)]
        public void Release_AfterAWheelDrag_PutsTheCursorBackOnTheIndicator(double dx, double dy)
        {
            using var spy = new CursorSpy();
            var (wheel, window, colors) = ColorWheelHost.Show();
            var origin = wheel.TranslatePoint(new Point(0, 0), window)!.Value;
            var start = new Point(origin.X + 150, origin.Y + 100);

            window.MouseMove(start);
            window.MouseDown(start, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            var end = new Point(start.X + dx, start.Y + dy);
            for (var i = 1; i <= 4; i++)
            {
                window.MouseMove(new Point(start.X + dx * i / 4, start.Y + dy * i / 4));
                Dispatcher.UIThread.RunJobs();
            }
            window.MouseUp(end, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();

            Assert.NotEmpty(colors);
            var placed = Assert.Single(spy.Placed);
            var expected = IndicatorOnScreen(wheel);
            Assert.InRange(placed.X, expected.X - 1, expected.X + 1);
            Assert.InRange(placed.Y, expected.Y - 1, expected.Y + 1);

            // The cursor is inside the circle of the wheel.
            var center = wheel.PointToScreen(new Point(wheel.Width / 2, wheel.Height / 2));
            Assert.True(Math.Sqrt(Math.Pow(placed.X - center.X, 2) + Math.Pow(placed.Y - center.Y, 2)) < wheel.Width / 2);
        }

        [AvaloniaFact]
        public void Drag_AWheelFromWhereTheCursorIsPutBack_ChangesTheColor()
        {
            using var spy = new CursorSpy();
            var (wheel, window, colors) = ColorWheelHost.Show();
            var origin = wheel.TranslatePoint(new Point(0, 0), window)!.Value;
            var start = new Point(origin.X + 150, origin.Y + 100);

            window.MouseMove(start);
            window.MouseDown(start, MouseButton.Left);
            window.MouseMove(new Point(start.X + 400, start.Y - 60));
            window.MouseUp(new Point(start.X + 400, start.Y - 60), MouseButton.Left);
            Dispatcher.UIThread.RunJobs();

            var at = window.PointToClient(new PixelPoint((int)spy.Placed[^1].X, (int)spy.Placed[^1].Y));
            var before = colors.Count;
            window.MouseMove(at);
            window.MouseDown(at, MouseButton.Left);
            window.MouseMove(new Point(at.X - 40, at.Y + 30));
            window.MouseMove(new Point(at.X - 80, at.Y + 60));
            window.MouseUp(new Point(at.X - 80, at.Y + 60), MouseButton.Left);
            Dispatcher.UIThread.RunJobs();

            Assert.True(colors.Count > before);
        }

        [AvaloniaFact]
        public void Drag_AWheelOutsideItself_HidesTheCursorUntilTheRelease()
        {
            using var spy = new CursorSpy();
            var (wheel, window, _) = ColorWheelHost.Show();
            var origin = wheel.TranslatePoint(new Point(0, 0), window)!.Value;
            var start = new Point(origin.X + 150, origin.Y + 100);
            var outside = new Point(origin.X + 330, origin.Y + 100);
            var before = wheel.Cursor?.ToString();

            window.MouseMove(start);
            window.MouseDown(start, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(StandardCursorType.None.ToString(), wheel.Cursor?.ToString());
            Assert.Contains(":dragging", wheel.Classes);

            // The pointer leaves the wheel and the control says so, while the drag goes on.
            window.MouseMove(outside);
            wheel.RaiseEvent(new PointerEventArgs(InputElement.PointerExitedEvent, wheel, PointerInput.NewPointer(), window, outside, 0,
                new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.Other), KeyModifiers.None));
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(StandardCursorType.None.ToString(), wheel.Cursor?.ToString());
            Assert.Contains(":dragging", wheel.Classes);

            // It comes back with the release, which puts the cursor on the indicator.
            window.MouseUp(outside, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(before, wheel.Cursor?.ToString());
            Assert.DoesNotContain(":dragging", wheel.Classes);
            Assert.Single(spy.Placed);
        }
    }
}
