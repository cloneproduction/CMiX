using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.Views;
using CMiX.Studio.Avalonia.Views.Modulation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The flyouts of the sync settings close when the pointer goes far away from them.
    public class FlyoutPointerAwayTests
    {
        private static void MoveFarAway(Window window)
        {
            window.MouseMove(new Point(window.Width - 10, window.Height - 10));
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaTheory]
        [InlineData("syncButton")]
        [InlineData("editConnectionButton")]
        public void MouseMove_FarFromAFlyoutOfTheSyncSettings_ClosesTheFlyout(string buttonName)
        {
            var provider = TestServiceProviderFactory.Create();
            var peer = provider.GetRequiredService<SyncPeer>();
            var view = new SettingsSync { DataContext = provider.GetRequiredService<SettingsSyncViewModel>() };
            var window = new Window { Content = view, Width = 1200, Height = 1000 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var button = view.FindControl<Button>(buttonName)!;
            button.Flyout!.ShowAt(button);
            TestServiceProviderFactory.Pump();
            Assert.True(button.Flyout.IsOpen);

            MoveFarAway(window);

            Assert.False(button.Flyout.IsOpen);
            peer.Dispose();
            window.Close();
        }
    }
}
