// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Services;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls.Boolean;
using CMiX.UI.Panels;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The Settings tab: its panels, the closed Shortcuts panel and the key hints toggle.
    public class TabSettingsTests : IDisposable
    {
        private Window? window;

        public void Dispose() => window?.Close();

        [AvaloniaFact]
        public void Show_TheTab_HasTheOutputMappingsTheRedisAddressesTheInterfaceAndTheShortcuts()
        {
            var tab = new TabSettings();
            window = new Window { Content = tab, Width = 400, Height = 400 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var headers = tab.GetVisualDescendants().OfType<ExpanderIndented>()
                .Where(expander => expander.IsEffectivelyVisible)
                .Select(expander => expander.Header);

            Assert.Equal(new object[] { "Output Mappings", "Redis Addresses", "Interface", "Shortcuts" }, headers);
        }

        [AvaloniaFact]
        public void Show_TheTab_HasTheShortcutsExpanderClosed()
        {
            (_, var mainWindow, var viewModel) = TestServiceProviderFactory.ShowMainWindow();
            window = mainWindow;
            TestServiceProviderFactory.SelectMainTab(mainWindow, "Settings");

            var tab = mainWindow.GetVisualDescendants().OfType<TabSettings>().Single();
            var expander = tab.GetVisualDescendants().OfType<ExpanderIndented>().Single(e => Equals(e.Header, "Shortcuts"));

            Assert.False(expander.IsExpanded);

            // A closed expander does not attach its content. The binding resolves when it opens.
            expander.IsExpanded = true;
            TestServiceProviderFactory.Pump();
            Assert.Same(viewModel.ShortcutSettings, ((Control)expander.Content!).DataContext);
        }

        [AvaloniaFact]
        public void Show_TheTab_HasTheKeyHintsToggleOnTheInterfaceOptions()
        {
            var (provider, mainWindow, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            window = mainWindow;
            TestServiceProviderFactory.SelectMainTab(mainWindow, "Settings");

            var tab = mainWindow.GetVisualDescendants().OfType<TabSettings>().Single();
            var toggle = tab.GetVisualDescendants().OfType<ToggleButtonCaptioned>().Single(t => t.Name == "showKeyHintsToggle");

            Assert.Same(provider.GetRequiredService<InterfaceOptions>(), viewModel.Interface);
            Assert.True(toggle.IsChecked);

            toggle.IsChecked = false;
            TestServiceProviderFactory.Pump();

            Assert.False(viewModel.Interface.ShowKeyHints);
            Assert.False(mainWindow.GetVisualDescendants().OfType<ListTreeProject>().First().HintStrip.IsVisible);
        }
    }
}
