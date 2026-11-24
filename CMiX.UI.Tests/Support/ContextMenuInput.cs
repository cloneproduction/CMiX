using Avalonia.Controls;
using Avalonia.Threading;

namespace CMiX.UI.Tests.Support
{
    internal static class ContextMenuInput
    {
        public static void Request(Control source)
        {
            source.RaiseEvent(new ContextRequestedEventArgs { Source = source });
            Dispatcher.UIThread.RunJobs();
        }
    }
}
