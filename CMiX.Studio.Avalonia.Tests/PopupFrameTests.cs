using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Views.Controls;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The PopupFrame theme gives the frame its template.
    public class PopupFrameTests
    {
        private static Window Host(Control content)
        {
            var window = new Window { Content = content, Width = 400, Height = 300 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return window;
        }

        [AvaloniaFact]
        public void PopupFrame_InAWindow_PresentsItsContentInTheThemedTemplate()
        {
            var text = new TextBlock { Text = "Item" };
            var frame = new PopupFrame { Content = text };
            Host(frame);
            frame.ApplyTemplate();

            var root = Assert.IsType<Border>(frame.GetVisualChildren().Single());
            Assert.NotNull(root.Effect);
            var presenter = frame.GetVisualDescendants().OfType<ContentPresenter>()
                .Single(p => p.Name == "PART_ContentPresenter" && p.TemplatedParent == frame);
            Assert.Same(text, presenter.Content);
        }
    }
}
