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
    // A remove moves the selection only when the removed item was the selected one. The undo command owns the removed item.
    public class RemoveItemTests
    {
        private sealed class Setup
        {
            public PrefabManager Manager { get; init; } = null!;
            public UndoManager Undo { get; init; } = null!;
            public RecordingMessageSender Sent { get; init; } = null!;
            public List<IControl> Items { get; init; } = null!;
        }

        private static Setup ThreeItemsWithTheSelectionOn(int selectedIndex)
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = provider.GetRequiredService<PrefabManager>();
            for (var i = 0; i < 3; i++)
                manager.AddItem(typeof(CheckerBoard));
            manager.SelectAt(selectedIndex);

            var recorder = new RecordingMessageSender();
            provider.GetRequiredService<ControlMessenger>().Register(recorder);
            var undo = provider.GetRequiredService<UndoManager>();
            undo.Clear();
            return new Setup { Manager = manager, Undo = undo, Sent = recorder, Items = manager.ManagerData.Items.ToList() };
        }

        private static (Composition composition, Layer layer, Entity entity) CompositionWithALayerAndAnEntity(PrefabManager compositionManager)
        {
            compositionManager.AddItem(typeof(Composition));
            var composition = (Composition)compositionManager.SelectedItem;
            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;
            layer.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (Entity)layer.ModelEntityManager.SelectedItem;
            return (composition, layer, entity);
        }

        [Theory]
        [InlineData(2, 0, 1)]
        [InlineData(2, 1, 1)]
        [InlineData(0, 1, 0)]
        [InlineData(0, 2, 0)]
        [InlineData(1, 0, 0)]
        [InlineData(1, 2, 1)]
        public void RemoveItem_OfAnItemThatIsNotSelected_KeepsTheSelectionOnTheSameItem(int selected, int removed, int expectedIndex)
        {
            var setup = ThreeItemsWithTheSelectionOn(selected);
            var selectedItem = setup.Items[selected];

            setup.Manager.RemoveItem(setup.Items[removed]);

            Assert.Same(selectedItem, setup.Manager.SelectedItem);
            Assert.Equal(expectedIndex, setup.Manager.ManagerData.SelectedIndex);
        }

        [Fact]
        public void RemoveItem_OfTheSelectedItem_SelectsTheItemBeforeIt()
        {
            var setup = ThreeItemsWithTheSelectionOn(2);

            setup.Manager.RemoveItem(setup.Items[2]);

            Assert.Same(setup.Items[1], setup.Manager.SelectedItem);
        }

        [Fact]
        public void RemoveItem_OfAnItemThatIsNotSelected_SendsTheIndexOfTheKeptSelection()
        {
            var setup = ThreeItemsWithTheSelectionOn(2);

            setup.Manager.RemoveItem(setup.Items[0]);

            Assert.Equal(1, Assert.Single(setup.Sent.Sent.OfType<MessageRemoveItem>()).SelectedIndex);
            Assert.Empty(setup.Sent.Sent.OfType<MessageSelectItem>());
        }

        [Fact]
        public void Undo_OfARemoveOfAnItemThatWasNotSelected_SelectsTheSameItemAsBefore()
        {
            var setup = ThreeItemsWithTheSelectionOn(2);
            setup.Manager.RemoveItem(setup.Items[0]);
            setup.Sent.Sent.Clear();

            setup.Undo.Undo();

            Assert.Equal(setup.Items, setup.Manager.ManagerData.Items);
            Assert.Same(setup.Items[2], setup.Manager.SelectedItem);
            Assert.Equal(2, setup.Manager.ManagerData.SelectedIndex);
            var selected = Assert.Single(setup.Sent.Sent.OfType<MessageSelectItem>());
            Assert.Equal(setup.Items[2].ID, selected.ControlID);
            Assert.Equal(2, selected.Index);
        }

        [Fact]
        public void Undo_OfARemoveOfTheSelectedItem_SelectsTheRestoredItem()
        {
            var setup = ThreeItemsWithTheSelectionOn(1);
            setup.Manager.RemoveItem(setup.Items[1]);

            setup.Undo.Undo();

            Assert.Same(setup.Items[1], setup.Manager.SelectedItem);
            Assert.Equal(1, setup.Manager.ManagerData.SelectedIndex);
        }

        [Fact]
        public void Redo_OfARemoveOfAnItemThatWasNotSelected_KeepsTheSelectionOnTheSameItem()
        {
            var setup = ThreeItemsWithTheSelectionOn(2);
            setup.Manager.RemoveItem(setup.Items[0]);
            setup.Undo.Undo();

            setup.Undo.Redo();

            Assert.Same(setup.Items[2], setup.Manager.SelectedItem);
            Assert.Equal(1, setup.Manager.ManagerData.SelectedIndex);
        }

        [Fact]
        public void Receive_ARemoveFromAPeer_KeepsTheSelectionOnTheSameItem()
        {
            var setup = ThreeItemsWithTheSelectionOn(2);

            setup.Manager.Receive(new MessageRemoveItem(setup.Manager.ManagerData.ID, setup.Items[0].ID, 1));

            Assert.Same(setup.Items[2], setup.Manager.SelectedItem);
            Assert.Equal(1, setup.Manager.ManagerData.SelectedIndex);
        }

        // The context menu removes any row without a confirmation, the last composition included, so undo must work.
        [Fact]
        public void RemoveItem_OfTheOnlyComposition_LeavesNoneAndUndoBringsItBack()
        {
            var provider = TestServiceProviderFactory.Create();
            var undo = provider.GetRequiredService<UndoManager>();
            var compositions = provider.GetRequiredService<PrefabManager>();
            compositions.AddItem(typeof(Composition));
            var composition = (Composition)compositions.SelectedItem;
            composition.LayerManager.AddItem(typeof(Layer));

            compositions.RemoveItem(composition);

            Assert.Empty(compositions.ManagerData.Items);
            Assert.Null(compositions.SelectedItem);

            undo.Undo();

            Assert.Same(composition, Assert.Single(compositions.ManagerData.Items));
            Assert.Single(composition.LayerManager.ManagerData.Items);
        }

        [Fact]
        public void Undo_OfARemovedLayer_RestoresTheLayerWithItsContents()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var (composition, layer, entity) = CompositionWithALayerAndAnEntity(provider.GetRequiredService<PrefabManager>());

            composition.LayerManager.RemoveItem(layer);

            Assert.DoesNotContain(layer, composition.LayerManager.ManagerData.Items);

            undoManager.Undo();

            Assert.Contains(layer, composition.LayerManager.ManagerData.Items);
            Assert.Contains(entity, layer.ModelEntityManager.ManagerData.Items);
            Assert.Same(entity, repository.GetControl(entity.ID));
        }

        [Fact]
        public void Clear_OfTheUndoEntryOfARemove_TearsTheLayerDown()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var (composition, layer, entity) = CompositionWithALayerAndAnEntity(provider.GetRequiredService<PrefabManager>());

            composition.LayerManager.RemoveItem(layer);
            undoManager.Clear();

            Assert.Empty(layer.ModelEntityManager.ManagerData.Items);
            Assert.Null(repository.GetControl(entity.ID));
            Assert.Null(repository.GetControl(layer.ID));
        }

        [Fact]
        public void RemoveEverywhereCommand_OfAControlInTwoLists_RemovesItFromBothAndTheRepository()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var managerA = provider.GetRequiredService<PrefabManager>();
            var managerB = provider.GetRequiredService<PrefabManager>();

            managerA.AddItem(typeof(Entity));
            var entity = (Entity)managerA.SelectedItem;
            managerB.AddExistingItem(entity);

            Assert.Contains(entity, managerA.ManagerData.Items);
            Assert.Contains(entity, managerB.ManagerData.Items);
            undoManager.Clear();

            managerA.RemoveEverywhereCommand.Execute(entity);

            Assert.DoesNotContain(entity, managerA.ManagerData.Items);
            Assert.DoesNotContain(entity, managerB.ManagerData.Items);
            Assert.Null(repository.GetControl(entity.ID));
        }

        [Fact]
        public void Undo_OfARemoveEverywhere_RestoresBothReferencesAsOneStep()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var managerA = provider.GetRequiredService<PrefabManager>();
            var managerB = provider.GetRequiredService<PrefabManager>();

            managerA.AddItem(typeof(Entity));
            var entity = (Entity)managerA.SelectedItem;
            managerB.AddExistingItem(entity);
            undoManager.Clear();

            managerA.RemoveEverywhereCommand.Execute(entity);
            Assert.True(undoManager.CanUndo);

            undoManager.Undo();

            Assert.False(undoManager.CanUndo);
            Assert.Contains(entity, managerA.ManagerData.Items);
            Assert.Contains(entity, managerB.ManagerData.Items);
            Assert.Same(entity, repository.GetControl(entity.ID));
        }

        [Fact]
        public void AddItemUndoCommand_UndoThenRedoInTheMiddle_KeepsTheIndex()
        {
            var provider = TestServiceProviderFactory.Create();
            var compositions = provider.GetRequiredService<PrefabManager>();
            compositions.AddItem(typeof(Composition));
            var layers = ((Composition)compositions.SelectedItem).LayerManager;
            for (var i = 0; i < 3; i++)
                layers.AddItem(typeof(Layer));
            var items = layers.ManagerData.Items.ToList();

            var added = provider.GetRequiredService<ControlFactory>().Create(typeof(Layer));
            var sync = new CollectionSync(layers.Collection, layers.ControlMessenger, layers.MessageFactory);
            var command = new AddItemUndoCommand(sync, added, 1);

            command.Execute();
            Assert.Equal(new[] { items[0], added, items[1], items[2] }, layers.ManagerData.Items);

            command.Undo();
            Assert.Equal(items, layers.ManagerData.Items);

            command.Execute();
            Assert.Equal(new[] { items[0], added, items[1], items[2] }, layers.ManagerData.Items);
            Assert.Equal(1, layers.ManagerData.SelectedIndex);
        }
    }
}
