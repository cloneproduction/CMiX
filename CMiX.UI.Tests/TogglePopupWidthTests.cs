using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Collection;
using Xunit;

namespace CMiX.UI.Tests
{
    // A popup is at least PopupMinWidth wide, or as wide as its toggle when MatchesToggleWidth is set.
    public class TogglePopupWidthTests
    {
        private static (ToggleButton Toggle, Popup Popup) Show(Control content, bool matchesToggle, double toggleWidth = 120)
        {
            var toggle = new ToggleButton { Content = "Open", Width = toggleWidth, HorizontalAlignment = HorizontalAlignment.Left };
            var popup = new Popup { Child = new Border { Child = content } };
            TogglePopup.SetToggle(popup, toggle);
            TogglePopup.SetMatchesToggleWidth(popup, matchesToggle);
            var panel = new StackPanel { Children = { toggle, popup } };
            new Window { Content = panel, Width = 900, Height = 500 }.Show();
            Dispatcher.UIThread.RunJobs();

            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            Assert.True(popup.IsOpen);
            return (toggle, popup);
        }

        [AvaloniaFact]
        public void ThePopupContent_IsAtLeastTheMinimumWidth()
        {
            var (_, popup) = Show(new TextBlock { Text = "Short" }, matchesToggle: false);

            Assert.Equal(200.0, popup.Child!.MinWidth);
            Assert.True(popup.Child.Bounds.Width >= 200);
        }

        [AvaloniaFact]
        public void WiderContent_MakesThePopupWiderThanTheMinimum()
        {
            var (_, popup) = Show(new Border { Width = 350, Height = 20 }, matchesToggle: false);

            Assert.Equal(350, popup.Child!.Bounds.Width);
        }

        [AvaloniaFact]
        public void WithMatchesToggleWidth_ThePopupIsAsWideAsTheToggle()
        {
            var (toggle, popup) = Show(new TextBlock { Text = "Short" }, matchesToggle: true);

            Assert.Equal(toggle.Bounds.Width, popup.Width);
            Assert.Equal(0.0, popup.Child!.MinWidth);
        }

        [AvaloniaFact]
        public void WithMatchesToggleWidth_ThePopupFollowsTheToggleWhenItChanges()
        {
            var (toggle, popup) = Show(new TextBlock { Text = "Short" }, matchesToggle: true, toggleWidth: 120);

            toggle.Width = 300;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(300, popup.Width);
        }

        [AvaloniaFact]
        public void TheAddPanel_IsAsWideAsItsAddToggle()
        {
            var list = new PanelItemList { AddCaption = "Add", AddContent = new TextBlock { Text = "Short" }, Width = 400, HorizontalAlignment = HorizontalAlignment.Left };
            new Window { Content = list, Width = 900, Height = 500 }.Show();
            Dispatcher.UIThread.RunJobs();

            var toggle = list.FindControl<ToggleButton>("AddToggle")!;
            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            var popup = (Popup)list.GetLogicalChildren().OfType<StackPanel>().Single().Children.OfType<Popup>().Single();
            Assert.Equal(400, popup.Width);
        }

        [AvaloniaFact]
        public void ThePopupButton_IsAsWideAsItsToggle()
        {
            var button = new PopupButton { Caption = "Pick", PopupContent = new TextBlock { Text = "Short" }, Width = 320, HorizontalAlignment = HorizontalAlignment.Left };
            new Window { Content = button, Width = 900, Height = 500 }.Show();
            Dispatcher.UIThread.RunJobs();

            var toggle = button.FindControl<ToggleButton>("PopUpToggle")!;
            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            var popup = TogglePopup.GetToggle(button.GetLogicalDescendants().OfType<Popup>().Single());
            Assert.Same(toggle, popup);
            Assert.Equal(320, button.GetLogicalDescendants().OfType<Popup>().Single().Width);
        }
    }
}
