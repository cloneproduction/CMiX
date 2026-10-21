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
    public class SyncStatusIndicatorTests
    {
        private static IEnumerable<TextBlock> VisibleTexts(Control view) =>
            view.GetVisualDescendants().OfType<TextBlock>().Where(text => text.IsEffectivelyVisible);

        [AvaloniaFact]
        public void TheIndicator_ShowsTheStatusOfThePeerOnce_LikeTheNetworkTab()
        {
            var peer = TestServiceProviderFactory.Create().GetRequiredService<SyncPeer>();
            var indicator = new SyncStatusIndicator { DataContext = peer };
            var network = new SyncSettings { DataContext = peer };
            new Window { Content = new StackPanel { Children = { indicator, network } }, Width = 400, Height = 600 }.Show();
            Dispatcher.UIThread.RunJobs();

            var texts = VisibleTexts(indicator).Select(text => text.Text).ToList();

            Assert.Equal(new[] { peer.Status }, texts);
            Assert.Contains(peer.Status, VisibleTexts(network).Select(text => text.Text));
            Assert.Single(indicator.GetVisualDescendants().OfType<Ellipse>());

            peer.Dispose();
        }

        // The status says "Not in sync" when the peer is connected and has not joined. The bar must not say it twice.
        [AvaloniaFact]
        public void TheBottomBar_ShowsOneStatusText()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            var bar = window.GetVisualDescendants().OfType<SyncStatusIndicator>().Single();

            var shown = VisibleTexts(bar).Select(text => text.Text).ToList();

            Assert.Equal(new[] { viewModel.Sync.Status }, shown);
        }

        [AvaloniaFact]
        public void TheBottomBarOfTheMainWindow_ShowsTheIndicators_AndNoNameOfTheProgramOrTheCompany()
        {
            var (_, window, _) = TestServiceProviderFactory.ShowMainWindow();

            Assert.Single(window.GetVisualDescendants().OfType<SyncStatusIndicator>());
            var texts = window.GetVisualDescendants().OfType<TextBlock>().Select(text => text.Text).ToList();
            Assert.DoesNotContain("CMiX Studio", texts);
            Assert.DoesNotContain("CloneProduction", texts);
        }
    }
}
