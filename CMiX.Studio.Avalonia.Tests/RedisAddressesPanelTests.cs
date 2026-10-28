using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Animations;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Studio.Avalonia.Views.Managers;
using CMiX.UI.Controls.Collection;
using CMiX.UI.Controls.String;
using CMiX.UI.Panels;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The list of Redis addresses in the Settings tab.
    public class RedisAddressesPanelTests
    {
        private static (RedisAddressRegistry Registry, RedisAddressesPanel Panel, IServiceProvider Provider) Show(int addressCount)
        {
            var provider = TestServiceProviderFactory.Create();
            var registry = provider.GetRequiredService<RedisAddressRegistry>();
            var panel = new RedisAddressesPanel { DataContext = registry };
            new Window { Content = panel, Width = 400, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();

            for (var i = 0; i < addressCount; i++)
                registry.AddressManager.AddItem(typeof(RedisAddress));
            Dispatcher.UIThread.RunJobs();
            return (registry, panel, provider);
        }

        [AvaloniaFact]
        public void EachAddress_IsAClickToEditText()
        {
            var (_, panel, _) = Show(2);

            Assert.Equal(2, panel.GetVisualDescendants().OfType<TextBoxEditable>().Count());
        }

        [AvaloniaFact]
        public void TheListHasAddRemoveAndMoveCommands()
        {
            var (_, panel, _) = Show(2);
            var list = panel.GetVisualDescendants().OfType<ListBoxEditable>().Single();

            Assert.NotNull(list.AddCommand);
            Assert.NotNull(list.RemoveCommand);
            Assert.NotNull(list.MoveUpCommand);
            Assert.NotNull(list.MoveDownCommand);
        }

        [AvaloniaFact]
        public void MovingAnAddressDown_ChangesTheOrderOfTheAddresses()
        {
            var (registry, panel, provider) = Show(0);
            foreach (var text in new[] { "cmix:a", "cmix:b" })
            {
                registry.AddressManager.AddItem(typeof(RedisAddress));
                ((RedisAddress)registry.AddressManager.SelectedItem).Value = text;
            }
            var list = panel.GetVisualDescendants().OfType<ListBoxEditable>().Single();

            list.MoveDownCommand!.Execute(registry.AddressManager.ManagerData.Items[0]);

            Assert.Equal(new[] { "cmix:b", "cmix:a" }, registry.Addresses.Select(address => address.Value));
        }

        [AvaloniaFact]
        public void RemovingAnAddress_UnbindsTheValuesBoundToIt()
        {
            var (registry, panel, provider) = Show(1);
            var factory = provider.GetRequiredService<ControlFactory>();
            var blur = (Core.Texturing.Filters.Blur)factory.Create(typeof(Core.Texturing.Filters.Blur));
            blur.ModulatorManager.AddItem(typeof(RedisAddressModulator));
            var modulator = blur.ModulatorManager.ManagerData.Items.OfType<RedisAddressModulator>().Single();
            modulator.SelectedAddress = registry.Addresses.Single();
            blur.Strength.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, modulator.Outputs[0]));
            var list = panel.GetVisualDescendants().OfType<ListBoxEditable>().Single();

            list.RemoveCommand!.Execute(registry.AddressManager.ManagerData.Items[0]);

            Assert.Null(blur.Strength.ModulatorID);
        }

        [AvaloniaFact]
        public void TheSettingsTab_ShowsTheOutputMappingsAndTheRedisAddresses()
        {
            var tab = new Views.SettingsTab();
            new Window { Content = tab, Width = 400, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();

            var headers = tab.GetVisualDescendants().OfType<ExpanderIndented>()
                .Where(expander => expander.IsEffectivelyVisible)
                .Select(expander => expander.Header);

            Assert.Equal(new object[] { "Output Mappings", "Redis Addresses" }, headers);
        }
    }
}
