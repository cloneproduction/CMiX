using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using CMiX.UI.Controls;
using Xunit;

namespace CMiX.UI.Tests
{
    // A button that opens a popup with any content.
    public class PopupButtonTests
    {
        private static (PopupButton Panel, TextBlock Content) Show()
        {
            var content = new TextBlock { Text = "choice" };
            var panel = new PopupButton { Caption = "Add", SelectionPanel = content, Width = 200 };
            var window = new Window { Content = panel, Width = 300, Height = 200 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (panel, content);
        }

        [AvaloniaFact]
        public void TheButton_ShowsTheCaption_AndTheContentStartsInTheLogicalTree()
        {
            var (panel, content) = Show();

            Assert.Equal("Add", panel.FindControl<ToggleButton>("PopUpToggle")!.Content);
            Assert.Same(panel, ((ILogical)content).LogicalParent);
        }

        [AvaloniaFact]
        public void CheckingTheButton_OpensThePopup_AndUncheckingClosesIt()
        {
            var (panel, _) = Show();
            var toggle = panel.FindControl<ToggleButton>("PopUpToggle")!;
            var popup = panel.GetLogicalDescendants().OfType<Popup>().Single();
            Assert.False(popup.IsOpen);

            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            Assert.True(popup.IsOpen);

            toggle.IsChecked = false;
            Dispatcher.UIThread.RunJobs();
            Assert.False(popup.IsOpen);
        }
    }
}
