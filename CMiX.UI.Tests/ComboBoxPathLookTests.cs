using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Controls.Path;
using CMiX.UI.Panels;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    public class ComboBoxPathLookTests
    {
        [AvaloniaFact]
        public void TheBrowseButton_IsRoundedOnTheRightOnly()
        {
            var selector = new ComboBoxPath();
            TestWindow.Show(selector, 400, 100);

            var browse = selector.GetVisualDescendants().OfType<Button>().First();

            Assert.Equal(ControlPosition.Right, PositionedControl.GetPosition(browse));
        }
    }
}
