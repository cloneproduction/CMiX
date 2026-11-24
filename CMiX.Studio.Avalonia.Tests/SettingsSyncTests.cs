using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using CMiX.Core.Networking;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls.Boolean;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class SettingsSyncTests
    {
        [AvaloniaFact]
        public void Show_TheCredentialNameAndToggle_DisableUserAndPassword()
        {
            var provider = TestServiceProviderFactory.Create();
            var peer = provider.GetRequiredService<SyncPeer>();
            var view = new SettingsSync { DataContext = provider.GetRequiredService<SettingsSyncViewModel>() };
            var window = new Window { Content = view, Width = 400, Height = 600 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var button = view.FindControl<Button>("editConnectionButton")!;
            button.Flyout!.ShowAt(button);
            TestServiceProviderFactory.Pump();

            var toggle = view.FindControl<ToggleButtonCaptioned>("useCredentialManagerBox");
            var nameBox = view.FindControl<TextBox>("credentialNameBox");
            var userBox = view.FindControl<TextBox>("userBox");
            var passwordBox = view.FindControl<TextBox>("passwordBox");
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

        [AvaloniaTheory]
        [InlineData("syncButton")]
        [InlineData("editConnectionButton")]
        public void Press_EscapeInAFlyout_ClosesTheFlyout(string buttonName)
        {
            var provider = TestServiceProviderFactory.Create();
            var peer = provider.GetRequiredService<SyncPeer>();
            var view = new SettingsSync { DataContext = provider.GetRequiredService<SettingsSyncViewModel>() };
            var window = new Window { Content = view, Width = 400, Height = 600 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var button = view.FindControl<Button>(buttonName)!;
            button.Flyout!.ShowAt(button);
            TestServiceProviderFactory.Pump();
            Assert.True(button.Flyout.IsOpen);

            window.KeyPress(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, null);
            window.KeyRelease(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, null);
            TestServiceProviderFactory.Pump();

            Assert.False(button.Flyout.IsOpen);
            peer.Dispose();
            window.Close();
        }

        [AvaloniaFact]
        public void Connect_ABadPort_ShowsTheErrorText()
        {
            var provider = TestServiceProviderFactory.Create();
            var peer = provider.GetRequiredService<SyncPeer>();
            var viewModel = provider.GetRequiredService<SettingsSyncViewModel>();
            var view = new SettingsSync { DataContext = viewModel };
            var window = new Window { Content = view, Width = 400, Height = 600 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var button = view.FindControl<Button>("editConnectionButton")!;
            button.Flyout!.ShowAt(button);
            TestServiceProviderFactory.Pump();
            var errorText = view.FindControl<TextBlock>("errorText")!;

            viewModel.Port = "abc";
            viewModel.ConnectCommand.Execute(null);
            TestServiceProviderFactory.Pump();

            Assert.True(errorText.IsEffectivelyVisible);
            Assert.Equal("The port must be a whole number above zero.", errorText.Text);

            button.Flyout.Hide();
            peer.Dispose();
            window.Close();
        }

        [AvaloniaFact]
        public void LoadConnection_AfterAnError_ClearsTheErrorText()
        {
            var provider = TestServiceProviderFactory.Create();
            var peer = provider.GetRequiredService<SyncPeer>();
            var viewModel = provider.GetRequiredService<SettingsSyncViewModel>();
            var view = new SettingsSync { DataContext = viewModel };
            var window = new Window { Content = view, Width = 400, Height = 600 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var button = view.FindControl<Button>("editConnectionButton")!;
            button.Flyout!.ShowAt(button);
            TestServiceProviderFactory.Pump();
            var errorText = view.FindControl<TextBlock>("errorText")!;
            var errorPanel = (StackPanel)errorText.Parent!;
            viewModel.Port = "abc";
            viewModel.ConnectCommand.Execute(null);
            TestServiceProviderFactory.Pump();
            Assert.NotEmpty(errorText.Text!);

            viewModel.LoadConnectionCommand.Execute(null);
            TestServiceProviderFactory.Pump();

            Assert.Empty(errorText.Text!);
            Assert.True(errorPanel.Bounds.Height >= 40);

            button.Flyout.Hide();
            peer.Dispose();
            window.Close();
        }
    }
}
