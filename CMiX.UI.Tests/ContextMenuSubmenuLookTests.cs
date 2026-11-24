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
    // A row of a context menu that opens a submenu: the panel of the submenu is a PopupFrame like the panel of the menu,
    // and the rows of the submenu have the theme of the rows of the menu.
    public class ContextMenuSubmenuLookTests
    {
        private static (MenuItem Submenu, MenuItem Row) Open()
        {
            var row = new MenuItem { Header = "One" };
            var submenu = new MenuItem { Header = "Add" };
            submenu.Items.Add(row);

            var menu = new ContextMenu { Theme = Tokens.Theme("ContextMenuDefault"), ItemsSource = new object[] { submenu } };
            var button = new Button { Content = "Owner", Width = 100, Height = 30, ContextMenu = menu };
            TestWindow.Show(button, 400, 300);

            menu.Open(button);
            Dispatcher.UIThread.RunJobs();
            submenu.IsSubMenuOpen = true;
            Dispatcher.UIThread.RunJobs();
            return (submenu, row);
        }

        [AvaloniaFact]
        public void Show_TheSubmenuPanel_IsAPopupFrame()
        {
            var (submenu, _) = Open();

            var popup = submenu.GetVisualDescendants().OfType<Popup>().First(p => p.Name == "PART_Popup");

            Assert.IsType<PopupFrame>(popup.Child);
        }

        [AvaloniaFact]
        public void Show_TheRowsOfTheSubmenu_HaveTheThemeOfTheMenuRows()
        {
            var (submenu, row) = Open();

            Assert.Same(Tokens.Theme("MenuItemContext"), submenu.Theme);
            Assert.Same(Tokens.Theme("MenuItemContext"), row.Theme);
        }
    }
}
