using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Views.Managers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests.Integration
{
    // The flow in the real window: pick an entity in the swap list with the keyboard, then undo and redo with the keys of the window.
    public class SwapListUndoTests : IDisposable
    {
        private sealed record Setup(Views.MainWindow Window, Layer Layer, Entity Entity, ListPrefabSlot Slot, UndoManager Undo);

        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

        // The entity "Torus" is in a slot of the first layer. The window shows the entity slot of the second layer.
        private Setup Show()
        {
            var (provider, window, viewModel) = windows.ShowMainWindow();

            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.SelectedItem;

            composition.LayerManager.AddItem(typeof(Layer));
            var first = (Layer)composition.LayerManager.SelectedItem;
            first.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (Entity)first.ModelEntityManager.SelectedItem;
            entity.PrefabService.Name.Value = "Torus";

            composition.LayerManager.AddItem(typeof(Layer));
            var second = (Layer)composition.LayerManager.SelectedItem;

            var tabControl = TestServiceProviderFactory.MainTabControl(window);
            tabControl.SelectedItem = tabControl.Items.OfType<TabItem>().Single(tab => tab.Content is Views.TabLayers);
            TestServiceProviderFactory.Pump();

            var slot = window.GetVisualDescendants().OfType<ListPrefabSlot>()
                .First(manager => ReferenceEquals(manager.ItemsSource, viewModel.ControlRepository.ModelEntities)
                                  && ReferenceEquals(manager.DataContext, second.ModelEntityManager));

            return new Setup(window, second, entity, slot, provider.GetRequiredService<UndoManager>());
        }

        private static void PickWithSearch(Setup setup, string text)
        {
            setup.Slot.FindControl<ToggleButton>("swapToggle")!.IsChecked = true;
            TestServiceProviderFactory.Pump();

            setup.Window.KeyTextInput(text);
            Dispatcher.UIThread.RunJobs();
            setup.Window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);
            TestServiceProviderFactory.Pump();
        }

        private static void Press(Setup setup, PhysicalKey key)
        {
            setup.Window.KeyPressQwerty(key, RawInputModifiers.Control);
            TestServiceProviderFactory.Pump();
        }

        [AvaloniaFact]
        public void Undo_AfterSearchEnterAndRemove_BringsTheEntityBack()
        {
            var setup = Show();
            var manager = setup.Layer.ModelEntityManager;
            PickWithSearch(setup, "tor");

            manager.RemoveItemCommand.Execute(manager.SelectedItem);
            TestServiceProviderFactory.Pump();
            Assert.DoesNotContain(setup.Entity, manager.ManagerData.Items);

            Press(setup, PhysicalKey.Z);

            Assert.Contains(setup.Entity, manager.ManagerData.Items);
        }

        [AvaloniaFact]
        public void Redo_AfterSearchEnterAndUndo_PutsTheEntityBack()
        {
            var setup = Show();
            var items = setup.Layer.ModelEntityManager.ManagerData.Items;
            PickWithSearch(setup, "tor");
            Assert.Contains(setup.Entity, items);

            Press(setup, PhysicalKey.Z);
            Assert.DoesNotContain(setup.Entity, items);

            Press(setup, PhysicalKey.Y);

            Assert.Contains(setup.Entity, items);
        }
    }
}
