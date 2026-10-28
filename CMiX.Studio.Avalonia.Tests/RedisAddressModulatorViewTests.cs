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
using CMiX.UI.Controls.Enum;
using CMiX.UI.Controls.Number;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ViewRedisAddressModulator = CMiX.Studio.Avalonia.Views.RedisAddressModulator;

namespace CMiX.Studio.Avalonia.Tests
{
    // The address combo box and the gain of a RedisAddressModulator in the window.
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

        private RedisAddress AddAddress(string text)
        {
            Registry.AddAddress();
            var address = (RedisAddress)Registry.AddressManager.SelectedItem;
            address.Value = text;
            return address;
        }

        private static ViewRedisAddressModulator Show(RedisAddressModulator modulator)
        {
            var view = new ViewRedisAddressModulator { DataContext = modulator };
            new Window { Content = view, Width = 400, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();
            return view;
        }

        private static ComboBoxCaptioned AddressBox(Control view) =>
            view.GetVisualDescendants().OfType<ComboBoxCaptioned>().Single(combo => combo.Caption == "Address");

        private static DragValue GainValue(Control view) =>
            view.GetVisualDescendants().OfType<DragValue>().Single(drag => drag.Caption == "Gain");

        [AvaloniaFact]
        public void Show_TheAddressBox_ListsTheAddressesOfTheProject()
        {
            var modulator = NewModulator();
            var first = AddAddress("cmix:kick");
            var second = AddAddress("cmix:snare");

            var view = Show(modulator);

            Assert.Equal(new object[] { first, second }, AddressBox(view).ItemsSource!.Cast<object>());
        }

        [AvaloniaFact]
        public void SelectedItem_PickingAnAddress_SetsTheSelectedAddressID()
        {
            var modulator = NewModulator();
            AddAddress("cmix:kick");
            var second = AddAddress("cmix:snare");
            var view = Show(modulator);

            AddressBox(view).SelectedItem = second;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(second.ID, modulator.SelectedAddressID.Value);
        }

        [AvaloniaFact]
        public void Show_AModulatorWithASelection_ShowsItAsTheSelectedItem()
        {
            var modulator = NewModulator();
            AddAddress("cmix:kick");
            var second = AddAddress("cmix:snare");
            modulator.SelectedAddress = second;

            var view = Show(modulator);

            Assert.Same(second, AddressBox(view).SelectedItem);
        }

        [AvaloniaFact]
        public void Value_TheGainDragValue_ChangesTheGain()
        {
            var modulator = NewModulator();
            var view = Show(modulator);

            GainValue(view).Value = 3.0;

            Assert.Equal(3.0f, modulator.Gain.Value);
        }

        [AvaloniaFact]
        public void Show_TheView_LeavesNothingOnTheUndoStack()
        {
            AddAddress("cmix:kick");
            var modulator = NewModulator();
            var undo = _provider.GetRequiredService<UndoManager>();
            undo.Clear();

            Show(modulator);

            Assert.False(undo.CanUndo);
        }
    }
}
