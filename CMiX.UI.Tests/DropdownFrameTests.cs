using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
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
            var button = new PopupButton { Caption = "Add", PopupContent = new TextBlock { Text = "Short" }, Width = 300, HorizontalAlignment = HorizontalAlignment.Left };
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

            var expected = (ISolidColorBrush)Application.Current!.FindResource("Grey200Brush")!;
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
        public void ThePopup_MovesByTheRoomOfTheShadow_SoThePanelStaysWhereItWas()
        {
            var (_, toggle, popup) = ShowPopupButton();

            Open(toggle);

            Assert.Equal(-ShadowRoom, popup.HorizontalOffset);
            Assert.Equal(-ShadowRoom, popup.VerticalOffset);
            Assert.Equal(300, PanelOf(popup).Bounds.Width);
        }

        [AvaloniaFact]
        public void OpeningThePopupAgain_DoesNotMoveItAgain()
        {
            var (_, toggle, popup) = ShowPopupButton();
            Open(toggle);
            toggle.IsChecked = false;
            Dispatcher.UIThread.RunJobs();

            Open(toggle);

            Assert.Equal(-ShadowRoom, popup.HorizontalOffset);
            Assert.Equal(-ShadowRoom, popup.VerticalOffset);
        }

        [AvaloniaFact]
        public void TheComboBoxDropdown_HasTheSameFrame_AndItsPanelIsAsWideAsTheComboBox()
        {
            var combo = new ComboBox { ItemsSource = new[] { "One", "Two", "Three" }, SelectedIndex = 0, Width = 220, HorizontalAlignment = HorizontalAlignment.Left };
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
