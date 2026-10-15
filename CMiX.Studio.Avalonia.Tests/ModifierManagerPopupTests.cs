using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Views;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The add popup of a modifier panel does not close when the pointer goes away. Other popups do.
    public class ModifierManagerPopupTests
    {
        private static (Window Window, ToggleButton Toggle, Popup Popup) ShowWithOpenPopup(Action<ModifierManager>? configure = null)
        {
            var manager = new ModifierManager
            {
                Caption = "Add",
                SelectionPanel = new TextBlock { Text = "Modifiers" },
                Width = 300,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            configure?.Invoke(manager);
            var window = new Window { Content = new StackPanel { Margin = new Thickness(0, 100, 0, 0), Children = { manager } }, Width = 1200, Height = 800 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var toggle = manager.GetVisualDescendants().OfType<ToggleButton>().First(t => t.Name == "AddToggle");
            var popup = manager.GetVisualDescendants().OfType<Popup>().First();
            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            Assert.True(popup.IsOpen);

            window.MouseMove(toggle.TranslatePoint(new Point(10, 10), window)!.Value);
            Dispatcher.UIThread.RunJobs();
            return (window, toggle, popup);
        }

        private static void MoveFarAway(Window window, ToggleButton toggle)
        {
            var left = toggle.TranslatePoint(new Point(0, 0), window)!.Value;
            window.MouseMove(new Point(left.X + toggle.Bounds.Width + 600, left.Y + 5));
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void TheAddPopup_StaysOpen_WhenThePointerGoesAway()
        {
            var (window, toggle, popup) = ShowWithOpenPopup();

            MoveFarAway(window, toggle);

            Assert.True(popup.IsOpen);
        }

        [AvaloniaFact]
        public void TheAddPopup_ClosesOnAPressOutsideIt()
        {
            var (window, toggle, popup) = ShowWithOpenPopup();
            var left = toggle.TranslatePoint(new Point(0, 0), window)!.Value;
            var outside = new Point(left.X + toggle.Bounds.Width + 600, left.Y + 5);

            window.MouseMove(outside);
            window.MouseDown(outside, MouseButton.Left);
            window.MouseUp(outside, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();

            Assert.False(popup.IsOpen);
        }

        [AvaloniaFact]
        public void WithAddClosesOnPointerAway_TheAddPopupClosesLikeTheOthers()
        {
            var (window, toggle, popup) = ShowWithOpenPopup(manager => manager.AddClosesOnPointerAway = true);

            MoveFarAway(window, toggle);

            Assert.False(popup.IsOpen);
        }
    }
}
