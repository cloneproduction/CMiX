using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.Threading;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Behaviors;
using Xunit;
using AvaloniaInteraction = Avalonia.Xaml.Interactivity.Interaction;

namespace CMiX.UI.Tests
{
    // A popup with TogglePopup.Toggle is open while its toggle button is checked, and it closes when the pointer goes away.
    public class TogglePopupTests
    {
        private static (Window Window, ToggleButton Toggle, Popup Popup) Show()
        {
            var toggle = new ToggleButton { Content = "open", Width = 100, Height = 30, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
            var popup = new Popup
            {
                Placement = PlacementMode.Bottom,
                Child = new Border { Width = 100, Height = 80, Child = new TextBlock { Text = "content" } },
            };
            TogglePopup.SetToggle(popup, toggle);
            var window = new Window { Content = new Panel { Children = { toggle, popup } }, Width = 500, Height = 400 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (window, toggle, popup);
        }

        [AvaloniaFact]
        public void CheckingTheToggle_OpensThePopup_AndUncheckingClosesIt()
        {
            var (_, toggle, popup) = Show();
            Assert.False(popup.IsOpen);

            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            Assert.True(popup.IsOpen);

            toggle.IsChecked = false;
            Dispatcher.UIThread.RunJobs();
            Assert.False(popup.IsOpen);
        }

        [AvaloniaFact]
        public void OpeningAndClosingThePopup_ChecksAndUnchecksTheToggle()
        {
            var (_, toggle, popup) = Show();

            popup.IsOpen = true;
            Dispatcher.UIThread.RunJobs();
            Assert.True(toggle.IsChecked);

            popup.IsOpen = false;
            Dispatcher.UIThread.RunJobs();
            Assert.False(toggle.IsChecked);
        }

        [AvaloniaFact]
        public void ThePopup_HasTheToggleAsTarget_NoLightDismiss_AndOneCloseBehavior()
        {
            var (_, toggle, popup) = Show();

            Assert.Same(toggle, popup.PlacementTarget);
            Assert.False(popup.IsLightDismissEnabled);
            Assert.Single(AvaloniaInteraction.GetBehaviors(popup).OfType<ClosePopupBehavior>());
        }

        [AvaloniaFact]
        public void SettingTheSameToggleAgain_AddsNoSecondBehavior()
        {
            var (_, toggle, popup) = Show();

            TogglePopup.SetToggle(popup, null);
            TogglePopup.SetToggle(popup, toggle);

            Assert.Single(AvaloniaInteraction.GetBehaviors(popup).OfType<ClosePopupBehavior>());
        }

        [AvaloniaFact]
        public void APointerFarAway_ClosesThePopup_AndUnchecksTheToggle()
        {
            var (window, toggle, popup) = Show();
            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            Assert.True(popup.IsOpen);

            window.MouseMove(new Point(490, 390));
            Dispatcher.UIThread.RunJobs();

            Assert.False(popup.IsOpen);
            Assert.False(toggle.IsChecked);
        }

        [AvaloniaFact]
        public void AfterTheToggleIsReplaced_TheOldToggleControlsNothing()
        {
            var (_, oldToggle, popup) = Show();
            var newToggle = new ToggleButton();

            TogglePopup.SetToggle(popup, newToggle);
            oldToggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            Assert.False(popup.IsOpen);

            newToggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            Assert.True(popup.IsOpen);
        }
    }
}
