using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Views.Controls;
using CommunityToolkit.Mvvm.Input;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The context menu theme shows its rows in a PopupFrame, gives the separator row a thin line, and opens with its middle on the click.
    public class ContextMenuLookTests
    {
        private static Window Host(Control content)
        {
            var window = new Window { Content = content, Width = 400, Height = 300 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return window;
        }

        [AvaloniaFact]
        public void ResetMenuOfASlider_WhenOpen_ShowsItsItemInAPopupFrame()
        {
            var slider = new CMiXSlider { Width = 200, Height = 30, ResetCommand = new RelayCommand(() => { }) };
            Host(slider);

            slider.ContextMenu!.Open(slider);
            Dispatcher.UIThread.RunJobs();

            var reset = slider.ContextMenu.Items.OfType<MenuItem>().Single(i => Equals(i.Header, "Reset"));
            Assert.NotNull(reset.FindAncestorOfType<PopupFrame>());
        }

        [AvaloniaFact]
        public void ContextMenu_WithADashItem_ShowsAThinSeparatorRow()
        {
            var menu = new ContextMenu
            {
                Theme = (ControlTheme)Application.Current!.FindResource("ContextMenuDefault")!,
                ItemsSource = new[]
                {
                    new MenuItem { Header = "A" },
                    new MenuItem { Header = "-" },
                    new MenuItem { Header = "B" }
                }
            };
            var owner = new Border { Width = 200, Height = 30, ContextMenu = menu };
            Host(owner);

            menu.Open(owner);
            Dispatcher.UIThread.RunJobs();

            var row = menu.GetVisualDescendants().OfType<MenuItem>().Single(i => Equals(i.Header, "-"));
            var line = row.GetVisualDescendants().OfType<Separator>().Single();
            Assert.Equal(0.5, line.Height);
            Assert.True(row.Bounds.Height < 12, $"The separator row is {row.Bounds.Height} px high.");
        }

        private static ContextMenu NewMenu(params string[] headers) => new()
        {
            Theme = (ControlTheme)Application.Current!.FindResource("ContextMenuDefault")!,
            ItemsSource = headers.Select(h => new MenuItem { Header = h }).ToArray()
        };

        // The middle of the open menu, in window coordinates.
        private static Point MiddleOf(Window window, ContextMenu menu)
        {
            var frame = menu.GetVisualDescendants().OfType<PopupFrame>().First();
            return frame.TranslatePoint(new Point(frame.Bounds.Width / 2, frame.Bounds.Height / 2), window)!.Value;
        }

        private static void AssertMiddleOn(Point expected, Point actual)
        {
            Assert.InRange(actual.X, expected.X - 1, expected.X + 1);
            Assert.InRange(actual.Y, expected.Y - 1, expected.Y + 1);
        }

        private static (Window Window, Border Owner) HostOwner(ContextMenu menu)
        {
            var owner = new Border { Width = 600, Height = 400, Background = global::Avalonia.Media.Brushes.Gray, ContextMenu = menu };
            var window = new Window { Content = owner, Width = 800, Height = 600 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (window, owner);
        }

        [AvaloniaTheory]
        [InlineData("Reset")]
        [InlineData("New Folder", "Rename this very long item", "Delete")]
        public void ContextMenu_OpenedByARightClick_HasItsMiddleOnTheClick(params string[] headers)
        {
            var menu = NewMenu(headers);
            var (window, owner) = HostOwner(menu);
            var click = new Point(300, 100);

            PointerInput.Click(window, owner, MouseButton.Right, click);

            Assert.True(menu.IsOpen);
            AssertMiddleOn(click, MiddleOf(window, menu));
        }

        [AvaloniaFact]
        public void ContextMenu_OpenedASecondTime_IsCenteredAgain()
        {
            var menu = NewMenu("Reset");
            var (window, owner) = HostOwner(menu);

            PointerInput.Click(window, owner, MouseButton.Right, new Point(300, 100));
            menu.Close();
            Dispatcher.UIThread.RunJobs();
            PointerInput.Click(window, owner, MouseButton.Right, new Point(200, 150));

            Assert.True(menu.IsOpen);
            AssertMiddleOn(new Point(200, 150), MiddleOf(window, menu));
        }
    }
}
