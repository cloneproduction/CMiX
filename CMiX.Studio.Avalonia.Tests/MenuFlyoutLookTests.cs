using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Views.Controls;
using CommunityToolkit.Mvvm.Input;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // A MenuFlyout shows its rows in the same frame and with the same row theme as a context menu.
    public class MenuFlyoutLookTests
    {
        public sealed class Choice
        {
            public string Label { get; init; } = "";
            public bool IsAssigned { get; init; }
            public ICommand? Command { get; init; }
        }

        private static ControlTheme Theme(string key) => (ControlTheme)Application.Current!.FindResource(key)!;

        // The same shape as the item theme of the assign popup: the row theme plus bindings to the item.
        private static ControlTheme ChoiceRowTheme()
        {
            var theme = new ControlTheme(typeof(MenuItem)) { BasedOn = Theme("ContextMenuSubItem") };
            theme.Setters.Add(new Setter(MenuItem.HeaderProperty, new Binding("Label")));
            theme.Setters.Add(new Setter(MenuItem.IsCheckedProperty, new Binding("IsAssigned")));
            theme.Setters.Add(new Setter(MenuItem.CommandProperty, new Binding("Command")));
            return theme;
        }

        private static (Window Window, MenuFlyout Flyout, Control Presenter) Open(params Choice[] choices)
        {
            var flyout = new MenuFlyout
            {
                FlyoutPresenterTheme = Theme("MenuFlyoutPresenterDefault"),
                ItemContainerTheme = ChoiceRowTheme(),
                ItemsSource = choices,
            };
            var button = new Button { Content = "Assign", Width = 100, Height = 30, Flyout = flyout };
            var window = new Window { Content = button, Width = 600, Height = 400 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            flyout.ShowAt(button);
            Dispatcher.UIThread.RunJobs();

            var presenter = window.GetVisualDescendants().OfType<MenuFlyoutPresenter>().Single();
            return (window, flyout, presenter);
        }

        private static Border RootOf(MenuItem row) => row.GetVisualDescendants().OfType<Border>().First(b => b.Name == "root");

        private static IEnumerable<MenuItem> Rows(Control presenter) => presenter.GetVisualDescendants().OfType<MenuItem>();

        [AvaloniaFact]
        public void MenuFlyout_ShowsItsRowsInAPopupFrame()
        {
            var (_, _, presenter) = Open(new Choice { Label = "A" });

            Assert.Single(presenter.GetVisualDescendants().OfType<PopupFrame>());
            Assert.All(Rows(presenter), row => Assert.NotNull(row.FindAncestorOfType<PopupFrame>()));
        }

        [AvaloniaFact]
        public void MenuFlyout_ShowsTheLabelOfEachItem()
        {
            var (_, _, presenter) = Open(new Choice { Label = "LFO Sine" }, new Choice { Label = "Random Value" });

            Assert.Equal(new object?[] { "LFO Sine", "Random Value" }, Rows(presenter).Select(r => r.Header).ToArray());
        }

        [AvaloniaFact]
        public void MenuFlyout_MarksTheAssignedItemAsChecked()
        {
            var (_, _, presenter) = Open(new Choice { Label = "A", IsAssigned = true }, new Choice { Label = "B" });

            var rows = Rows(presenter).ToList();
            Assert.True(rows[0].Classes.Contains(":checked"));
            Assert.False(rows[1].Classes.Contains(":checked"));
            Assert.Equal(((ISolidColorBrush)Application.Current!.FindResource("AccentInactiveBrush")!).Color, ((ISolidColorBrush)RootOf(rows[0]).Background!).Color);
            Assert.NotEqual(((ISolidColorBrush)Application.Current!.FindResource("AccentInactiveBrush")!).Color, ((ISolidColorBrush)RootOf(rows[1]).Background!).Color);
        }

        [AvaloniaFact]
        public void MenuFlyout_ClickOnAnItem_RunsItsCommand_AndClosesTheFlyout()
        {
            var ran = false;
            var (window, flyout, presenter) = Open(new Choice { Label = "A", Command = new RelayCommand(() => ran = true) });

            PointerInput.Click(window, Rows(presenter).Single(), MouseButton.Left);

            Assert.True(ran);
            Assert.False(flyout.IsOpen);
        }

        [AvaloniaFact]
        public void MenuFlyout_ItemWithADashLabel_IsAThinSeparatorRow()
        {
            var (_, _, presenter) = Open(new Choice { Label = "A" }, new Choice { Label = "-" }, new Choice { Label = "B" });

            var row = Rows(presenter).Single(r => Equals(r.Header, "-"));
            Assert.Equal(0.5, row.GetVisualDescendants().OfType<Separator>().Single().Height);
            Assert.True(row.Bounds.Height < 12);
        }
    }
}
