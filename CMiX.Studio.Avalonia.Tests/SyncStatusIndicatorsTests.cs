using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Networking;
using CMiX.Studio.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The bottom bar of the main window shows the state of the connection and the state of the sync, as the Network tab does.
    public class SyncStatusIndicatorsTests
    {
        private static IEnumerable<TextBlock> VisibleTexts(Control view) =>
            view.GetVisualDescendants().OfType<TextBlock>().Where(text => text.IsEffectivelyVisible);

        [AvaloniaFact]
        public void TheIndicators_ShowTheStatusAndTheSyncStateOfThePeer_LikeTheNetworkTab()
        {
            var peer = TestServiceProviderFactory.Create().GetRequiredService<SyncPeer>();
            var indicators = new SyncStatusIndicators { DataContext = peer };
            var network = new SyncSettings { DataContext = peer };
            new Window { Content = new StackPanel { Children = { indicators, network } }, Width = 400, Height = 600 }.Show();
            Dispatcher.UIThread.RunJobs();

            var texts = VisibleTexts(indicators).Select(text => text.Text).ToList();

            Assert.Contains(peer.Status, texts);
            Assert.Contains(peer.IsInSync ? "In sync" : "Not in sync", texts);
            Assert.Contains(peer.Status, VisibleTexts(network).Select(text => text.Text));
            Assert.Contains(peer.IsInSync ? "In sync" : "Not in sync", VisibleTexts(network).Select(text => text.Text));
            Assert.Equal(2, indicators.GetVisualDescendants().OfType<Ellipse>().Count());

            peer.Dispose();
        }

        [AvaloniaFact]
        public void TheBottomBarOfTheMainWindow_ShowsTheIndicators_AndNoNameOfTheProgramOrTheCompany()
        {
            var (_, window, _) = TestServiceProviderFactory.ShowMainWindow();

            Assert.Single(window.GetVisualDescendants().OfType<SyncStatusIndicators>());
            var texts = window.GetVisualDescendants().OfType<TextBlock>().Select(text => text.Text).ToList();
            Assert.DoesNotContain("CMiX Studio", texts);
            Assert.DoesNotContain("CloneProduction", texts);
        }
    }
}
