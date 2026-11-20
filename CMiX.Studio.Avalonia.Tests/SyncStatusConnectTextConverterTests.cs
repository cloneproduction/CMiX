// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Globalization;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using CMiX.Core.Networking;
using CMiX.Studio.Avalonia.Converters;
using CMiX.Studio.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class SyncStatusConnectTextConverterTests
    {
        private static object Convert(object? status) =>
            new SyncStatusConnectTextConverter().Convert(status, typeof(string), null, CultureInfo.InvariantCulture);

        [Fact]
        public void Convert_Connecting_GivesConnectingWithDots()
        {
            var text = Convert("Connecting");

            Assert.Equal("Connecting...", text);
        }

        [Theory]
        [InlineData("Not started")]
        [InlineData("Reconnecting")]
        [InlineData("Connected")]
        [InlineData("Not in sync")]
        [InlineData(null)]
        public void Convert_AnyOtherStatus_GivesConnect(string? status)
        {
            var text = Convert(status);

            Assert.Equal("Connect", text);
        }

        [AvaloniaFact]
        public void ConnectButton_APeerNotStarted_ReadsConnect()
        {
            var provider = TestServiceProviderFactory.Create();
            var peer = provider.GetRequiredService<SyncPeer>();
            var view = new SettingsSync { DataContext = peer };
            var window = new Window { Content = view, Width = 400, Height = 600 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var button = view.FindControl<Button>("editConnectionButton")!;
            button.Flyout!.ShowAt(button);
            TestServiceProviderFactory.Pump();
            var content = view.FindControl<Button>("connectButton")!.Content;

            Assert.Equal("Connect", content);
            button.Flyout.Hide();
            peer.Dispose();
            window.Close();
        }
    }
}
