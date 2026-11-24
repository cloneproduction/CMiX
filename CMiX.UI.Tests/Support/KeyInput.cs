using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace CMiX.UI.Tests.Support
{
    internal static class KeyInput
    {
        public static void Press(Interactive target, Key key, int times = 1)
        {
            for (var i = 0; i < times; i++)
            {
                target.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = key });
                Dispatcher.UIThread.RunJobs();
            }
        }

        // The key goes to the focused element of the window of the target, or to the target when nothing has the focus.
        public static void PressOnFocus(Control target, Key key, int times = 1)
        {
            for (var i = 0; i < times; i++)
            {
                var focused = TopLevel.GetTopLevel(target)?.FocusManager?.GetFocusedElement() as Interactive;
                Press(focused ?? target, key);
            }
        }
    }
}
