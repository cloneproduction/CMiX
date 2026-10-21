using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Animations;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Views.Managers;
using CMiX.UI.Controls.Number;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ViewRedisAddressModulator = CMiX.Studio.Avalonia.Views.RedisAddressModulator;

namespace CMiX.Studio.Avalonia.Tests
{
    // The multipliers of a RedisAddressModulator in the window.
    public class RedisAddressModulatorViewTests
    {
        private readonly IServiceProvider _provider = TestServiceProviderFactory.Create();

        private RedisAddressRegistry Registry => _provider.GetRequiredService<RedisAddressRegistry>();

        private RedisAddressModulator NewModulator()
        {
            var blur = (Blur)_provider.GetRequiredService<ControlFactory>().Create(typeof(Blur));
            blur.ModulatorManager.AddItem(typeof(RedisAddressModulator));
            return (RedisAddressModulator)blur.ModulatorManager.SelectedItem;
        }

        private static ViewRedisAddressModulator Show(RedisAddressModulator modulator)
        {
            var view = new ViewRedisAddressModulator { DataContext = modulator };
            new Window { Content = view, Width = 400, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();
            return view;
        }

        private static List<DragValue> DragValues(Control view) =>
            view.GetVisualDescendants().OfType<DragValue>().ToList();

        [AvaloniaFact]
        public void EachAddress_HasADragValueCaptionedWithTheAddress()
        {
            var modulator = NewModulator();
            Registry.AddAddress();
            ((RedisAddress)Registry.AddressManager.SelectedItem).Value = "cmix:kick";
            Registry.AddAddress();
            ((RedisAddress)Registry.AddressManager.SelectedItem).Value = "cmix:snare";

            var view = Show(modulator);

            Assert.Equal(new object?[] { "cmix:kick", "cmix:snare" }, DragValues(view).Select(drag => drag.Caption));
        }

        [AvaloniaFact]
        public void TheCaption_FollowsAnEditOfTheAddress()
        {
            var modulator = NewModulator();
            Registry.AddAddress();
            var view = Show(modulator);

            ((RedisAddress)Registry.AddressManager.SelectedItem).Value = "cmix:changed";
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("cmix:changed", DragValues(view).Single().Caption);
        }

        [AvaloniaFact]
        public void OpeningTheView_GivesTheGainsOfTheAddressesWithoutAnUndoStep()
        {
            Registry.AddressManager.AddItem(typeof(RedisAddress));
            var modulator = NewModulator();
            var undo = _provider.GetRequiredService<UndoManager>();
            undo.Clear();

            Show(modulator);

            Assert.Single(modulator.Gains);
            Assert.False(undo.CanUndo);
        }

        [AvaloniaFact]
        public void TheDragValue_ChangesTheGain()
        {
            var modulator = NewModulator();
            Registry.AddAddress();
            var view = Show(modulator);

            DragValues(view).Single().Value = 3.0;

            Assert.Equal(3.0f, modulator.Gains.Single().Value);
        }

        [AvaloniaFact]
        public void TheAddButtonOfTheSettingsTab_GivesTheGainsToTheModulators()
        {
            var modulator = NewModulator();
            var panel = new RedisAddressesPanel { DataContext = Registry };
            new Window { Content = panel, Width = 400, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();

            Registry.AddAddressCommand.Execute(null);

            Assert.Single(Registry.Addresses);
            Assert.Single(modulator.Gains);
        }
    }
}
