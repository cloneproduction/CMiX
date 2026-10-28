using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Animations;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Undo;
using CMiX.UI.Controls.Enum;
using CMiX.UI.Controls.Number;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ViewRedisBoolModulator = CMiX.Studio.Avalonia.Views.RedisBoolModulator;

namespace CMiX.Studio.Avalonia.Tests
{
    // The address combo box of a RedisBoolModulator in the window. The view has no gain.
    public class RedisBoolModulatorViewTests
    {
        private readonly IServiceProvider _provider = TestServiceProviderFactory.Create();

        private RedisAddressRegistry Registry => _provider.GetRequiredService<RedisAddressRegistry>();

        private RedisBoolModulator NewModulator()
        {
            var player = (SequencePlayer)_provider.GetRequiredService<ControlFactory>().Create(typeof(SequencePlayer));
            player.ModulatorManager.AddItem(typeof(RedisBoolModulator));
            return (RedisBoolModulator)player.ModulatorManager.SelectedItem;
        }

        private RedisAddress AddAddress(string text)
        {
            Registry.AddAddress();
            var address = (RedisAddress)Registry.AddressManager.SelectedItem;
            address.Value = text;
            return address;
        }

        private static ViewRedisBoolModulator Show(RedisBoolModulator modulator)
        {
            var view = new ViewRedisBoolModulator { DataContext = modulator };
            new Window { Content = view, Width = 400, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();
            return view;
        }

        private static ComboBoxCaptioned AddressBox(Control view) =>
            view.GetVisualDescendants().OfType<ComboBoxCaptioned>().Single(combo => combo.Caption == "Address");

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
        public void ResetCommand_OfTheAddressBox_ClearsTheSelection()
        {
            var modulator = NewModulator();
            AddAddress("cmix:kick");
            modulator.SelectedAddress = AddAddress("cmix:snare");
            var view = Show(modulator);
            var box = AddressBox(view);

            box.ResetCommand!.Execute(null);
            Dispatcher.UIThread.RunJobs();

            Assert.Null(modulator.SelectedAddressID.Value);
            Assert.Null(box.SelectedItem);
        }

        [AvaloniaFact]
        public void Show_TheView_HasNoDragValue()
        {
            var view = Show(NewModulator());

            Assert.Empty(view.GetVisualDescendants().OfType<DragValue>());
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
