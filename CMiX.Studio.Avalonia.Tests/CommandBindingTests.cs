using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Filters;
using CMiX.Studio.Avalonia.Services;
using CMiX.UI.Controls.Collection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ViewBeatStepModulator = CMiX.Studio.Avalonia.Views.BeatStepModulator;

namespace CMiX.Studio.Avalonia.Tests
{
    // A binding of a command to a wrong name gives no build error and no exception. The control
    // just has no command. These tests show each view and check that its controls hold the commands of the manager.
    public class CommandBindingTests
    {
        private static PrefabManager LayerManagerWithItems(int count)
        {
            var provider = TestServiceProviderFactory.Create();
            var compositions = provider.GetRequiredService<PrefabManager>();
            compositions.AddItem(typeof(Composition));
            var layers = ((Composition)compositions.SelectedItem).LayerManager;
            for (var i = 0; i < count; i++)
                layers.AddItem(typeof(Layer));
            return layers;
        }

        private static void Show(Control view)
        {
            new Window { Content = view, Width = 400, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();
        }

        private static List<ICommand?> CommandsOf(Control view) =>
            view.GetVisualDescendants().OfType<ICommandSource>().Select(source => source.Command).ToList();

        [AvaloniaFact]
        public void TheOutliner_MinusButton_HoldsTheCommandThatClearsTheSelection()
        {
            var manager = LayerManagerWithItems(2);
            var view = new Views.Controls.Outliner { DataContext = manager, ItemsSource = manager.ManagerData.Items };
            Show(view);

            var button = view.GetVisualDescendants().OfType<Button>().Single(b => b.Name == "deleteItemButton");

            Assert.Same(manager.ClearSelectionCommand, button.Command);
        }

        [AvaloniaFact]
        public void TheComposableToolbar_HoldsTheAddMoveAndDeleteCommands()
        {
            var manager = LayerManagerWithItems(2);
            var view = new Views.ComposableManagerToolbar { DataContext = manager };
            Show(view);
            var service = (ManagerReorderService)manager.ManagerReorderService;

            var commands = CommandsOf(view);

            Assert.Contains(manager.AddItemCommand, commands);
            Assert.Contains(service.MoveItemUpCommand, commands);
            Assert.Contains(service.MoveItemDownCommand, commands);
            Assert.Contains(manager.RemoveItemCommand, commands);
        }

        [AvaloniaFact]
        public void ThePrefabSlotManager_HoldsTheMoveAndDeleteCommands()
        {
            var manager = TestServiceProviderFactory.Create().GetRequiredService<PrefabManager>();
            var view = new Views.Managers.PrefabSlotManager { DataContext = manager };
            Show(view);
            var service = (ManagerReorderService)manager.ManagerReorderService;

            var commands = CommandsOf(view);

            Assert.Contains(manager.RemoveItemCommand, commands);
            Assert.Contains(service.MoveItemUpCommand, commands);
            Assert.Contains(service.MoveItemDownCommand, commands);
        }

        [AvaloniaFact]
        public void EachRowOfTheModifierManager_ClosesWithTheDeleteCommandOfTheManager()
        {
            var blur = (Blur)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(Blur));
            blur.ModulatorManager.AddItem(typeof(FFTModulator));
            blur.ModulatorManager.AddItem(typeof(FFTModulator));
            var view = new Views.ModifierManager { DataContext = blur.ModulatorManager, Caption = "Add Modulator" };
            Show(view);

            var rows = view.GetVisualDescendants().OfType<PanelItem>().ToList();

            Assert.Equal(2, rows.Count);
            Assert.All(rows, row => Assert.Same(blur.ModulatorManager.RemoveItemCommand, row.CloseCommand));
        }

        [AvaloniaFact]
        public void TheBeatStepList_HoldsTheRemoveAndMoveCommands()
        {
            var modulator = (BeatStepModulator)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(BeatStepModulator));
            var view = new ViewBeatStepModulator { DataContext = modulator };
            Show(view);
            var service = (ManagerReorderService)modulator.StepManager.ManagerReorderService;

            var list = view.GetVisualDescendants().OfType<ListBoxEditable>().Single();

            Assert.Same(modulator.StepManager.RemoveItemCommand, list.RemoveCommand);
            Assert.Same(service.MoveItemUpCommand, list.MoveUpCommand);
            Assert.Same(service.MoveItemDownCommand, list.MoveDownCommand);
        }

        [AvaloniaFact]
        public void TheRepositoryList_HoldsTheCommandThatDeletesEverywhere()
        {
            var manager = TestServiceProviderFactory.Create().GetRequiredService<PrefabManager>();
            var view = new Views.Managers.RepositoryManager { DataContext = manager, ItemsSource = manager.ManagerData.Items };
            Show(view);

            var list = view.GetVisualDescendants().OfType<ListBoxRepository>().Single();

            Assert.Same(manager.RemoveEverywhereCommand, list.DeleteCommand);
        }
    }
}
