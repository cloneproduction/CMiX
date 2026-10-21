using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Managers;
using CMiX.Studio.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // The list box of Avalonia can lose its selection when the selected item moves (Avalonia issue 16279).
    // Whatever repairs it, the selection of the model and the selection shown must stay on the moved item.
    public class MoveSelectionTests
    {
        private static (PrefabManager Layers, ListBox List, List<IControl> Items) Show()
        {
            var provider = TestServiceProviderFactory.Create();
            var compositions = provider.GetRequiredService<PrefabManager>();
            compositions.AddItem(typeof(Composition));
            var layers = ((Composition)compositions.SelectedItem).LayerManager;
            for (var i = 0; i < 3; i++)
                layers.AddItem(typeof(Layer));

            var view = new ComposableManager { ItemType = typeof(Layer), DataContext = layers };
            new Window { Content = view, Width = 400, Height = 300 }.Show();
            Dispatcher.UIThread.RunJobs();

            return (layers, view.GetVisualDescendants().OfType<ListBox>().First(), layers.ManagerData.Items.ToList());
        }

        [AvaloniaFact]
        public void MovingTheSelectedItem_KeepsItSelectedInTheModelAndInTheList()
        {
            var (layers, list, items) = Show();
            Assert.Same(items[2], layers.SelectedItem);

            layers.MoveItemTo(items[2], 0);
            Dispatcher.UIThread.RunJobs();

            Assert.Same(items[2], layers.SelectedItem);
            Assert.Equal(0, layers.ManagerData.SelectedIndex);
            Assert.Same(items[2], list.SelectedItem);
            Assert.Equal(0, list.SelectedIndex);
        }

        [AvaloniaFact]
        public void MovingAnotherItem_KeepsTheSelectionOnTheSameItem()
        {
            var (layers, list, items) = Show();

            layers.MoveItemTo(items[0], 2);
            Dispatcher.UIThread.RunJobs();

            Assert.Same(items[2], layers.SelectedItem);
            Assert.Equal(1, layers.ManagerData.SelectedIndex);
            Assert.Same(items[2], list.SelectedItem);
        }

        [AvaloniaFact]
        public void AMoveThatArrivesFromAPeer_KeepsTheSelectedItemSelectedInTheModelAndInTheList()
        {
            var (layers, list, items) = Show();

            layers.Receive(new MessageMoveItem(layers.ManagerData.ID, 2, 0));
            Dispatcher.UIThread.RunJobs();

            Assert.Same(items[2], layers.SelectedItem);
            Assert.Equal(0, layers.ManagerData.SelectedIndex);
            Assert.Same(items[2], list.SelectedItem);
            Assert.Equal(0, list.SelectedIndex);
        }
    }
}
