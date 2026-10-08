using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;

namespace CMiX.Studio.Avalonia.Tests
{
    internal static class PointerInput
    {
        // The headless mouse does not reach every control, so the pointer events are raised on the target.
        public static void Click(Window window, Control target, MouseButton button, Point? at = null)
        {
            var position = at ?? target.TranslatePoint(new Point(target.Bounds.Width / 2, target.Bounds.Height / 2), window)!.Value;
            var pointer = new Pointer(Pointer.GetNextFreeId(), PointerType.Mouse, true);
            var left = button == MouseButton.Left;

            window.MouseMove(position);
            Dispatcher.UIThread.RunJobs();
            target.RaiseEvent(new PointerPressedEventArgs(target, pointer, window, position, 1,
                new PointerPointProperties(left ? RawInputModifiers.LeftMouseButton : RawInputModifiers.RightMouseButton,
                    left ? PointerUpdateKind.LeftButtonPressed : PointerUpdateKind.RightButtonPressed), KeyModifiers.None));
            target.RaiseEvent(new PointerReleasedEventArgs(target, pointer, window, position, 2,
                new PointerPointProperties(RawInputModifiers.None,
                    left ? PointerUpdateKind.LeftButtonReleased : PointerUpdateKind.RightButtonReleased), KeyModifiers.None, button));
            Dispatcher.UIThread.RunJobs();
        }
    }
}
