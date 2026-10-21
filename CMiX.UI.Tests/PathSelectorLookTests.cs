using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Controls.Path;
using CMiX.UI.Panels;
using Xunit;

namespace CMiX.UI.Tests
{
    public class PathSelectorLookTests
    {
        [AvaloniaFact]
        public void TheBrowseButton_IsRoundedOnTheRightOnly()
        {
            var selector = new PathSelector();
            new Window { Content = selector, Width = 400, Height = 100 }.Show();
            Dispatcher.UIThread.RunJobs();

            var browse = selector.GetVisualDescendants().OfType<Button>().First();

            Assert.Equal(ControlPosition.Right, PositionedControl.GetPosition(browse));
        }
    }
}
