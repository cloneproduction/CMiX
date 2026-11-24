using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using CMiX.UI.Controls;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // A button that opens a popup with any content.
    public class ButtonPopupTests
    {
        private static (ButtonPopup Panel, TextBlock Content) Show()
        {
            var content = new TextBlock { Text = "choice" };
            var panel = new ButtonPopup { Caption = "Add", PopupContent = content, Width = 200 };
            TestWindow.Show(panel, 300, 200);
            return (panel, content);
        }

        [AvaloniaFact]
        public void TheButton_ShowsTheCaption_AndTheContentStartsInTheLogicalTree()
        {
            var (panel, content) = Show();

            Assert.Equal("Add", panel.FindControl<ToggleButton>("popupToggle")!.Content);
            Assert.Same(panel, ((ILogical)content).LogicalParent);
        }

        [AvaloniaFact]
        public void CheckingTheButton_OpensThePopup_AndUncheckingClosesIt()
        {
            var (panel, _) = Show();
            var toggle = panel.FindControl<ToggleButton>("popupToggle")!;
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
