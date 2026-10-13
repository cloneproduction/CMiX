using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using CMiX.Core.Networking;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class SyncSettingsTests
    {
        [AvaloniaFact]
        public void CredentialNameAndToggle_DisableUserAndPassword()
        {
            var provider = TestServiceProviderFactory.Create();
            var peer = provider.GetRequiredService<SyncPeer>();
            var view = new SyncSettings { DataContext = peer };
            var window = new Window { Content = view, Width = 400, Height = 600 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var button = view.FindControl<Button>("EditConnectionButton")!;
            button.Flyout!.ShowAt(button);
            TestServiceProviderFactory.Pump();

            var toggle = view.FindControl<CaptionedToggleButton>("UseCredentialManagerBox");
            var nameBox = view.FindControl<TextBox>("CredentialNameBox");
            var userBox = view.FindControl<TextBox>("UserBox");
            var passwordBox = view.FindControl<TextBox>("PasswordBox");
            Assert.NotNull(toggle);
            Assert.NotNull(nameBox);
            Assert.NotNull(userBox);
            Assert.NotNull(passwordBox);

            toggle!.IsChecked = true;
            nameBox!.Text = "MyRedis";
            TestServiceProviderFactory.Pump();
            Assert.False(userBox!.IsEnabled);
            Assert.False(passwordBox!.IsEnabled);

            nameBox.Text = "";
            TestServiceProviderFactory.Pump();
            Assert.True(userBox.IsEnabled);
            Assert.True(passwordBox.IsEnabled);

            nameBox.Text = "MyRedis";
            toggle.IsChecked = false;
            TestServiceProviderFactory.Pump();
            Assert.True(userBox.IsEnabled);
            Assert.True(passwordBox.IsEnabled);

            button.Flyout.Hide();
            peer.Dispose();
            window.Close();
        }
    }
}
