using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // The drop down of the main menu has the look of a context menu: rows of MenuItemContext in a PopupFrame.
    public class MainMenuDropdownLookTests
    {
        [AvaloniaFact]
        public void Open_ARowOfTheMenuBar_ShowsContextMenuRowsInAPopupFrame()
        {
            var open = new MenuItem { Header = "Open" };
            var file = new MenuItem
            {
                Header = "File",
                Theme = Tokens.Theme("MenuItemTop"),
                ItemsSource = new object[] { open },
            };
            var menu = new Menu { Theme = Tokens.Theme("MenuDefault"), ItemsSource = new object[] { file } };
            TestWindow.Show(menu, 500, 400);

            file.Open();
            Dispatcher.UIThread.RunJobs();

            var popup = file.GetVisualDescendants().OfType<Popup>().Single(part => part.Name == "PART_Popup");
            Assert.IsType<PopupFrame>(popup.Child);
            Assert.Same(Tokens.Theme("MenuItemContext"), open.Theme);
            Assert.Equal(Tokens.Get<double>("ControlHeightMD"), open.Bounds.Height);
        }
    }
}
