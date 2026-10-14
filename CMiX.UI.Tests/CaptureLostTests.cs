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

        private static void Move(Control target, Pointer pointer, Point inWindow, Window window, KeyModifiers modifiers = KeyModifiers.None)
        {
            target.RaiseEvent(new PointerEventArgs(InputElement.PointerMovedEvent, target, pointer, window, inWindow, 0,
                new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.Other), modifiers));
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
            Minimum = 0, Maximum = 1, Value = 0.5, Width = 300, Height = 30, IsEditable = false, LargeChange = 1,
        };

        // The slider next to the color wheel: vertical, the maximum at the top.
        private static SliderCMiX BrightnessSlider() => new()
        {
            Theme = (ControlTheme)Application.Current!.FindResource("SliderCMiXBrightness")!,
            Minimum = 0, Maximum = 100, Value = 50, Width = 16, Height = 200, IsEditable = false, LargeChange = 1,
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
            slider.LargeChange = 1;
            var center = slider.TranslatePoint(new Point(slider.Bounds.Width / 2, slider.Bounds.Height / 2), window)!.Value;

            DragAndRelease(window, slider, new Point(center.X + 300, center.Y));

            // The cursor is at the right end of the slider, and the popup is still open.
            var right = slider.PointToScreen(new Point(slider.Bounds.Width, slider.Bounds.Height / 2));
            Assert.InRange(spy.Placed[^1].X, right.X - 12, right.X);
            Assert.True(popup.IsOpen);
            Assert.True(OverTheTrackAt(window, slider, spy.Placed[^1]));
        }

        // ---------------------------------------------------------------- the cursor goes back on the wheel

        // Where the indicator of the color is, on the screen: the same formula as the wheel uses.
        private static Point IndicatorOnScreen(ColorPickerWheel wheel)
        {
            var ellipse = wheel.GetVisualDescendants().OfType<Avalonia.Controls.Shapes.Ellipse>().First(e => e.Name == "PART_CursorEllipse");
            var angle = wheel.Color.HSV_H * Math.PI / 180;
            var radius = (wheel.Height - ellipse.Height) / 2 * wheel.Color.HSV_S;
            var location = new Point(wheel.Width / 2 + Math.Cos(angle) * radius, wheel.Height / 2 - Math.Sin(angle) * radius);
            var onScreen = wheel.PointToScreen(location);

            return new Point(onScreen.X, onScreen.Y);
        }

        [AvaloniaTheory]
        [InlineData(120, 0)]
        [InlineData(450, -80)]
        [InlineData(-250, 300)]
        public void AWheel_PutsTheCursorBackOnTheIndicator_AlsoAfterALongDragOutside(double dx, double dy)
        {
            using var spy = new CursorSpy();
            var (wheel, window, colors) = ShowWheel();
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
        public void AWheel_CanBeDraggedAgainFromWhereTheCursorIsPutBack()
        {
            using var spy = new CursorSpy();
            var (wheel, window, colors) = ShowWheel();
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
        public void APressExactlyAtTheCenterOfAWheel_KeepsAValidHue()
        {
            var (wheel, window, _) = ShowWheel();
            var pointer = NewPointer();
            var center = wheel.TranslatePoint(new Point(wheel.Width / 2, wheel.Height / 2), window)!.Value;

            Press(wheel, pointer, center, window);

            Assert.True(double.IsFinite(wheel.Color.HSV_H));
            Assert.Equal(0, wheel.Color.HSV_S);
        }

        [AvaloniaFact]
        public void AWheel_WhoseCaptureIsLost_DoesNotMoveTheCursor()
        {
            using var spy = new CursorSpy();
            var (wheel, window, _) = ShowWheel();
            var pointer = NewPointer();
            var at = wheel.TranslatePoint(new Point(150, 100), window)!.Value;

            Press(wheel, pointer, at, window);
            pointer.Capture(null);
            Dispatcher.UIThread.RunJobs();

            Assert.Empty(spy.Placed);
        }

        // ---------------------------------------------------------------- the cursor stays hidden for the whole drag

        private static string? CursorName(ColorPickerWheel wheel) => wheel.Cursor?.ToString();

        [AvaloniaFact]
        public void AWheel_HidesTheCursorForTheWholeDrag_AlsoWhenThePointerLeavesIt()
        {
            using var spy = new CursorSpy();
            var (wheel, window, _) = ShowWheel();
            var origin = wheel.TranslatePoint(new Point(0, 0), window)!.Value;
            var start = new Point(origin.X + 150, origin.Y + 100);
            var outside = new Point(origin.X + 330, origin.Y + 100);
            var before = CursorName(wheel);

            window.MouseMove(start);
            window.MouseDown(start, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(StandardCursorType.None.ToString(), CursorName(wheel));
            Assert.Contains(":dragging", wheel.Classes);

            // The pointer leaves the wheel and the control says so, while the drag goes on.
            window.MouseMove(outside);
            wheel.RaiseEvent(new PointerEventArgs(InputElement.PointerExitedEvent, wheel, NewPointer(), window, outside, 0,
                new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.Other), KeyModifiers.None));
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(StandardCursorType.None.ToString(), CursorName(wheel));
            Assert.Contains(":dragging", wheel.Classes);

            // It comes back with the release, which puts the cursor on the indicator.
            window.MouseUp(outside, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(before, CursorName(wheel));
            Assert.DoesNotContain(":dragging", wheel.Classes);
            Assert.Single(spy.Placed);
        }

        [AvaloniaFact]
        public void AWheel_ShowsTheCursorAgain_WhenItsCaptureIsLost()
        {
            var (wheel, window, _) = ShowWheel();
            var pointer = NewPointer();
            var before = CursorName(wheel);

            Press(wheel, pointer, wheel.TranslatePoint(new Point(150, 100), window)!.Value, window);
            Assert.Equal(StandardCursorType.None.ToString(), CursorName(wheel));

            pointer.Capture(null);
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(before, CursorName(wheel));
            Assert.DoesNotContain(":dragging", wheel.Classes);
        }

        // ---------------------------------------------------------------- Shift slows the movement

        // The saturation of the wheel after a press at the given local point and the moves that follow.
        private static double SaturationAfter(Point pressAt, params (Point At, KeyModifiers Modifiers)[] moves)
        {
            var (wheel, window, _) = ShowWheel();
            var pointer = NewPointer();
            var origin = wheel.TranslatePoint(new Point(0, 0), window)!.Value;
            Point InWindow(Point local) => new(origin.X + local.X, origin.Y + local.Y);

            Press(wheel, pointer, InWindow(pressAt), window);
            foreach (var move in moves)
                Move(wheel, pointer, InWindow(move.At), window, move.Modifiers);

            return wheel.Color.HSV_S;
        }

        [AvaloniaFact]
        public void AWheel_WithShift_MovesTheIndicatorByAFractionOfTheMouse()
        {
            var press = new Point(130, 100);
            var to = new Point(170, 100);
            var start = SaturationAfter(press);

            var normal = SaturationAfter(press, (to, KeyModifiers.None)) - start;
            var fine = SaturationAfter(press, (to, KeyModifiers.Shift)) - start;

            Assert.True(normal > 0.3);
            Assert.InRange(fine / normal, DragEditHelperFineSpeed * 0.9, DragEditHelperFineSpeed * 1.1);
        }

        [AvaloniaFact]
        public void AWheel_DoesNotJump_WhenShiftIsLetGo()
        {
            var (wheel, window, _) = ShowWheel();
            var pointer = NewPointer();
            var origin = wheel.TranslatePoint(new Point(0, 0), window)!.Value;
            Point InWindow(double x, double y) => new(origin.X + x, origin.Y + y);

            Press(wheel, pointer, InWindow(130, 100), window);
            var start = wheel.Color.HSV_S;
            Move(wheel, pointer, InWindow(170, 100), window, KeyModifiers.Shift);
            var slowed = wheel.Color.HSV_S;
            Assert.InRange(slowed - start, 0, 0.01);

            // Shift is let go and the mouse has not moved: the indicator stays where it is.
            Move(wheel, pointer, InWindow(170, 100), window);
            Assert.Equal(slowed, wheel.Color.HSV_S, 9);

            // And it follows the mouse at full speed again.
            Move(wheel, pointer, InWindow(180, 100), window);
            Assert.Equal(slowed + 10 / ((wheel.Height - 12) / 2), wheel.Color.HSV_S, 3);
        }

        private const double DragEditHelperFineSpeed = 0.01;

        private static double SliderGainWith(RawInputModifiers modifiers)
        {
            var slider = new SliderCMiX { Minimum = 0, Maximum = 1, Value = 0.5, Width = 300, Height = 30, IsEditable = false };
            var window = Show(slider, 1500, 400);
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
        public void ASlider_MovesAsSlowAsADragValue()
        {
            var normal = SliderGainWith(RawInputModifiers.None);
            var fine = SliderGainWith(RawInputModifiers.Shift);

            // 60 pixels at 0.001 a pixel, and the small step of a DragValue with Shift.
            Assert.InRange(normal, 0.059, 0.061);
            Assert.InRange(fine / normal, 0.099, 0.101);
        }
    }
}
