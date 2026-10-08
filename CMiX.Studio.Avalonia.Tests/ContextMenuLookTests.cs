using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Views.Controls;
using CommunityToolkit.Mvvm.Input;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The context menu theme shows its rows in a PopupFrame and gives the separator row a thin line.
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
    }
}
