using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.VisualTree;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The cards stand out from the surface behind them. That surface is the color of the main window.
    // The containers of the panels are transparent, so this one color decides.
    public class MainWindowSurfaceTests
    {
        private static int Brightness(IBrush? brush)
        {
            var color = ((ISolidColorBrush)brush!).Color;
            return color.R + color.G + color.B;
        }

        [AvaloniaFact]
        public void TheWindowSurface_IsDarkerThanTheCards()
        {
            var (_, window, _) = TestServiceProviderFactory.ShowMainWindow();
            var cards = Brightness((IBrush)Application.Current!.FindResource("PanelBackgroundLevel1Brush")!);

            Assert.True(Brightness(window.Background) < cards, "The window is not darker than the cards.");

            var mainPanel = window.GetVisualDescendants().OfType<DockPanel>().First(d => d.Background != null);
            Assert.True(Brightness(mainPanel.Background) < cards, "The main panel is not darker than the cards.");
        }
    }
}
