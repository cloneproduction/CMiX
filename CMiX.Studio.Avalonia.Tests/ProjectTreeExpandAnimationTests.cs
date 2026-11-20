using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Panels;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // A row of the project tree opens and closes its children with the animation of PanelCollapse.
    public class ProjectTreeExpandAnimationTests
    {
        private static ControlTheme Theme => (ControlTheme)Application.Current!.FindResource("TreeViewItemCMiX")!;

        private static (TreeViewItem Row, PanelCollapse Panel) Show(bool expanded, bool withChild = true)
        {
            var row = new TreeViewItem { Theme = Theme, Header = "Parent", IsExpanded = expanded };
            if (withChild)
                row.Items.Add(new TreeViewItem { Theme = Theme, Header = "Child" });

            new Window { Content = new TreeView { Items = { row } }, Width = 300, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();
            return (row, row.GetVisualDescendants().OfType<PanelCollapse>().First());
        }

        [AvaloniaFact]
        public void Expand_AClosedOrOpenRow_HidesOrShowsItsChildren()
        {
            var (_, closed) = Show(false);
            var (_, open) = Show(true);

            Assert.False(closed.Child!.IsVisible);
            Assert.Equal(0, closed.Bounds.Height);
            Assert.True(open.Child!.IsVisible);
            Assert.True(open.Bounds.Height > 0);
        }

        [AvaloniaFact]
        public void Expand_ARow_IsAnAnimation()
        {
            var (row, panel) = Show(false);

            row.IsExpanded = true;
            Dispatcher.UIThread.RunJobs();

            Assert.True(panel.Child!.IsVisible);
            Assert.InRange(panel.Progress, 0, 0.999);
        }

        [AvaloniaFact]
        public void Expand_AfterTheAnimation_LeavesTheRowOpenOrClosed()
        {
            var (row, panel) = Show(false);
            panel.Duration = TimeSpan.Zero;

            row.IsExpanded = true;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(1, panel.Progress);

            row.IsExpanded = false;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(0, panel.Progress);
            Assert.False(panel.Child!.IsVisible);
        }

        [AvaloniaFact]
        public void Show_ARowWithoutChildren_TakesNoRoomForChildren()
        {
            var (_, panel) = Show(true, withChild: false);

            Assert.False(panel.IsVisible);
        }
    }
}
