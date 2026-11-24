// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Networking;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The bottom bar of the main window shows the state of the connection and the state of the sync, as the Network tab does.
    // The sync dots have the same colors in the main window and in the sync settings.
    public class BadgeSyncStatusTests : IDisposable
    {
        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

        private static IEnumerable<TextBlock> VisibleTexts(Control view) =>
            view.GetVisualDescendants().OfType<TextBlock>().Where(text => text.IsEffectivelyVisible);

        [AvaloniaFact]
        public void TheIndicator_ShowsTheStatusOfThePeerOnce_LikeTheNetworkTab()
        {
            var provider = TestServiceProviderFactory.Create();
            var peer = provider.GetRequiredService<SyncPeer>();
            var indicator = new BadgeSyncStatus { DataContext = peer };
            var network = new SettingsSync { DataContext = provider.GetRequiredService<SettingsSyncViewModel>() };
            windows.Add(new Window { Content = new StackPanel { Children = { indicator, network } }, Width = 400, Height = 600 }).Show();
            Dispatcher.UIThread.RunJobs();

            var texts = VisibleTexts(indicator).Select(text => text.Text).ToList();

            Assert.Equal(new[] { peer.Status }, texts);
            Assert.Contains(peer.Status, VisibleTexts(network).Select(text => text.Text));
            Assert.Single(indicator.GetVisualDescendants().OfType<Ellipse>());

            peer.Dispose();
        }

        // The status says "Not in sync" when the peer is connected and has not joined. The bar must not say it twice.
        [AvaloniaFact]
        public void Show_TheBottomBar_HasOneStatusText()
        {
            var (_, window, viewModel) = windows.ShowMainWindow();
            var bar = window.GetVisualDescendants().OfType<BadgeSyncStatus>().Single();

            var shown = VisibleTexts(bar).Select(text => text.Text).ToList();

            Assert.Equal(new[] { viewModel.Sync.Status }, shown);
        }

        [AvaloniaFact]
        public void TheBottomBarOfTheMainWindow_ShowsTheIndicators_AndNoNameOfTheProgramOrTheCompany()
        {
            var (_, window, _) = windows.ShowMainWindow();

            Assert.Single(window.GetVisualDescendants().OfType<BadgeSyncStatus>());
            var texts = window.GetVisualDescendants().OfType<TextBlock>().Select(text => text.Text).ToList();
            Assert.DoesNotContain("CMiX Studio", texts);
            Assert.DoesNotContain("CloneProduction", texts);
        }

        [AvaloniaFact]
        public void Show_TheDot_FollowsTheConnectionAndTheSyncStateOfThePeer()
        {
            var peer = TestServiceProviderFactory.Create().GetRequiredService<SyncPeer>();
            var indicator = new BadgeSyncStatus { DataContext = peer };
            windows.Add(new Window { Content = indicator, Width = 300, Height = 100 }).Show();
            Dispatcher.UIThread.RunJobs();
            var dot = indicator.GetVisualDescendants().OfType<Ellipse>().Single();

            Assert.Equal(peer.IsConnected, dot.Classes.Contains("connected"));
            Assert.Equal(!peer.IsInSync, dot.Classes.Contains("unsynced"));

            peer.Dispose();
        }

        private Ellipse Dot(string themeKey)
        {
            var dot = new Ellipse
            {
                Width = 10,
                Height = 10,
                Theme = (ControlTheme)Application.Current!.FindResource(themeKey)!
            };
            var window = windows.Add(new Window { Content = dot, Width = 50, Height = 50 });
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return dot;
        }

        // A color of the theme (Colors.axaml).
        private static Color Token(string key) => (Color)Application.Current!.FindResource(key)!;

        private static Color ColorOf(Ellipse dot)
        {
            Dispatcher.UIThread.RunJobs();
            return ((ISolidColorBrush)dot.Fill!).Color;
        }

        [AvaloniaFact]
        public void ConnectionDot_NoConnection_IsRed()
        {
            Assert.Equal(Token("PopupCardDanger"), ColorOf(Dot("EllipseSyncConnection")));
        }

        [AvaloniaFact]
        public void ConnectionDot_Connected_IsGreen()
        {
            var dot = Dot("EllipseSyncConnection");
            dot.Classes.Add("connected");

            Assert.Equal(Token("StatusSuccess"), ColorOf(dot));
        }

        [AvaloniaFact]
        public void ConnectionDot_IsGrayWhileConnecting_EvenWhenConnected()
        {
            var dot = Dot("EllipseSyncConnection");
            dot.Classes.Add("connected");
            dot.Classes.Add("connecting");

            Assert.Equal(Token("Grey800"), ColorOf(dot));
        }

        [AvaloniaFact]
        public void ConnectionDot_ConnectedButNotInSync_IsOrange()
        {
            var dot = Dot("EllipseSyncConnection");
            dot.Classes.Add("connected");
            dot.Classes.Add("unsynced");

            Assert.Equal(Token("PopupCardWarning"), ColorOf(dot));
        }

        [AvaloniaFact]
        public void ConnectionDot_IsRed_WhenNotInSyncAndNotConnected()
        {
            var dot = Dot("EllipseSyncConnection");
            dot.Classes.Add("unsynced");

            Assert.Equal(Token("PopupCardDanger"), ColorOf(dot));
        }

        [AvaloniaFact]
        public void ConnectionDot_IsGrayWhileConnecting_EvenWhenNotInSync()
        {
            var dot = Dot("EllipseSyncConnection");
            dot.Classes.Add("connected");
            dot.Classes.Add("unsynced");
            dot.Classes.Add("connecting");

            Assert.Equal(Token("Grey800"), ColorOf(dot));
        }

        // A dot is a small shape with no text: it needs a contrast of 3:1 or more on the window and on the panels.
        [AvaloniaTheory]
        [InlineData("StatusDisconnectedBrush")]
        [InlineData("StatusConnectedBrush")]
        [InlineData("StatusUnsyncedBrush")]
        [InlineData("StatusPendingBrush")]
        [InlineData("StatusSyncedBrush")]
        public void ADot_OnTheWindowAndOnAPanel_CanBeSeen(string brush)
        {
            var dot = ((ISolidColorBrush)Application.Current!.FindResource(brush)!).Color;

            foreach (var surface in new[] { "Grey100", "Grey200", "Grey300", "Grey400" })
                Assert.True(CMiX.UI.Mathematics.ColorMix.Contrast(dot, Token(surface)) >= 3, $"{brush} on {surface}: {CMiX.UI.Mathematics.ColorMix.Contrast(dot, Token(surface)):0.00}");
        }

        [AvaloniaFact]
        public void StateDot_IsGrayUntilInSync_ThenBlue()
        {
            var dot = Dot("EllipseSyncState");
            Assert.Equal(Token("Grey800"), ColorOf(dot));

            dot.Classes.Add("insync");

            Assert.Equal(Token("AccentPrimary"), ColorOf(dot));
        }
    }
}
