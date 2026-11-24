// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Managers;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // The list box of Avalonia can lose its selection when the selected item moves (Avalonia issue 16279).
    // Whatever repairs it, the selection of the model and the selection shown must stay on the moved item.
    // A list box of Avalonia clears its selection on a click with the Ctrl key. The list and the model must not disagree afterwards.
    public class ListComposableTests : IDisposable
    {
        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

        private (PrefabManager Layers, ListBox List, List<IControl> Items) Show()
        {
            var provider = TestServiceProviderFactory.Create();
            var compositions = provider.GetRequiredService<PrefabManager>();
            compositions.AddItem(typeof(Composition));
            var layers = ((Composition)compositions.SelectedItem).LayerManager;
            for (var i = 0; i < 3; i++)
                layers.AddItem(typeof(Layer));

            var view = new ListComposable { ItemType = typeof(Layer), DataContext = layers };
            windows.Add(new Window { Content = view, Width = 400, Height = 300 }).Show();
            Dispatcher.UIThread.RunJobs();

            return (layers, view.GetVisualDescendants().OfType<ListBox>().First(), layers.ManagerData.Items.ToList());
        }

        [AvaloniaFact]
        public void Move_TheSelectedItem_KeepsItSelectedInTheModelAndInTheList()
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
        public void Move_AnotherItem_KeepsTheSelectionOnTheSameItem()
        {
            var (layers, list, items) = Show();

            layers.MoveItemTo(items[0], 2);
            Dispatcher.UIThread.RunJobs();

            Assert.Same(items[2], layers.SelectedItem);
            Assert.Equal(1, layers.ManagerData.SelectedIndex);
            Assert.Same(items[2], list.SelectedItem);
        }

        [AvaloniaFact]
        public void Receive_AMoveFromAPeer_KeepsTheSelectedItemSelectedInTheModelAndInTheList()
        {
            var (layers, list, items) = Show();

            layers.Receive(new MessageMoveItem(layers.ManagerData.ID, 2, 0));
            Dispatcher.UIThread.RunJobs();

            Assert.Same(items[2], layers.SelectedItem);
            Assert.Equal(0, layers.ManagerData.SelectedIndex);
            Assert.Same(items[2], list.SelectedItem);
            Assert.Equal(0, list.SelectedIndex);
        }

        [AvaloniaFact]
        public void CtrlClick_TheSelectedRow_LeavesTheListAndTheModelInAgreement()
        {
            var provider = TestServiceProviderFactory.Create();
            var compositions = provider.GetRequiredService<PrefabManager>();
            compositions.AddItem(typeof(Composition));
            var layers = ((Composition)compositions.SelectedItem).LayerManager;
            for (var i = 0; i < 3; i++)
                layers.AddItem(typeof(Layer));

            var view = new ListComposable { ItemType = typeof(Layer), DataContext = layers };
            var window = windows.Add(new Window { Content = view, Width = 400, Height = 300 });
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var list = view.GetVisualDescendants().OfType<ListBox>().First();
            var selectedRow = view.GetVisualDescendants().OfType<RowComposable>().ElementAt(2);
            var point = selectedRow.TranslatePoint(new Point(selectedRow.Bounds.Width / 2, selectedRow.Bounds.Height / 2), window)!.Value;

            window.MouseMove(point);
            Dispatcher.UIThread.RunJobs();
            window.MouseDown(point, MouseButton.Left, RawInputModifiers.Control);
            window.MouseUp(point, MouseButton.Left, RawInputModifiers.Control);
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(layers.ManagerData.SelectedIndex, list.SelectedIndex);
        }
    }
}
