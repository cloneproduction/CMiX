using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Parts;
using Xunit;

namespace CMiX.UI.Tests
{
    // The dropdown panels share one frame: the same color, the same shadow, and the position that they had without the shadow.
    public class DropdownFrameTests
    {
        private const double ShadowRoom = 8;

        private static (PopupButton Button, ToggleButton Toggle, Popup Popup) ShowPopupButton()
        {
            var button = new PopupButton { Caption = "Add", PopupContent = new TextBlock { Text = "Short" }, Width = 300, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(100, 40, 0, 0) };
            new Window { Content = button, Width = 900, Height = 500 }.Show();
            Dispatcher.UIThread.RunJobs();
            var toggle = button.FindControl<ToggleButton>("PopUpToggle")!;
            return (button, toggle, button.GetLogicalDescendants().OfType<Popup>().Single());
        }

        private static void Open(ToggleButton toggle)
        {
            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
        }

        private static Border PanelOf(Popup popup) =>
            popup.Child!.GetVisualDescendants().OfType<Border>().Single(border => border.Name == "PART_Panel");

        [AvaloniaFact]
        public void ThePopupOfAPopupButton_HoldsADropdownFrame()
        {
            var (_, toggle, popup) = ShowPopupButton();

            Open(toggle);

            Assert.IsType<DropdownFrame>(popup.Child);
        }

        [AvaloniaFact]
        public void ThePanel_HasTheAddPopupColor()
        {
            var (_, toggle, popup) = ShowPopupButton();

            Open(toggle);

            var expected = (ISolidColorBrush)Application.Current!.FindResource("PopupBackgroundBrush")!;
            Assert.Equal(expected.Color, ((ISolidColorBrush)PanelOf(popup).Background!).Color);
        }

        [AvaloniaFact]
        public void TheFrame_CastsTheWindowShadow()
        {
            var (_, toggle, popup) = ShowPopupButton();

            Open(toggle);

            var shadowed = popup.Child!.GetVisualDescendants().OfType<Border>().Where(border => border.Effect != null).ToList();
            Assert.Single(shadowed);
            Assert.Same(Application.Current!.FindResource("DropShadowWindow"), shadowed[0].Effect);
        }

        [AvaloniaFact]
        public void ACenteredPopup_MovesUpByTheRoomOfTheShadow_ButNotSideways()
        {
            var (_, toggle, popup) = ShowPopupButton();

            Open(toggle);

            Assert.Equal(0, popup.HorizontalOffset);
            Assert.Equal(-ShadowRoom, popup.VerticalOffset);
            Assert.Equal(300, PanelOf(popup).Bounds.Width);
        }

        [AvaloniaFact]
        public void ThePanelOfAPopupButton_StartsAtTheBottomEdgeOfTheToggle()
        {
            var (_, toggle, popup) = ShowPopupButton();

            Open(toggle);

            var target = OnScreen(toggle);
            Assert.Equal(target.Top + toggle.Bounds.Height, OnScreen(PanelOf(popup)).Top);
        }

        [AvaloniaFact]
        public void ThePanelOfAnEdgeAlignedComboBox_StartsAtTheLeftEdgeOfTheComboBox()
        {
            var theme = (ControlTheme)Application.Current!.FindResource("ComboBoxWithButton")!;
            var combo = new ComboBox { Theme = theme, ItemsSource = new[] { "One", "Two" }, SelectedIndex = 0, Width = 120, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(100, 40, 0, 0) };
            new Window { Content = combo, Width = 900, Height = 500 }.Show();
            Dispatcher.UIThread.RunJobs();

            combo.IsDropDownOpen = true;
            Dispatcher.UIThread.RunJobs();

            var panel = OnScreen(PanelOf(combo.GetVisualDescendants().OfType<Popup>().Single()));
            Assert.Equal(OnScreen(combo).Left, panel.Left);
            Assert.Equal(250, panel.Width);
        }

        // Where the panel is on the screen, against the control that it hangs under.
        private static (double Left, double Width, double Top) OnScreen(Control control)
        {
            var point = control.PointToScreen(new Point(0, 0));
            return (point.X, control.Bounds.Width, point.Y);
        }

        [AvaloniaFact]
        public void ThePanelOfAPopupButton_StartsAtTheLeftEdgeOfTheToggle_AndIsAsWide()
        {
            var (_, toggle, popup) = ShowPopupButton();

            Open(toggle);

            var panel = OnScreen(PanelOf(popup));
            var target = OnScreen(toggle);
            Assert.Equal(target.Left, panel.Left);
            Assert.Equal(target.Width, panel.Width);
        }

        [AvaloniaFact]
        public void ThePanelOfAComboBox_StartsAtTheLeftEdgeOfTheComboBox_AndIsAsWide()
        {
            var combo = new ComboBox { ItemsSource = new[] { "One", "Two", "Three" }, SelectedIndex = 0, Width = 220, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(100, 40, 0, 0) };
            new Window { Content = combo, Width = 900, Height = 500 }.Show();
            Dispatcher.UIThread.RunJobs();

            combo.IsDropDownOpen = true;
            Dispatcher.UIThread.RunJobs();

            var panel = OnScreen(PanelOf(combo.GetVisualDescendants().OfType<Popup>().Single()));
            var target = OnScreen(combo);
            Assert.Equal(target.Left, panel.Left);
            Assert.Equal(target.Width, panel.Width);
        }

        [AvaloniaFact]
        public void OpeningThePopupAgain_DoesNotMoveItAgain()
        {
            var (_, toggle, popup) = ShowPopupButton();
            Open(toggle);
            toggle.IsChecked = false;
            Dispatcher.UIThread.RunJobs();

            Open(toggle);

            Assert.Equal(0, popup.HorizontalOffset);
            Assert.Equal(-ShadowRoom, popup.VerticalOffset);
        }

        [AvaloniaFact]
        public void TheComboBoxDropdown_HasTheSameFrame_AndItsPanelIsAsWideAsTheComboBox()
        {
            var combo = new ComboBox { ItemsSource = new[] { "One", "Two", "Three" }, SelectedIndex = 0, Width = 220, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(100, 40, 0, 0) };
            new Window { Content = combo, Width = 900, Height = 500 }.Show();
            Dispatcher.UIThread.RunJobs();

            combo.IsDropDownOpen = true;
            Dispatcher.UIThread.RunJobs();

            var popup = combo.GetVisualDescendants().OfType<Popup>().Single();
            Assert.IsType<DropdownFrame>(popup.Child);
            Assert.Equal(220, PanelOf(popup).Bounds.Width);
            Assert.Equal(2 - ShadowRoom, popup.VerticalOffset);
        }
    }
}
