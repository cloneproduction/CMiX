// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // A select message from a peer selects the control with that id. The selected index goes on the wire and into the file.
    public class SelectionTests
    {
        private sealed class Setup
        {
            public IServiceProvider Provider { get; init; } = null!;
            public PrefabManager Manager { get; init; } = null!;
            public RecordingMessageSender Sent { get; init; } = null!;
            public List<IControl> Items { get; init; } = null!;

            public void Receive(Guid controlID, int index = 0) =>
                Manager.Receive(new MessageSelectItem(Manager.ManagerData.ID, controlID, index));
        }

        private static Setup ThreeItems()
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = provider.GetRequiredService<PrefabManager>();
            for (var i = 0; i < 3; i++)
                manager.AddItem(typeof(CheckerBoard));

            var recorder = new RecordingMessageSender();
            provider.GetRequiredService<ControlMessenger>().Register(recorder);
            provider.GetRequiredService<UndoManager>().Clear();
            return new Setup { Provider = provider, Manager = manager, Sent = recorder, Items = manager.ManagerData.Items.ToList() };
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void Receive_ASelectOfAControlOfTheList_SelectsIt(int index)
        {
            var setup = ThreeItems();

            setup.Receive(setup.Items[index].ID);

            Assert.Same(setup.Items[index], setup.Manager.SelectedItem);
            Assert.Equal(index, setup.Manager.ManagerData.SelectedIndex);
        }

        [Fact]
        public void Receive_ASelectWithAnEmptyID_LeavesNothingSelected()
        {
            var setup = ThreeItems();

            setup.Receive(Guid.Empty);

            Assert.Null(setup.Manager.SelectedItem);
            Assert.Equal(-1, setup.Manager.ManagerData.SelectedIndex);
        }

        [Fact]
        public void Receive_ASelectWithAnUnknownID_LeavesNothingSelected()
        {
            var setup = ThreeItems();

            setup.Receive(Guid.NewGuid());

            Assert.Null(setup.Manager.SelectedItem);
        }

        [Fact]
        public void Receive_ASelectOfAControlOfAnotherList_LeavesNothingSelected()
        {
            var setup = ThreeItems();
            var other = setup.Provider.GetRequiredService<CollectionManager>();
            var foreign = setup.Provider.GetRequiredService<ControlFactory>().Create(typeof(CheckerBoard));
            other.InsertItem(foreign, 0);

            setup.Receive(foreign.ID);

            Assert.Null(setup.Manager.SelectedItem);
            Assert.Equal(setup.Items, setup.Manager.ManagerData.Items);
        }

        [Fact]
        public void Receive_ASelect_SendsNothingBackAndLeavesNoUndoStep()
        {
            var setup = ThreeItems();
            var undo = setup.Provider.GetRequiredService<UndoManager>();

            setup.Receive(setup.Items[0].ID);
            undo.Undo();

            Assert.Empty(setup.Sent.Sent);
            Assert.Same(setup.Items[0], setup.Manager.SelectedItem);
        }

        [Fact]
        public void SelectAt_ThenUndo_SelectsThePreviousItemAgain()
        {
            var setup = ThreeItems();
            var undo = setup.Provider.GetRequiredService<UndoManager>();
            Assert.Same(setup.Items[2], setup.Manager.SelectedItem);

            setup.Manager.SelectAt(0);
            undo.Undo();

            Assert.Same(setup.Items[2], setup.Manager.SelectedItem);
        }

        [Fact]
        public void ListEdits_OfAManager_SendTheIndexSelectedAfterTheEdit()
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = provider.GetRequiredService<PrefabManager>();
            var undo = provider.GetRequiredService<UndoManager>();
            var recorder = new RecordingMessageSender();
            provider.GetRequiredService<ControlMessenger>().Register(recorder);

            for (var i = 0; i < 3; i++)
                manager.AddItem(typeof(CheckerBoard));
            Assert.Equal(new[] { 0, 1, 2 }, recorder.Sent.OfType<MessageAddItem>().Select(message => message.SelectedIndex));

            recorder.Sent.Clear();
            manager.RemoveItem(manager.ManagerData.Items[2]);
            Assert.Equal(1, Assert.Single(recorder.Sent.OfType<MessageRemoveItem>()).SelectedIndex);

            recorder.Sent.Clear();
            undo.Undo();
            Assert.Equal(2, Assert.Single(recorder.Sent.OfType<MessageAddItem>()).SelectedIndex);

            recorder.Sent.Clear();
            manager.SelectAt(0);
            Assert.Equal(0, Assert.Single(recorder.Sent.OfType<MessageSelectItem>()).Index);

            recorder.Sent.Clear();
            manager.RemoveItem(manager.ManagerData.Items[0]);
            Assert.Equal(0, Assert.Single(recorder.Sent.OfType<MessageRemoveItem>()).SelectedIndex);
        }

        private static (Layer Layer, ControlFactory Factory) LayerWithEntities(int selectedIndex)
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var layer = (Layer)factory.Create(typeof(Layer));
            for (var i = 0; i < 3; i++)
                layer.ModelEntityManager.AddItem(typeof(Entity));
            if (selectedIndex < 0)
                layer.ModelEntityManager.ClearSelection();
            else
                layer.ModelEntityManager.SelectAt(selectedIndex);
            return (layer, factory);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void Create_OfASavedSelection_KeepsTheSelectedIndex(int selectedIndex)
        {
            var (layer, factory) = LayerWithEntities(selectedIndex);
            var model = (LayerModel)layer.ToModel();
            Assert.Equal(selectedIndex, model.ModelEntityManager.ManagerData.SelectedIndex);

            var loaded = (Layer)factory.Create(ControlCloner.CloneWithNewGuids(model));

            Assert.Equal(selectedIndex, loaded.ModelEntityManager.ManagerData.SelectedIndex);
            Assert.Same(loaded.ModelEntityManager.ManagerData.Items[selectedIndex], loaded.ModelEntityManager.SelectedItem);
        }

        [Fact]
        public void Create_OfASavedEmptySelection_SelectsNothing()
        {
            var (layer, factory) = LayerWithEntities(-1);
            var model = (LayerModel)layer.ToModel();
            Assert.Equal(-1, model.ModelEntityManager.ManagerData.SelectedIndex);

            var loaded = (Layer)factory.Create(ControlCloner.CloneWithNewGuids(model));

            Assert.Equal(-1, loaded.ModelEntityManager.ManagerData.SelectedIndex);
            Assert.Null(loaded.ModelEntityManager.SelectedItem);
        }
    }
}
