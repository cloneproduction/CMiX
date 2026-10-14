using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls;
using CMiX.UI.Controls.ColorPicker;
using CMiX.UI.Controls.Number;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // A drag ends when the pointer is taken away without a release, for example when the popup of the control
    // closes. The control must not go on following the mouse, and a popup must not close under a drag.
    public class CaptureLostTests
    {
        private static Window Show(Control control, double width = 400, double height = 200)
        {
            var window = new Window { Content = control, Width = width, Height = height };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            // The headless renderer cannot hit-test through a geometry clip.
            foreach (var visual in window.GetVisualDescendants().Where(v => v.Clip != null))
                visual.Clip = null;
            Dispatcher.UIThread.RunJobs();
            return window;
        }

        private static Pointer NewPointer() => new(Pointer.GetNextFreeId(), PointerType.Mouse, true);

        private static void Press(Control target, Pointer pointer, Point inWindow, Window window)
        {
            target.RaiseEvent(new PointerPressedEventArgs(target, pointer, window, inWindow, 0,
                new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.LeftButtonPressed), KeyModifiers.None)
            { RoutedEvent = InputElement.PointerPressedEvent });
            Dispatcher.UIThread.RunJobs();
        }

        private static void Move(Control target, Pointer pointer, Point inWindow, Window window)
        {
            target.RaiseEvent(new PointerEventArgs(InputElement.PointerMovedEvent, target, pointer, window, inWindow, 0,
                new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.Other), KeyModifiers.None));
            Dispatcher.UIThread.RunJobs();
        }

        private static void Release(Control target, Pointer pointer, Point inWindow, Window window)
        {
            target.RaiseEvent(new PointerReleasedEventArgs(target, pointer, window, inWindow, 0,
                new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.LeftButtonReleased), KeyModifiers.None, MouseButton.Left));
            Dispatcher.UIThread.RunJobs();
        }

        // The real pointer of the headless mouse, taken from the press that reaches the control.
        private static Func<IPointer?> WatchPointer(Control control)
        {
            IPointer? pointer = null;
            control.AddHandler(InputElement.PointerPressedEvent, (s, e) => pointer = e.Pointer, RoutingStrategies.Tunnel, handledEventsToo: true);
            return () => pointer;
        }

        // ---------------------------------------------------------------- the slider

        [AvaloniaFact]
        public void ASlider_DraggedWithTheButtonDown_ChangesTheValue()
        {
            var slider = new SliderCMiX { Minimum = 0, Maximum = 1, Value = 0.5, Width = 300, Height = 30, IsEditable = false };
            var window = Show(slider);
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
        public void ASlider_WhoseCaptureIsLost_NoLongerFollowsTheMouse()
        {
            var slider = new SliderCMiX { Minimum = 0, Maximum = 1, Value = 0.5, Width = 300, Height = 30, IsEditable = false };
            var window = Show(slider);
            var pointer = WatchPointer(slider);
            var center = slider.TranslatePoint(new Point(150, 15), window)!.Value;

            window.MouseMove(center);
            Dispatcher.UIThread.RunJobs();
            window.MouseDown(center, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            Assert.NotNull(pointer()!.Captured);

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
            Assert.False(slider.Classes.Contains(":pressed"));
            Assert.NotEqual(StandardCursorType.None, slider.Cursor?.GetType() == typeof(Cursor) ? StandardCursorType.Arrow : StandardCursorType.Arrow);
        }

        // ---------------------------------------------------------------- the color wheel

        private static (ColorPickerWheel Wheel, Window Window, List<Avalonia.Media.Color> Colors) ShowWheel()
        {
            // The wheel draws its cursor from its theme, as ColorPickerFull sets it.
            var wheel = new ColorPickerWheel
            {
                Width = 200,
                Height = 200,
                Theme = (ControlTheme)Application.Current!.FindResource("ColorPickerWheel")!,
                SelectedColor = Avalonia.Media.Colors.Red,
            };
            var window = Show(wheel, 400, 400);
            var colors = new List<Avalonia.Media.Color>();
            wheel.ColorChanged += (s, e) => colors.Add(e.Color);
            return (wheel, window, colors);
        }

        [AvaloniaFact]
        public void AWheel_DraggedWithTheButtonDown_ChangesTheColor()
        {
            var (wheel, window, colors) = ShowWheel();
            var pointer = NewPointer();
            var at = wheel.TranslatePoint(new Point(150, 100), window)!.Value;

            Press(wheel, pointer, at, window);
            Move(wheel, pointer, new Point(at.X - 60, at.Y + 40), window);
            Move(wheel, pointer, new Point(at.X - 100, at.Y - 40), window);

            Assert.NotEmpty(colors);
        }

        [AvaloniaFact]
        public void AWheel_WhoseCaptureIsLost_NoLongerFollowsTheMouse()
        {
            var (wheel, window, colors) = ShowWheel();
            var pointer = NewPointer();
            var at = wheel.TranslatePoint(new Point(150, 100), window)!.Value;

            Press(wheel, pointer, at, window);
            pointer.Capture(null);
            Dispatcher.UIThread.RunJobs();
            var count = colors.Count;

            Move(wheel, pointer, new Point(at.X - 60, at.Y + 40), window);
            Move(wheel, pointer, new Point(at.X - 100, at.Y - 40), window);

            Assert.Equal(count, colors.Count);
        }

        // ---------------------------------------------------------------- the resize strip

        private static double ListHeight(ListBoxCMiX list) =>
            list.GetVisualDescendants().OfType<ScrollViewer>().First().GetVisualAncestors().OfType<Border>().First(b => b.Name == "Bd").Height;

        [AvaloniaFact]
        public void AListBoxCMiX_WhoseCaptureIsLost_StopsResizing()
        {
            var list = new ListBoxCMiX { MinItemCount = Tokens.ListVisibleItems, ItemsSource = Enumerable.Range(1, 30).Select(i => "Item " + i).ToList() };
            var window = Show(list, 400, 1500);
            var strip = list.GetVisualDescendants().OfType<Border>().Single(b => b.Name == "resizeBorder");
            var pointer = WatchPointer(strip);
            var start = strip.TranslatePoint(new Point(strip.Bounds.Width / 2, strip.Bounds.Height / 2), window)!.Value;
            var before = ListHeight(list);

            window.MouseMove(start);
            window.MouseDown(start, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            Assert.NotNull(pointer()!.Captured);

            pointer()!.Capture(null);
            Dispatcher.UIThread.RunJobs();

            // A move over the strip, as when the mouse comes back to it.
            Move(strip, NewPointer(), new Point(start.X, start.Y + 200), window);

            Assert.Equal(before, ListHeight(list));
        }

        // ---------------------------------------------------------------- the popup

        private static (Window Window, ColorSelector Selector, Popup Popup) ShowSelectorWithOpenPopup()
        {
            var selector = new ColorSelector
            {
                DataContext = new StringValue(),
                Width = 300,
                Height = 40,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
            };
            var window = new Window { Content = selector, Width = 1000, Height = 600 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var swatch = selector.FindControl<ToggleButton>("PopupToggle")!;
            var popup = selector.FindControl<Popup>("colorPickerPopup")!;
            var top = swatch.TranslatePoint(new Point(0, 0), window)!.Value;
            var click = new Point(top.X + swatch.Bounds.Width * 0.05, top.Y + swatch.Bounds.Height / 2);
            window.MouseMove(click);
            window.MouseDown(click, MouseButton.Left);
            window.MouseUp(click, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            Assert.True(popup.IsOpen);
            return (window, selector, popup);
        }

        private static readonly Point Far = new(950, 20);

        [AvaloniaFact]
        public void WhileAControlOfThePopupHoldsThePointer_ThePopupStaysOpenFarAway()
        {
            var (window, _, popup) = ShowSelectorWithOpenPopup();
            var pointer = NewPointer();
            pointer.Capture(popup.Child);

            Move(window, pointer, Far, window);

            Assert.True(popup.IsOpen);
        }

        [AvaloniaFact]
        public void WhenTheButtonIsReleasedFarAway_ThePopupClosesOnTheNextMove()
        {
            var (window, _, popup) = ShowSelectorWithOpenPopup();
            var pointer = NewPointer();
            pointer.Capture(popup.Child);
            Move(window, pointer, Far, window);

            // The release itself closes nothing: the control that held the pointer ends its drag first.
            pointer.Capture(null);
            Release(window, pointer, Far, window);
            Assert.True(popup.IsOpen);

            Move(window, pointer, new Point(Far.X - 5, Far.Y), window);
            Assert.False(popup.IsOpen);
        }

        [AvaloniaFact]
        public void WhenTheButtonIsReleasedNearThePopup_ThePopupStaysOpen()
        {
            var (window, selector, popup) = ShowSelectorWithOpenPopup();
            var pointer = NewPointer();
            pointer.Capture(popup.Child);
            var swatch = selector.FindControl<ToggleButton>("PopupToggle")!;
            var near = swatch.TranslatePoint(new Point(swatch.Bounds.Width / 2, swatch.Bounds.Height / 2), window)!.Value;
            Move(window, pointer, Far, window);

            pointer.Capture(null);
            Release(window, pointer, near, window);

            Assert.True(popup.IsOpen);
        }

        [AvaloniaFact]
        public void WithoutAHeldPointer_ThePopupStillClosesWhenThePointerGoesAway()
        {
            var (window, _, popup) = ShowSelectorWithOpenPopup();

            Move(window, NewPointer(), Far, window);

            Assert.False(popup.IsOpen);
        }

        // ---------------------------------------------------------------- the cursor goes back on the slider

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

        [AvaloniaTheory]
        [InlineData(40, 0)]
        [InlineData(200, 150)]
        public void APlainSlider_PutsTheCursorBackOnItsThumb(double dx, double dy)
        {
            using var spy = new CursorSpy();
            var slider = new SliderCMiX { Minimum = 0, Maximum = 1, Value = 0.5, Width = 300, Height = 30, IsEditable = false };
            var window = Show(slider, 700, 400);
            var center = slider.TranslatePoint(new Point(150, 15), window)!.Value;

            DragAndRelease(window, slider, new Point(center.X + dx, center.Y + dy));

            Assert.Single(spy.Placed);
            AssertOnTheSlider(slider, spy.Placed[0]);
        }

        [AvaloniaTheory]
        [InlineData(30, 0)]
        [InlineData(420, -150)]
        public void ASliderInThePopup_PutsTheCursorBackOnItsThumb_AlsoAfterALongDrag(double dx, double dy)
        {
            using var spy = new CursorSpy();
            var (window, _, popup) = ShowSelectorWithOpenPopup();
            foreach (var visual in window.GetVisualDescendants().Where(v => v.Clip != null))
                visual.Clip = null;
            Dispatcher.UIThread.RunJobs();
            var slider = HorizontalSliderUnderTheMouse(window, popup);
            var center = slider.TranslatePoint(new Point(slider.Bounds.Width / 2, slider.Bounds.Height / 2), window)!.Value;

            DragAndRelease(window, slider, new Point(center.X + dx, center.Y + dy));

            Assert.Single(spy.Placed);
            AssertOnTheSlider(slider, spy.Placed[0]);
            // The cursor is back next to the popup, so the next move keeps it open.
            Assert.True(popup.IsOpen);
        }

        // ---------------------------------------------------------------- the cursor is not put on the edge

        // A press starts a drag only when the pointer is over the track. This moves the mouse to the place where the
        // cursor was put back, as the person does, and asks the track.
        private static bool OverTheTrackAt(Window window, SliderCMiX slider, Point placedOnScreen)
        {
            var at = window.PointToClient(new PixelPoint((int)placedOnScreen.X, (int)placedOnScreen.Y));
            window.MouseMove(at);
            Dispatcher.UIThread.RunJobs();

            return slider.GetVisualDescendants().OfType<Border>().First(b => b.Name == "sliderBorder").IsPointerOver;
        }

        // The color picker stacks its modes (RGB, HSV and others) on top of each other, and only the visible mode takes
        // the mouse. This is the horizontal slider that is really under the pointer at its center.
        private static SliderCMiX HorizontalSliderUnderTheMouse(Window window, Popup popup)
        {
            foreach (var slider in popup.Child!.GetVisualDescendants().OfType<SliderCMiX>().Where(s => s.IsEffectivelyVisible && s.Orientation == Orientation.Horizontal))
            {
                var center = slider.TranslatePoint(new Point(slider.Bounds.Width / 2, slider.Bounds.Height / 2), window)!.Value;
                window.MouseMove(center);
                Dispatcher.UIThread.RunJobs();
                if (slider.GetVisualDescendants().OfType<Border>().First(b => b.Name == "sliderBorder").IsPointerOver)
                    return slider;
            }

            throw new InvalidOperationException("No horizontal slider of the popup is under the mouse.");
        }

        private static SliderCMiX HorizontalSlider() => new()
        {
            Minimum = 0, Maximum = 1, Value = 0.5, Width = 300, Height = 30, IsEditable = false,
        };

        // The slider next to the color wheel: vertical, the maximum at the top.
        private static SliderCMiX BrightnessSlider() => new()
        {
            Theme = (ControlTheme)Application.Current!.FindResource("SliderCMiXBrightness")!,
            Minimum = 0, Maximum = 100, Value = 50, Width = 16, Height = 200, IsEditable = false,
        };

        [AvaloniaTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void AHorizontalSlider_DraggedToAnEnd_CanBePressedAgainWhereTheCursorIsPutBack(bool toMaximum)
        {
            using var spy = new CursorSpy();
            var slider = HorizontalSlider();
            var window = Show(slider, 1500, 1200);
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
        public void AVerticalSlider_DraggedToAnEnd_CanBePressedAgainWhereTheCursorIsPutBack(bool toMaximum)
        {
            using var spy = new CursorSpy();
            var slider = BrightnessSlider();
            var window = Show(slider, 1500, 1200);
            var center = slider.TranslatePoint(new Point(8, 100), window)!.Value;

            // The maximum is at the top.
            DragAndRelease(window, slider, new Point(center.X, center.Y + (toMaximum ? -500 : 500)));

            Assert.Equal(toMaximum ? 100.0 : 0.0, slider.Value);
            Assert.True(OverTheTrackAt(window, slider, spy.Placed[^1]));
        }

        [AvaloniaFact]
        public void TheValueOfASlider_DoesNotTakeThePress()
        {
            var slider = HorizontalSlider();
            var window = Show(slider, 1500, 1200);
            var label = slider.GetVisualDescendants().OfType<Label>().First(l => l.Name == "textDisplay");
            var onTheLabel = label.TranslatePoint(new Point(label.Bounds.Width - 12, label.Bounds.Height / 2), window)!.Value;

            window.MouseMove(onTheLabel);
            Dispatcher.UIThread.RunJobs();

            Assert.False(label.IsHitTestVisible);
            Assert.True(slider.GetVisualDescendants().OfType<Border>().First(b => b.Name == "sliderBorder").IsPointerOver);
        }

        [AvaloniaFact]
        public void InTheColorPickerPopup_AfterADragToTheMaximum_TheSliderCanBePressedAgain()
        {
            using var spy = new CursorSpy();
            var (window, _, popup) = ShowSelectorWithOpenPopup();
            foreach (var visual in window.GetVisualDescendants().Where(v => v.Clip != null))
                visual.Clip = null;
            Dispatcher.UIThread.RunJobs();
            var slider = HorizontalSliderUnderTheMouse(window, popup);
            var center = slider.TranslatePoint(new Point(slider.Bounds.Width / 2, slider.Bounds.Height / 2), window)!.Value;

            DragAndRelease(window, slider, new Point(center.X + 300, center.Y));

            // The cursor is at the right end of the slider, and the popup is still open.
            var right = slider.PointToScreen(new Point(slider.Bounds.Width, slider.Bounds.Height / 2));
            Assert.InRange(spy.Placed[^1].X, right.X - 12, right.X);
            Assert.True(popup.IsOpen);
            Assert.True(OverTheTrackAt(window, slider, spy.Placed[^1]));
        }
    }
}
