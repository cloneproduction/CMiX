using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.Number;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // A drag ends when the pointer is taken away without a release, for example when the popup of the control
    // closes, or when Windows gives the mouse to another window. The control must not go on following the mouse.
    public class CaptureLostTests
    {
        private static SliderCMiX NewSlider() => new() { Minimum = 0, Maximum = 1, Value = 0.5, Width = 300, Height = 30, IsEditable = false };

        private static (DragValue Drag, Window Window, Border Border, Point Center) ShowDragValue()
        {
            var drag = new DragValue { Width = 300, Height = 30, Value = 0 };
            var window = TestWindow.ShowWithoutClips(drag, 400, 200);
            var border = Parts.Find<Border>(drag, "borderValueDisplay");
            var center = border.TranslatePoint(new Point(border.Bounds.Width / 2, border.Bounds.Height / 2), window)!.Value;
            return (drag, window, border, center);
        }

        // The slider.

        [AvaloniaFact]
        public void Drag_ASliderWithTheButtonDown_ChangesTheValue()
        {
            var slider = NewSlider();
            var window = TestWindow.ShowWithoutClips(slider, 400, 200);
            var center = slider.TranslatePoint(new Point(150, 15), window)!.Value;

            window.MouseMove(center);
            Dispatcher.UIThread.RunJobs();
            window.MouseDown(center, MouseButton.Left);
            for (var i = 1; i <= 5; i++)
            {
                window.MouseMove(new Point(center.X + 15 * i, center.Y));
                Dispatcher.UIThread.RunJobs();
            }
            window.MouseUp(new Point(center.X + 75, center.Y), MouseButton.Left);

            Assert.True(slider.Value > 0.5);
        }

        [AvaloniaFact]
        public void CaptureLost_DuringASliderDrag_StopsFollowingTheMouse()
        {
            var slider = NewSlider();
            var window = TestWindow.ShowWithoutClips(slider, 400, 200);
            var pointer = PointerInput.WatchPressedPointer(slider);
            var center = slider.TranslatePoint(new Point(150, 15), window)!.Value;

            window.MouseMove(center);
            Dispatcher.UIThread.RunJobs();
            window.MouseDown(center, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            Assert.NotNull(pointer()!.Captured);
            window.MouseMove(new Point(center.X + 15, center.Y));
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(StandardCursorType.None.ToString(), slider.Cursor?.ToString());

            // The popup of the slider closes: the pointer is taken away, and no release arrives.
            pointer()!.Capture(null);
            Dispatcher.UIThread.RunJobs();
            var value = slider.Value;

            for (var i = 1; i <= 6; i++)
            {
                window.MouseMove(new Point(center.X + 15 * i, center.Y));
                Dispatcher.UIThread.RunJobs();
            }

            Assert.Equal(value, slider.Value);
            Assert.DoesNotContain(":pressed", slider.Classes);
            Assert.NotEqual(StandardCursorType.None.ToString(), slider.Cursor?.ToString());
        }

        [AvaloniaFact]
        public void MouseTaken_DuringASliderDrag_EndsTheDragOnTheNextMove()
        {
            using var spy = new CursorSpy();
            var slider = NewSlider();
            var window = TestWindow.ShowWithoutClips(slider, 400, 200);
            var pointer = PointerInput.WatchPressedPointer(slider);
            var center = slider.TranslatePoint(new Point(150, 15), window)!.Value;

            window.MouseMove(center);
            window.MouseDown(center, MouseButton.Left);
            window.MouseMove(new Point(center.X + 10, center.Y));
            Dispatcher.UIThread.RunJobs();
            var value = slider.Value;

            using (new MouseTakenByAnotherWindow())
            {
                window.MouseMove(new Point(center.X + 40, center.Y));
                Dispatcher.UIThread.RunJobs();
            }
            window.MouseMove(new Point(center.X + 80, center.Y));
            Dispatcher.UIThread.RunJobs();

            Assert.Null(pointer()!.Captured);
            Assert.Equal(value, slider.Value);
            Assert.DoesNotContain(":pressed", slider.Classes);
            Assert.Empty(spy.Placed);
        }

        // The drag value.

        [AvaloniaFact]
        public void CaptureLost_DuringADragValueDrag_StopsFollowingTheMouse()
        {
            using var spy = new CursorSpy();
            var (drag, window, border, center) = ShowDragValue();
            var pointer = PointerInput.WatchPressedPointer(border);

            window.MouseMove(center);
            window.MouseDown(center, MouseButton.Left);
            window.MouseMove(new Point(center.X + 20, center.Y));
            window.MouseMove(new Point(center.X + 40, center.Y));
            Dispatcher.UIThread.RunJobs();
            Assert.NotEqual(0, drag.Value);
            Assert.Equal(StandardCursorType.None.ToString(), border.Cursor?.ToString());

            // Windows gives the mouse to another window: the release goes there.
            pointer()!.Capture(null);
            Dispatcher.UIThread.RunJobs();
            var value = drag.Value;

            // The mouse comes back over the field with no button down.
            for (var i = 1; i <= 6; i++)
            {
                window.MouseMove(new Point(center.X + 20 - 10 * i, center.Y));
                Dispatcher.UIThread.RunJobs();
            }
            window.MouseUp(new Point(center.X - 40, center.Y), MouseButton.Left);
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(value, drag.Value);
            Assert.NotEqual(StandardCursorType.None.ToString(), border.Cursor?.ToString());
            Assert.DoesNotContain(":pressed", drag.Classes);
            Assert.Empty(spy.Placed);
            Assert.False(drag.IsEditing);
        }

        [AvaloniaFact]
        public void Click_ADragValueWithoutAMove_StartsTheEdit()
        {
            var (drag, window, _, center) = ShowDragValue();

            window.MouseMove(center);
            window.MouseDown(center, MouseButton.Left);
            window.MouseUp(center, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();

            Assert.True(drag.IsEditing);
        }

        [AvaloniaFact]
        public void MouseTaken_DuringADragValueDrag_EndsTheDragOnTheNextMove()
        {
            using var spy = new CursorSpy();
            var (drag, window, border, center) = ShowDragValue();

            window.MouseMove(center);
            window.MouseDown(center, MouseButton.Left);
            window.MouseMove(new Point(center.X + 20, center.Y));
            window.MouseMove(new Point(center.X + 40, center.Y));
            Dispatcher.UIThread.RunJobs();
            var value = drag.Value;

            using (new MouseTakenByAnotherWindow())
            {
                window.MouseMove(new Point(center.X + 60, center.Y));
                Dispatcher.UIThread.RunJobs();
            }
            window.MouseMove(new Point(center.X + 80, center.Y));
            window.MouseUp(new Point(center.X + 80, center.Y), MouseButton.Left);
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(value, drag.Value);
            Assert.NotEqual(StandardCursorType.None.ToString(), border.Cursor?.ToString());
            Assert.Empty(spy.Placed);
            Assert.False(drag.IsEditing);
        }

        // The color wheel.

        [AvaloniaFact]
        public void Drag_AWheelWithTheButtonDown_ChangesTheColor()
        {
            var (wheel, window, colors) = ColorWheelHost.Show();
            var pointer = PointerInput.NewPointer();
            var at = wheel.TranslatePoint(new Point(150, 100), window)!.Value;

            PointerInput.Press(wheel, pointer, at, window);
            PointerInput.Move(wheel, pointer, new Point(at.X - 60, at.Y + 40), window);
            PointerInput.Move(wheel, pointer, new Point(at.X - 100, at.Y - 40), window);

            Assert.NotEmpty(colors);
        }

        [AvaloniaFact]
        public void Press_TheCenterOfAWheel_KeepsAValidHue()
        {
            var (wheel, window, _) = ColorWheelHost.Show();
            var center = wheel.TranslatePoint(new Point(wheel.Width / 2, wheel.Height / 2), window)!.Value;

            PointerInput.Press(wheel, PointerInput.NewPointer(), center, window);

            Assert.True(double.IsFinite(wheel.Color.HSV_H));
            Assert.Equal(0, wheel.Color.HSV_S);
        }

        [AvaloniaFact]
        public void CaptureLost_DuringAWheelDrag_StopsFollowingTheMouse()
        {
            var (wheel, window, colors) = ColorWheelHost.Show();
            var pointer = PointerInput.NewPointer();
            var at = wheel.TranslatePoint(new Point(150, 100), window)!.Value;

            PointerInput.Press(wheel, pointer, at, window);
            pointer.Capture(null);
            Dispatcher.UIThread.RunJobs();
            var count = colors.Count;

            PointerInput.Move(wheel, pointer, new Point(at.X - 60, at.Y + 40), window);
            PointerInput.Move(wheel, pointer, new Point(at.X - 100, at.Y - 40), window);

            Assert.Equal(count, colors.Count);
        }

        [AvaloniaFact]
        public void CaptureLost_DuringAWheelDrag_LeavesTheCursorInPlace()
        {
            using var spy = new CursorSpy();
            var (wheel, window, _) = ColorWheelHost.Show();
            var pointer = PointerInput.NewPointer();

            PointerInput.Press(wheel, pointer, wheel.TranslatePoint(new Point(150, 100), window)!.Value, window);
            pointer.Capture(null);
            Dispatcher.UIThread.RunJobs();

            Assert.Empty(spy.Placed);
        }

        [AvaloniaFact]
        public void CaptureLost_DuringAWheelDrag_ShowsTheCursorAgain()
        {
            var (wheel, window, _) = ColorWheelHost.Show();
            var pointer = PointerInput.NewPointer();
            var before = wheel.Cursor?.ToString();

            PointerInput.Press(wheel, pointer, wheel.TranslatePoint(new Point(150, 100), window)!.Value, window);
            Assert.Equal(StandardCursorType.None.ToString(), wheel.Cursor?.ToString());

            pointer.Capture(null);
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(before, wheel.Cursor?.ToString());
            Assert.DoesNotContain(":dragging", wheel.Classes);
        }

        [AvaloniaFact]
        public void MouseTaken_DuringAWheelDrag_EndsTheDragOnTheNextMove()
        {
            var (wheel, window, colors) = ColorWheelHost.Show();
            var pointer = PointerInput.NewPointer();
            var at = wheel.TranslatePoint(new Point(150, 100), window)!.Value;

            PointerInput.Press(wheel, pointer, at, window);
            var count = colors.Count;

            using (new MouseTakenByAnotherWindow())
                PointerInput.Move(wheel, pointer, new Point(at.X - 60, at.Y + 40), window);
            PointerInput.Move(wheel, pointer, new Point(at.X - 100, at.Y - 40), window);

            Assert.Null(pointer.Captured);
            Assert.Equal(count, colors.Count);
            Assert.DoesNotContain(":dragging", wheel.Classes);
        }
    }
}
