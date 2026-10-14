using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using CMiX.UI.Controls;
using Xunit;

namespace CMiX.UI.Tests
{
    // A click must not make the view scroll. A click gives focus to a control, and a pick selects a
    // list item. By default Avalonia scrolls to show both. The theme turns this off.
    public class ScrollStabilityTests
    {
        private static (Window window, ScrollViewer scroll, List<Button> buttons) ShowButtons(bool? bringIntoView = null)
        {
            var panel = new StackPanel();
            var buttons = Enumerable.Range(0, 20).Select(i => new Button { Content = "button " + i, Height = 50 }).ToList();
            foreach (var button in buttons)
                panel.Children.Add(button);

            var scroll = new ScrollViewer { Content = panel };
            if (bringIntoView.HasValue)
                scroll.BringIntoViewOnFocusChange = bringIntoView.Value;

            var window = new Window { Content = scroll, Width = 300, Height = 200 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (window, scroll, buttons);
        }

        [AvaloniaFact]
        public void AControlBelowTheView_GettingFocus_DoesNotScrollTheView()
        {
            var (_, scroll, buttons) = ShowButtons();

            buttons[19].Focus();
            Dispatcher.UIThread.RunJobs();

            Assert.False(scroll.BringIntoViewOnFocusChange);
            Assert.Equal(0, scroll.Offset.Y);
        }

        [AvaloniaFact]
        public void ASingleScrollViewer_CanTurnTheScrollOnFocusBackOn()
        {
            var (_, scroll, buttons) = ShowButtons(bringIntoView: true);

            buttons[19].Focus();
            Dispatcher.UIThread.RunJobs();

            Assert.True(scroll.Offset.Y > 0, "With the setting on, the view should scroll to the focused button. If it does not, the test above proves nothing.");
        }

        [AvaloniaFact]
        public void APickInAListBox_DoesNotScrollTheListOrTheViewAroundIt()
        {
            var list = new ListBoxCMiX { Height = 120, ItemsSource = Enumerable.Range(0, 30).Select(i => "item " + i).ToList() };
            var outer = new ScrollViewer { Content = new StackPanel { Children = { new Button { Height = 300 }, list, new Button { Height = 300 } } } };
            new Window { Content = outer, Width = 300, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();

            list.SelectedIndex = 29;
            Dispatcher.UIThread.RunJobs();

            Assert.False(list.AutoScrollToSelectedItem);
            Assert.Equal(0, outer.Offset.Y);
        }

        [AvaloniaFact]
        public void ThePlainListBox_DoesNotAutoScrollEither()
        {
            var list = new ListBox { Height = 120, ItemsSource = new[] { "a", "b" } };
            new Window { Content = list, Width = 300, Height = 200 }.Show();
            Dispatcher.UIThread.RunJobs();

            Assert.False(list.AutoScrollToSelectedItem);
        }

        [AvaloniaFact]
        public void ATextBox_KeepsScrollingToItsFocus()
        {
            var box = new TextBox();
            new Window { Content = box, Width = 300, Height = 200 }.Show();
            Dispatcher.UIThread.RunJobs();

            // The template of a text box binds its own setting, so the theme does not change it.
            Assert.True(ScrollViewer.GetBringIntoViewOnFocusChange(box));
        }
    }
}
