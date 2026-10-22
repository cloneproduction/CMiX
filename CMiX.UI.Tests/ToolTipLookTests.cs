using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;

namespace CMiX.UI.Tests
{
    // A tooltip has the color of the popups and the flyouts: the token PopupBackgroundBrush.
    public class ToolTipLookTests
    {
        [AvaloniaFact]
        public void ATooltip_HasThePopupBackground()
        {
            var tip = new ToolTip { Content = "Tip" };
            new Window { Content = tip, Width = 200, Height = 100 }.Show();
            Dispatcher.UIThread.RunJobs();

            var border = tip.GetVisualDescendants().OfType<Border>().First(part => part.Background != null && part.CornerRadius != default);

            var popup = ((ISolidColorBrush)Application.Current!.FindResource("PopupBackgroundBrush")!).Color;
            Assert.Equal(popup, ((ISolidColorBrush)border.Background!).Color);
        }
    }
}
