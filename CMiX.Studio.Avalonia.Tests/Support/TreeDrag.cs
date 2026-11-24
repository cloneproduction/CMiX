using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using CMiX.Studio.Avalonia.ViewModels.ListTree;

namespace CMiX.Studio.Avalonia.Tests.Support
{
    // Drives a drag of a row of the project tree with the mouse of the headless window: a press on the source row, a move past the
    // threshold, moves to the target, then a release (the drop) or an Escape (the cancel). The points are in the window.
    internal static class TreeDrag
    {
        // A move that starts the drag: longer than the threshold of the drag numbers.
        private static readonly Vector Start = new(0, 2 * ListTreeDragNumbers.Default.Threshold);

        // The hit test of the window reads the last rendered scene. After a change of the rows it must be rendered first.
        public static void Render(Window window)
        {
            window.UpdateLayout();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            Dispatcher.UIThread.RunJobs();
        }

        // Presses the source row and moves the pointer to the target. The drag runs and the button stays down.
        public static void Over(Window window, Point source, Point target)
        {
            Render(window);
            window.MouseDown(source, MouseButton.Left);
            window.MouseMove(source + Start, RawInputModifiers.LeftMouseButton);
            MoveTo(window, target);
        }

        // Moves the pointer of the drag to the point.
        public static void MoveTo(Window window, Point point)
        {
            Render(window);
            window.MouseMove(point, RawInputModifiers.LeftMouseButton);
            Dispatcher.UIThread.RunJobs();
        }

        // Releases the button at the point: the drop.
        public static void Release(Window window, Point point)
        {
            window.MouseUp(point, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
        }

        // Drags the source row to the target and drops it there.
        public static void Drop(Window window, Point source, Point target)
        {
            Over(window, source, target);
            Release(window, target);
        }

        // Escape ends the drag without a drop. The button goes up at the point after it.
        public static void Cancel(Window window, Point point)
        {
            window.KeyPressQwerty(PhysicalKey.Escape, RawInputModifiers.None);
            Dispatcher.UIThread.RunJobs();
            Release(window, point);
        }
    }
}
