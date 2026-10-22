using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Views.Controls;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Panels;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class OutlinerLookTests
    {
        [AvaloniaFact]
        public void TheClearButton_IsRoundedOnTheRightOnly()
        {
            var outliner = new Outliner();
            new Window { Content = outliner, Width = 400, Height = 100 }.Show();
            Dispatcher.UIThread.RunJobs();

            var clear = outliner.GetVisualDescendants().OfType<Button>().Single(button => button.Name == "deleteItemButton");

            Assert.Equal(ControlPosition.Right, PositionedControl.GetPosition(clear));
        }
    }
}
