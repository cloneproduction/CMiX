using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Controls.String;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // A press outside the open popups only closes them: the control under the pointer gets neither the press nor its release.
    public class OutsideClickTests
    {
        private sealed class Screen
        {
            public Window Window { get; init; } = null!;
            public ToggleButton Toggle { get; init; } = null!;
            public Popup Popup { get; init; } = null!;
            public Button Inside { get; init; } = null!;
            public Button Below { get; init; } = null!;
            public int BelowClicks { get; set; }
            public int InsideClicks { get; set; }
            public int MenuRequests { get; set; }
        }

        // A toggle that opens a popup with a button, and a button far from the popup that counts its clicks and its menu requests.
        private static Screen Show()
        {
            var toggle = new ToggleButton { Content = "Open", Width = 120, HorizontalAlignment = HorizontalAlignment.Left };
            var inside = new Button { Content = "Inside", Width = 100, Height = 30 };
            var popup = new Popup { Child = new Border { Background = Avalonia.Media.Brushes.Gray, Padding = new Thickness(10), Child = inside } };
            TogglePopup.SetToggle(popup, toggle);
            TogglePopup.SetClosesOnPointerAway(popup, false);
            var below = new Button { Content = "Below", Width = 200, Height = 40, Margin = new Thickness(600, 300, 0, 0), HorizontalAlignment = HorizontalAlignment.Left };
            var window = TestWindow.Show(new StackPanel { Children = { toggle, popup, below } }, 1200, 800);

            var screen = new Screen { Window = window, Toggle = toggle, Popup = popup, Inside = inside, Below = below };
            below.Click += (_, _) => screen.BelowClicks++;
            inside.Click += (_, _) => screen.InsideClicks++;
            below.ContextRequested += (_, e) => { screen.MenuRequests++; e.Handled = true; };
            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            return screen;
        }

        private static Point Middle(Window window, Control control) =>
            control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;

        [AvaloniaFact]
        public void Click_OutsideAnOpenPopup_ClosesItAndClicksNothingElse()
        {
            var screen = Show();
            Assert.True(screen.Popup.IsOpen);

            PointerInput.Click(screen.Window, Middle(screen.Window, screen.Below));

            Assert.False(screen.Popup.IsOpen);
            Assert.False(screen.Toggle.IsChecked);
            Assert.Equal(0, screen.BelowClicks);

            // The popup is closed: the next click reaches the button.
            PointerInput.Click(screen.Window, Middle(screen.Window, screen.Below));
            Assert.Equal(1, screen.BelowClicks);
        }

        [AvaloniaFact]
        public void RightClick_OutsideAnOpenPopup_ClosesItAndOpensNoMenu()
        {
            var screen = Show();

            PointerInput.Click(screen.Window, Middle(screen.Window, screen.Below), MouseButton.Right);

            Assert.False(screen.Popup.IsOpen);
            Assert.Equal(0, screen.MenuRequests);
        }

        [AvaloniaFact]
        public void Click_InsideAnOpenPopup_ReachesItsButton()
        {
            var screen = Show();

            PointerInput.Click(screen.Window, Middle(screen.Window, screen.Inside));

            Assert.Equal(1, screen.InsideClicks);
        }

        [AvaloniaFact]
        public void Click_OnTheToggleOfAnOpenPopup_ClosesItAndLeavesItClosed()
        {
            var screen = Show();

            PointerInput.Click(screen.Window, Middle(screen.Window, screen.Toggle));

            Assert.False(screen.Popup.IsOpen);
            Assert.False(screen.Toggle.IsChecked);
        }

        [AvaloniaFact]
        public void Click_OutsideAnOpenContextMenu_ClosesItAndClicksNothingElse()
        {
            var screen = Show();
            screen.Toggle.IsChecked = false;
            Dispatcher.UIThread.RunJobs();
            var menu = new ContextMenu { ItemsSource = new[] { new MenuItem { Header = "One" } } };
            screen.Window.MouseMove(new Point(20, 200));
            menu.Open(screen.Toggle);
            Dispatcher.UIThread.RunJobs();
            Assert.True(menu.IsOpen);

            PointerInput.Click(screen.Window, Middle(screen.Window, screen.Below));

            Assert.False(menu.IsOpen);
            Assert.Equal(0, screen.BelowClicks);
        }

        // The drop-down of a combo box takes IsOpen from the combo box with a binding of its template. A close by a click outside must
        // keep that binding, so the combo box opens its drop-down again.
        [AvaloniaFact]
        public void Click_AComboBoxAfterAClickOutside_OpensItsDropDownAgain()
        {
            var combo = new ComboBox { ItemsSource = new[] { "One", "Two", "Three" }, Width = 150, HorizontalAlignment = HorizontalAlignment.Left };
            var window = TestWindow.Show(new StackPanel { Children = { combo } }, 800, 600);
            Popup DropDown() => combo.GetVisualDescendants().OfType<Popup>().First(popup => popup.Name == "PART_Popup");
            var outside = new Point(600, 500);

            for (var round = 1; round <= 3; round++)
            {
                PointerInput.Click(window, Middle(window, combo));
                Assert.True(DropDown().IsOpen, "open, round " + round);

                PointerInput.Click(window, outside);
                Assert.False(DropDown().IsOpen, "closed, round " + round);
                Assert.False(combo.IsDropDownOpen, "combo closed, round " + round);
            }
        }

        [AvaloniaFact]
        public void Click_WhileATooltipIsOpen_ReachesTheButton()
        {
            var screen = Show();
            screen.Toggle.IsChecked = false;
            Dispatcher.UIThread.RunJobs();
            ToolTip.SetTip(screen.Toggle, "Tip");
            ToolTip.SetIsOpen(screen.Toggle, true);
            Dispatcher.UIThread.RunJobs();

            PointerInput.Click(screen.Window, Middle(screen.Window, screen.Below));

            Assert.Equal(1, screen.BelowClicks);
            ToolTip.SetIsOpen(screen.Toggle, false);
        }

        // ---------------------------------------------------------------- the rename of a name

        private static (Window Window, TextBoxEditable Name, Button Other, Func<int> MenuRequests) ShowName()
        {
            var name = new TextBoxEditable { Text = "Layer", Width = 200, HorizontalAlignment = HorizontalAlignment.Left };
            var other = new Button { Content = "Other", Width = 200, Height = 40, Margin = new Thickness(0, 200, 0, 0), HorizontalAlignment = HorizontalAlignment.Left };
            var requests = 0;
            other.ContextRequested += (_, e) => { requests++; e.Handled = true; };
            var window = TestWindow.Show(new StackPanel { Children = { name, other } }, 800, 600);
            name.IsEditing = true;
            Dispatcher.UIThread.RunJobs();
            return (window, name, other, () => requests);
        }

        private static TextBox InputOf(TextBoxEditable name) =>
            name.GetVisualDescendants().OfType<TextBox>().First(box => box.IsVisible);

        [AvaloniaFact]
        public void RightClick_OutsideANameThatIsEdited_CancelsTheEditAndOpensNoMenu()
        {
            var (window, name, other, menuRequests) = ShowName();
            InputOf(name).Text = "Typed";

            PointerInput.Click(window, Middle(window, other), MouseButton.Right);

            Assert.False(name.IsEditing);
            Assert.Equal("Layer", name.Text);
            Assert.Equal(0, menuRequests());

            // The edit is over: the next right click opens the menu.
            PointerInput.Click(window, Middle(window, other), MouseButton.Right);
            Assert.Equal(1, menuRequests());
        }

        [AvaloniaFact]
        public void LeftClick_OutsideANameThatIsEdited_KeepsTheTypedName()
        {
            var (window, name, other, _) = ShowName();
            InputOf(name).Text = "Typed";

            PointerInput.Click(window, Middle(window, other));

            Assert.False(name.IsEditing);
            Assert.Equal("Typed", name.Text);
        }
    }
}
