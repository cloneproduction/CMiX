using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace CMiX.UI.Tests.Support
{
    internal static class PointerInput
    {
        // The headless mouse does not reach every control, so the pointer events are raised on the target.
        public static void Click(Window window, Control target, MouseButton button, Point? at = null)
        {
            var position = Position(window, target, at);
            var pointer = NewPointer();

            Press(window, target, button, position, pointer);
            Release(window, target, button, position, pointer);
        }

        public static void Press(Window window, Control target, MouseButton button, Point? at = null) =>
            Press(window, target, button, Position(window, target, at), NewPointer());

        public static void Release(Window window, Control target, MouseButton button, Point? at = null) =>
            Release(window, target, button, Position(window, target, at), NewPointer());

        private static void Press(Window window, Control target, MouseButton button, Point position, Pointer pointer)
        {
            var left = button == MouseButton.Left;

            window.MouseMove(position);
            Dispatcher.UIThread.RunJobs();
            target.RaiseEvent(new PointerPressedEventArgs(target, pointer, window, position, 1,
                new PointerPointProperties(left ? RawInputModifiers.LeftMouseButton : RawInputModifiers.RightMouseButton,
                    left ? PointerUpdateKind.LeftButtonPressed : PointerUpdateKind.RightButtonPressed), KeyModifiers.None));
        }

        private static void Release(Window window, Control target, MouseButton button, Point position, Pointer pointer)
        {
            var left = button == MouseButton.Left;

            target.RaiseEvent(new PointerReleasedEventArgs(target, pointer, window, position, 2,
                new PointerPointProperties(RawInputModifiers.None,
                    left ? PointerUpdateKind.LeftButtonReleased : PointerUpdateKind.RightButtonReleased), KeyModifiers.None, button));
            Dispatcher.UIThread.RunJobs();
        }

        // The headless mouse of the window: a move, a press and a release at one point.
        public static void Click(Window window, Point at, MouseButton button = MouseButton.Left)
        {
            window.MouseMove(at);
            window.MouseDown(at, button);
            window.MouseUp(at, button);
            Dispatcher.UIThread.RunJobs();
        }

        public static void Move(Window window, Point to)
        {
            window.MouseMove(to);
            Dispatcher.UIThread.RunJobs();
        }

        // A pointer of its own, for a test that captures it or takes the capture away.
        public static Pointer NewPointer() => new(Pointer.GetNextFreeId(), PointerType.Mouse, true);

        // The real pointer of the headless mouse, taken from the press that reaches the control.
        public static Func<IPointer?> WatchPressedPointer(Control control)
        {
            IPointer? pointer = null;
            control.AddHandler(InputElement.PointerPressedEvent, (_, e) => pointer = e.Pointer, RoutingStrategies.Tunnel, handledEventsToo: true);
            return () => pointer;
        }

        public static void Press(Control target, IPointer pointer, Point inWindow, Window window)
        {
            target.RaiseEvent(new PointerPressedEventArgs(target, pointer, window, inWindow, 0,
                new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.LeftButtonPressed), KeyModifiers.None)
            { RoutedEvent = InputElement.PointerPressedEvent });
            Dispatcher.UIThread.RunJobs();
        }

        public static void Move(Control target, IPointer pointer, Point inWindow, Window window,
            KeyModifiers modifiers = KeyModifiers.None, RawInputModifiers buttons = RawInputModifiers.None)
        {
            target.RaiseEvent(new PointerEventArgs(InputElement.PointerMovedEvent, target, pointer, window, inWindow, 0,
                new PointerPointProperties(buttons, PointerUpdateKind.Other), modifiers));
            Dispatcher.UIThread.RunJobs();
        }

        public static void Release(Control target, IPointer pointer, Point inWindow, Window window)
        {
            target.RaiseEvent(new PointerReleasedEventArgs(target, pointer, window, inWindow, 0,
                new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.LeftButtonReleased), KeyModifiers.None, MouseButton.Left));
            Dispatcher.UIThread.RunJobs();
        }

        private static Point Position(Window window, Control target, Point? at) =>
            at ?? target.TranslatePoint(new Point(target.Bounds.Width / 2, target.Bounds.Height / 2), window)!.Value;
    }
}
