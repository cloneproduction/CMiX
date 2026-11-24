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
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // The dropdown panels share one frame: the same color, the same shadow, and the position that they had without the shadow.
    public class DropdownFrameTests
    {
        private const double ShadowRoom = 8;

        private static Border PanelOf(Popup popup) =>
            popup.Child!.GetVisualDescendants().OfType<Border>().Single(border => border.Name == "PART_Surface");

        [AvaloniaFact]
        public void ThePanelOfAnEdgeAlignedComboBox_StartsAtTheLeftEdgeOfTheComboBox()
        {
            var theme = Tokens.Theme("ComboBoxWithButton");
            var combo = new ComboBox { Theme = theme, ItemsSource = new[] { "One", "Two" }, SelectedIndex = 0, Width = 120, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(100, 40, 0, 0) };
            TestWindow.Show(combo, 900, 500);

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
        public void TheComboBoxDropdown_HasTheSameFrame_AndItsPanelIsAsWideAsTheComboBox()
        {
            var combo = new ComboBox { ItemsSource = new[] { "One", "Two", "Three" }, SelectedIndex = 0, Width = 220, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(100, 40, 0, 0) };
            TestWindow.Show(combo, 900, 500);

            combo.IsDropDownOpen = true;
            Dispatcher.UIThread.RunJobs();

            var popup = combo.GetVisualDescendants().OfType<Popup>().Single();
            Assert.IsType<DropdownFrame>(popup.Child);
            Assert.Equal(220, PanelOf(popup).Bounds.Width);
            Assert.Equal(OnScreen(combo).Left, OnScreen(PanelOf(popup)).Left);
            Assert.Equal(2 - ShadowRoom, popup.VerticalOffset);
        }
    }
}
