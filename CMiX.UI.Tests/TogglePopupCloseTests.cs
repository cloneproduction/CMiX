using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Collection;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // Each popup chooses how it closes when the pointer goes away: ClosesOnPointerAway and PointerAwayDistance of TogglePopup.
    public class TogglePopupCloseTests
    {
        private static (Window Window, ToggleButton Toggle, Popup Popup) Show(Action<Popup>? configure = null)
        {
            var toggle = new ToggleButton { Content = "Open", Width = 120, HorizontalAlignment = HorizontalAlignment.Left };
            var popup = new Popup { Child = new Border { Child = new TextBlock { Text = "Content" } } };
            TogglePopup.SetToggle(popup, toggle);
            configure?.Invoke(popup);
            var window = TestWindow.Show(new StackPanel { Margin = new Thickness(0, 100, 0, 0), Children = { toggle, popup } }, 1200, 800);

            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            Assert.True(popup.IsOpen);
            PointerInput.Move(window, Middle(window, toggle));
            return (window, toggle, popup);
        }

        private static Point Middle(Window window, Control control) =>
            control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;

        // A point to the right of the toggle, "away" pixels from its right edge.
        private static Point Away(Window window, Control toggle, double away)
        {
            var left = toggle.TranslatePoint(new Point(0, 0), window)!.Value;
            return new Point(left.X + toggle.Bounds.Width + away, left.Y + 5);
        }

        [AvaloniaFact]
        public void ByDefault_ThePopupClosesWhenThePointerGoesAway()
        {
            var (window, toggle, popup) = Show();

            PointerInput.Move(window, Away(window, toggle, 200));

            Assert.False(popup.IsOpen);
        }

        [AvaloniaFact]
        public void WithoutClosesOnPointerAway_ThePopupStaysOpen()
        {
            var (window, toggle, popup) = Show(p => TogglePopup.SetClosesOnPointerAway(p, false));

            PointerInput.Move(window, Away(window, toggle, 600));

            Assert.True(popup.IsOpen);
        }

        [AvaloniaFact]
        public void WithoutClosesOnPointerAway_APressOutsideStillClosesIt()
        {
            var (window, toggle, popup) = Show(p => TogglePopup.SetClosesOnPointerAway(p, false));
            var outside = Away(window, toggle, 600);

            PointerInput.Click(window, outside);

            Assert.False(popup.IsOpen);
        }

        [AvaloniaFact]
        public void ABiggerDistance_KeepsThePopupOpenFartherAway()
        {
            var (window, toggle, popup) = Show(p => TogglePopup.SetPointerAwayDistance(p, 400));

            PointerInput.Move(window, Away(window, toggle, 300));
            Assert.True(popup.IsOpen);

            PointerInput.Move(window, Away(window, toggle, 700));
            Assert.False(popup.IsOpen);
        }

        [AvaloniaFact]
        public void TheSettings_CanChangeWhileThePopupIsOpen()
        {
            var (window, toggle, popup) = Show();

            TogglePopup.SetClosesOnPointerAway(popup, false);
            PointerInput.Move(window, Away(window, toggle, 600));

            Assert.True(popup.IsOpen);
        }

        [AvaloniaTheory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void TheListPanelItem_PassesTheSettingToItsAddPopup(bool closesOnPointerAway, bool expectedOpen)
        {
            var list = new ListPanelItem
            {
                AddCaption = "Add",
                AddContent = new TextBlock { Text = "Short" },
                AddClosesOnPointerAway = closesOnPointerAway,
                Width = 300,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var window = TestWindow.Show(new StackPanel { Margin = new Thickness(0, 100, 0, 0), Children = { list } }, 1200, 800);
            var toggle = list.FindControl<ToggleButton>("addToggle")!;
            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            PointerInput.Move(window, Middle(window, toggle));

            PointerInput.Move(window, Away(window, toggle, 600));

            Assert.Equal(expectedOpen, list.GetLogicalDescendants().OfType<Popup>().Single().IsOpen);
        }

        [AvaloniaTheory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void TheButtonPopup_PassesTheSettingToItsPopup(bool closesOnPointerAway, bool expectedOpen)
        {
            var button = new ButtonPopup
            {
                Caption = "Pick",
                PopupContent = new TextBlock { Text = "Short" },
                ClosesOnPointerAway = closesOnPointerAway,
                Width = 300,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var window = TestWindow.Show(new StackPanel { Margin = new Thickness(0, 100, 0, 0), Children = { button } }, 1200, 800);
            var toggle = button.FindControl<ToggleButton>("popupToggle")!;
            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            PointerInput.Move(window, Middle(window, toggle));

            PointerInput.Move(window, Away(window, toggle, 600));

            Assert.Equal(expectedOpen, button.GetLogicalDescendants().OfType<Popup>().Single().IsOpen);
        }
    }
}
