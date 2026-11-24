using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace CMiX.UI.Tests.Support
{
    internal static class TestWindow
    {
        public static Window Show(Control content, double width = 400, double height = 300)
        {
            var window = new Window { Content = content, Width = width, Height = height };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return window;
        }

        // The headless renderer cannot hit-test through a geometry clip, so the clips are removed.
        public static Window ShowWithoutClips(Control content, double width = 400, double height = 300)
        {
            var window = Show(content, width, height);
            foreach (var visual in window.GetVisualDescendants().Where(v => v.Clip != null))
                visual.Clip = null;
            Dispatcher.UIThread.RunJobs();
            return window;
        }

        // The hit test uses the last rendered frame, so a test renders one after a change of the layout.
        public static void Render()
        {
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            Dispatcher.UIThread.RunJobs();
        }
    }
}
