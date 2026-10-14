using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.ColorPicker;
using CMiX.UI.Controls.Number;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // A drag goes on when the cursor reaches the edge of the screen: the cursor is placed near the opposite edge,
    // and the control sees one continuous movement, like the continuous grab of Blender.
    public class DragCursorWrapTests
    {
        private const double FineSpeed = DragEditHelper.FineSpeed;

        // A screen as big as the window of the test.
        private sealed class FakeScreen : IDisposable
        {
            private readonly Func<Visual, PixelPoint, PixelRect?> previous = DragEditHelper.ScreenBoundsAt;

            public FakeScreen(PixelRect bounds) => DragEditHelper.ScreenBoundsAt = (_, _) => bounds;

            public void Dispose() => DragEditHelper.ScreenBoundsAt = previous;
        }

        // A Windows that moves the cursor where the app places it.
        private sealed class FakeMouse
        {
            private readonly Window window;
            private readonly Control target;
            private readonly IPointer pointer;
            private readonly CursorSpy spy;

            public FakeMouse(Window window, Control target, IPointer pointer, CursorSpy spy, PixelRect bounds, PixelPoint cursor)
            {
                this.window = window;
                this.target = target;
                this.pointer = pointer;
                this.spy = spy;
                Bounds = bounds;
                Cursor = cursor;
            }

            public PixelRect Bounds { get; }
            public PixelPoint Cursor { get; private set; }

            // How far the mouse really moved, without the jumps of the placements.
            public double MovedX { get; private set; }
            public double MovedY { get; private set; }

            // Moves that were queued before a placement, and still come from the old side.
            public int StaleMoves { get; set; }

            // False: the placement does not work, and the cursor stays where it is.
            public bool HonorsPlacement { get; set; } = true;

            // False: Windows sends no move for the placement itself. The next move comes from the new place.
            public bool ReportsPlacement { get; set; } = true;

            public int Placements => spy.Placed.Count;

            public void Move(int dx, int dy, KeyModifiers modifiers = KeyModifiers.Shift)
            {
                var old = Cursor;
                var moved = new PixelPoint(
                    Math.Clamp(old.X + dx, Bounds.X, Bounds.Right - 1),
                    Math.Clamp(old.Y + dy, Bounds.Y, Bounds.Bottom - 1));
                MovedX += moved.X - old.X;
                MovedY += moved.Y - old.Y;
                Cursor = moved;

                var before = spy.Placed.Count;
                Raise(moved, modifiers);

                if (spy.Placed.Count > before && HonorsPlacement)
                {
                    for (var i = 0; i < StaleMoves; i++)
                        Raise(moved, modifiers);

                    var to = spy.Placed[^1];
                    Cursor = new PixelPoint((int)to.X, (int)to.Y);
                    if (ReportsPlacement)
                        Raise(Cursor, modifiers);
                }
            }

            private void Raise(PixelPoint screen, KeyModifiers modifiers)
            {
                target.RaiseEvent(new PointerEventArgs(InputElement.PointerMovedEvent, target, pointer, window, window.PointToClient(screen), 0,
                    new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.Other), modifiers));
                Dispatcher.UIThread.RunJobs();
            }
        }

        private static Window Show(Control control)
        {
            var window = new Window { Content = control, Width = 400, Height = 400 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            foreach (var visual in window.GetVisualDescendants().Where(v => v.Clip != null))
                visual.Clip = null;
            Dispatcher.UIThread.RunJobs();
            return window;
        }

        private static PixelRect ScreenOf(Window window)
        {
            var topLeft = window.PointToScreen(new Point(0, 0));
            return new PixelRect(topLeft.X, topLeft.Y, 400, 400);
        }

        // A wheel pressed at one pixel from its center. With Shift, the indicator is at (1 + 0.01 * moved) / radius.
        private static (ColorPickerWheel Wheel, FakeMouse Mouse, CursorSpy Spy, FakeScreen Screen) WheelDrag()
        {
            var spy = new CursorSpy();
            var wheel = new ColorPickerWheel
            {
                Width = 200,
                Height = 200,
                Theme = (ControlTheme)Application.Current!.FindResource("ColorPickerWheel")!,
                SelectedColor = Avalonia.Media.Colors.Red,
            };
            var window = Show(wheel);
            var bounds = ScreenOf(window);
            var screen = new FakeScreen(bounds);
            var pointer = new Avalonia.Input.Pointer(Avalonia.Input.Pointer.GetNextFreeId(), PointerType.Mouse, true);
            var origin = wheel.TranslatePoint(new Point(0, 0), window)!.Value;
            var press = new Point(origin.X + 101, origin.Y + 100);

            wheel.RaiseEvent(new PointerPressedEventArgs(wheel, pointer, window, press, 0,
                new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.LeftButtonPressed), KeyModifiers.None)
            { RoutedEvent = InputElement.PointerPressedEvent });
            Dispatcher.UIThread.RunJobs();

            var cursor = window.PointToScreen(press);
            return (wheel, new FakeMouse(window, wheel, pointer, spy, bounds, cursor), spy, screen);
        }

        private static double Radius(ColorPickerWheel wheel) => (wheel.Height - 12) / 2;

        [AvaloniaTheory]
        [InlineData(0)]
        [InlineData(2)]
        [InlineData(5)]
        public void AWheelWithShift_KeepsMovingPastTheRightEdge(int staleMoves)
        {
            var (wheel, mouse, spy, screen) = WheelDrag();
            using var keepSpy = spy;
            using var keepScreen = screen;
            mouse.StaleMoves = staleMoves;

            for (var i = 0; i < 400; i++)
                mouse.Move(20, 0);

            Assert.True(mouse.Placements >= 8, "placements: " + mouse.Placements);
            var expected = (1 + FineSpeed * mouse.MovedX) / Radius(wheel);
            Assert.InRange(wheel.Color.HSV_S, expected - 0.01, expected + 0.01);
            Assert.InRange(wheel.Color.HSV_S, 0.5, 1.0);
        }

        // A fast mouse moves far in one event, so the first move after a placement is far from the new place.
        [AvaloniaTheory]
        [InlineData(90, 0, true)]
        [InlineData(90, 3, true)]
        [InlineData(90, 0, false)]
        [InlineData(150, 2, false)]
        public void AFastWheelWithShift_DoesNotStutterAtTheEdge(int step, int staleMoves, bool reportsPlacement)
        {
            var (wheel, mouse, spy, screen) = WheelDrag();
            using var keepSpy = spy;
            using var keepScreen = screen;
            mouse.StaleMoves = staleMoves;
            mouse.ReportsPlacement = reportsPlacement;
            var last = wheel.Color.HSV_S;
            var biggest = 0.0;

            for (var i = 0; i < 300; i++)
            {
                mouse.Move(step, 0);
                biggest = Math.Max(biggest, Math.Abs(wheel.Color.HSV_S - last));
                last = wheel.Color.HSV_S;
            }

            Assert.True(mouse.Placements >= 8, "placements: " + mouse.Placements);
            // One event moves the indicator by step * FineSpeed / radius at most, and nothing else.
            Assert.True(biggest <= step * FineSpeed / Radius(wheel) + 0.001, "biggest step: " + biggest);
            var expected = (1 + FineSpeed * mouse.MovedX) / Radius(wheel);
            Assert.InRange(wheel.Color.HSV_S, Math.Min(expected, 1) - 0.01, Math.Min(expected, 1) + 0.01);
        }

        [AvaloniaFact]
        public void AWheelWithShift_KeepsMovingPastTheLeftEdge()
        {
            var (wheel, mouse, spy, screen) = WheelDrag();
            using var keepSpy = spy;
            using var keepScreen = screen;

            for (var i = 0; i < 400; i++)
                mouse.Move(-20, 0);

            Assert.True(mouse.Placements >= 8, "placements: " + mouse.Placements);
            // The indicator went through the center and out on the other side.
            var expected = Math.Abs(1 + FineSpeed * mouse.MovedX) / Radius(wheel);
            Assert.InRange(wheel.Color.HSV_S, expected - 0.01, expected + 0.01);
            Assert.InRange(wheel.Color.HSV_H, 179, 181);
        }

        [AvaloniaFact]
        public void AWheelWithShift_KeepsMovingPastTheTopAndTheBottomEdges()
        {
            var (wheel, mouse, spy, screen) = WheelDrag();
            using var keepSpy = spy;
            using var keepScreen = screen;

            for (var i = 0; i < 400; i++)
                mouse.Move(0, 20);

            Assert.True(mouse.Placements >= 8, "placements: " + mouse.Placements);
            var expected = Math.Sqrt(1 + Math.Pow(FineSpeed * mouse.MovedY, 2)) / Radius(wheel);
            Assert.InRange(wheel.Color.HSV_S, expected - 0.01, expected + 0.01);

            for (var i = 0; i < 400; i++)
                mouse.Move(0, -20);

            Assert.True(mouse.Placements >= 16, "placements: " + mouse.Placements);
            var back = Math.Sqrt(1 + Math.Pow(FineSpeed * mouse.MovedY, 2)) / Radius(wheel);
            Assert.InRange(wheel.Color.HSV_S, back - 0.01, back + 0.01);
        }

        [AvaloniaFact]
        public void AWheelWithShift_KeepsMovingOnBothAxesAtOnce()
        {
            var (wheel, mouse, spy, screen) = WheelDrag();
            using var keepSpy = spy;
            using var keepScreen = screen;
            mouse.StaleMoves = 2;

            for (var i = 0; i < 400; i++)
                mouse.Move(17, 11);

            Assert.True(mouse.Placements >= 8, "placements: " + mouse.Placements);
            var expected = Math.Sqrt(Math.Pow(1 + FineSpeed * mouse.MovedX, 2) + Math.Pow(FineSpeed * mouse.MovedY, 2)) / Radius(wheel);
            Assert.InRange(wheel.Color.HSV_S, expected - 0.01, expected + 0.01);
        }

        [AvaloniaFact]
        public void ACursorFarFromTheEdges_IsNotMoved()
        {
            var (_, mouse, spy, screen) = WheelDrag();
            using var keepSpy = spy;
            using var keepScreen = screen;

            for (var i = 0; i < 5; i++)
                mouse.Move(10, 10);

            Assert.Equal(0, mouse.Placements);
        }

        [AvaloniaFact]
        public void WithoutKnownScreens_NothingIsMoved()
        {
            var (_, mouse, spy, screen) = WheelDrag();
            using var keepSpy = spy;
            using var keepScreen = screen;
            DragEditHelper.ScreenBoundsAt = (_, _) => null;

            for (var i = 0; i < 100; i++)
                mouse.Move(20, 0);

            Assert.Equal(0, mouse.Placements);
        }

        [AvaloniaFact]
        public void WhenThePlacementDoesNotWork_TheDragDoesNotJump()
        {
            var (wheel, mouse, spy, screen) = WheelDrag();
            using var keepSpy = spy;
            using var keepScreen = screen;
            mouse.HonorsPlacement = false;

            for (var i = 0; i < 60; i++)
                mouse.Move(20, 0);
            var atTheEdge = wheel.Color.HSV_S;

            // The cursor stays on the edge and the mouse keeps sending moves from there.
            for (var i = 0; i < 40; i++)
                mouse.Move(5, 0);

            Assert.InRange(wheel.Color.HSV_S, atTheEdge - 0.02, atTheEdge + 0.02);
        }

        [AvaloniaFact]
        public void ASliderWithShift_KeepsMovingPastTheRightEdge()
        {
            using var spy = new CursorSpy();
            var slider = new SliderCMiX { Minimum = 0, Maximum = 1, Value = 0.1, Width = 300, Height = 30, IsEditable = false };
            var window = Show(slider);
            var bounds = ScreenOf(window);
            using var screen = new FakeScreen(bounds);
            IPointer? real = null;
            slider.AddHandler(InputElement.PointerPressedEvent, (s, e) => real = e.Pointer, RoutingStrategies.Tunnel, handledEventsToo: true);
            var center = slider.TranslatePoint(new Point(150, 15), window)!.Value;

            window.MouseMove(center);
            window.MouseDown(center, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            var mouse = new FakeMouse(window, slider, real!, spy, bounds, window.PointToScreen(center)) { StaleMoves = 2 };

            // The first move after the press only starts the drag.
            mouse.Move(2, 0);
            var placementsBefore = mouse.Placements;
            var movedBefore = mouse.MovedX;
            for (var i = 0; i < 200; i++)
                mouse.Move(20, 0);

            Assert.True(mouse.Placements - placementsBefore >= 8, "placements: " + (mouse.Placements - placementsBefore));
            var expected = 0.1 + slider.SmallChange * (mouse.MovedX - movedBefore + 2);
            Assert.InRange(slider.Value, expected - 0.01, expected + 0.01);
        }

        [AvaloniaTheory]
        [InlineData(20)]
        [InlineData(90)]
        public void ADragValueWithShift_KeepsMovingPastTheRightEdge(int step)
        {
            using var spy = new CursorSpy();
            var drag = new DragValue { Width = 300, Height = 30, Value = 0 };
            var window = Show(drag);
            var bounds = ScreenOf(window);
            using var screen = new FakeScreen(bounds);
            var border = drag.GetVisualDescendants().OfType<Border>().First(b => b.Name == "borderValueDisplay");
            IPointer? real = null;
            border.AddHandler(InputElement.PointerPressedEvent, (s, e) => real = e.Pointer, RoutingStrategies.Tunnel, handledEventsToo: true);
            var center = border.TranslatePoint(new Point(border.Bounds.Width / 2, border.Bounds.Height / 2), window)!.Value;

            window.MouseMove(center);
            window.MouseDown(center, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            var mouse = new FakeMouse(window, border, real!, spy, bounds, window.PointToScreen(center)) { ReportsPlacement = false };

            // The first moves only pass the click threshold.
            mouse.Move(10, 0);
            var movedBefore = mouse.MovedX;
            for (var i = 0; i < 300; i++)
                mouse.Move(step, 0);

            Assert.True(mouse.Placements >= 8, "placements: " + mouse.Placements);
            var expected = (mouse.MovedX - movedBefore) * drag.SmallChange;
            Assert.InRange(drag.Value, expected * 0.98, expected * 1.02);
        }
    }
}
