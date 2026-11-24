// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // MoveItemTo moves in one list, MoveItemToManager moves a layer to another composition. An item that another layer shares blocks the move.
    public class MoveItemTests
    {
        private static (IServiceProvider provider, PrefabManager layers, List<Layer> items) ThreeLayers()
        {
            var provider = TestServiceProviderFactory.Create();
            var compositions = provider.GetRequiredService<PrefabManager>();
            compositions.AddItem(typeof(Composition));
            var layers = ((Composition)compositions.SelectedItem).LayerManager;
            for (var i = 0; i < 3; i++)
                layers.AddItem(typeof(Layer));
            return (provider, layers, layers.ManagerData.Items.Cast<Layer>().ToList());
        }

        private static (Layer First, Layer Second, Layer Third) ThreeLayersAsTuple()
        {
            var items = ThreeLayers().items;
            return (items[0], items[1], items[2]);
        }

        private sealed class Scene
        {
            public Composition A { get; init; } = null!;
            public Composition B { get; init; } = null!;
            public UndoManager Undo { get; init; } = null!;
            public IServiceProvider Provider { get; init; } = null!;

            public Layer LayerOfA(int index) => (Layer)A.LayerManager.ManagerData.Items[index];
            public Layer LayerOfB(int index) => (Layer)B.LayerManager.ManagerData.Items[index];
        }

        // Composition A holds two layers, composition B holds one.
        private static Scene CreateScene()
        {
            var provider = TestServiceProviderFactory.Create();
            var compositions = provider.GetRequiredService<PrefabManager>();
            compositions.AddItem(typeof(Composition));
            compositions.AddItem(typeof(Composition));
            var a = (Composition)compositions.ManagerData.Items[0];
            var b = (Composition)compositions.ManagerData.Items[1];

            a.LayerManager.AddItem(typeof(Layer));
            a.LayerManager.AddItem(typeof(Layer));
            b.LayerManager.AddItem(typeof(Layer));

            var undo = provider.GetRequiredService<UndoManager>();
            undo.Clear();
            return new Scene { A = a, B = b, Undo = undo, Provider = provider };
        }

        [Fact]
        public void MoveItemTo_AnotherIndex_MovesTheItemAndUndoPutsItBack()
        {
            var (provider, layers, items) = ThreeLayers();
            var undo = provider.GetRequiredService<UndoManager>();

            layers.MoveItemTo(items[0], 2);
            Assert.Equal(new[] { items[1], items[2], items[0] }, layers.ManagerData.Items);

            undo.Undo();
            Assert.Equal(items, layers.ManagerData.Items);

            undo.Redo();
            Assert.Equal(new[] { items[1], items[2], items[0] }, layers.ManagerData.Items);
        }

        [Fact]
        public void MoveItemTo_AnotherIndex_SendsOneMoveMessage()
        {
            var (provider, layers, items) = ThreeLayers();
            var sender = new RecordingMessageSender();
            provider.GetRequiredService<ControlMessenger>().Register(sender);

            layers.MoveItemTo(items[2], 0);

            var message = Assert.Single(sender.Sent.OfType<MessageMoveItem>());
            Assert.Equal(2, message.OldIndex);
            Assert.Equal(0, message.NewIndex);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(3)]
        public void MoveItemTo_WithTheSameOrABadIndex_DoesNothing(int newIndex)
        {
            var (provider, layers, items) = ThreeLayers();
            var sender = new RecordingMessageSender();
            provider.GetRequiredService<ControlMessenger>().Register(sender);

            layers.MoveItemTo(items[0], newIndex);

            Assert.Equal(items, layers.ManagerData.Items);
            Assert.Empty(sender.Sent.OfType<MessageMoveItem>());
        }

        [Fact]
        public void MoveItemTo_ForAnItemOfAnotherList_DoesNothing()
        {
            var (_, layers, items) = ThreeLayers();
            var other = ThreeLayers().items[0];

            layers.MoveItemTo(other, 1);

            Assert.Equal(items, layers.ManagerData.Items);
        }

        [Fact]
        public void MoveItemToManager_ALayer_LeavesTheSourceAndEntersTheTargetAtTheIndex()
        {
            var scene = CreateScene();
            var moved = scene.LayerOfA(0);
            var stays = scene.LayerOfA(1);
            var other = scene.LayerOfB(0);

            var result = scene.B.LayerManager.MoveItemToManager(moved, scene.A.LayerManager, 0);

            Assert.True(result.Moved);
            Assert.Empty(result.Blocking);
            Assert.Equal(new IControl[] { stays }, scene.A.LayerManager.ManagerData.Items);
            Assert.Equal(new IControl[] { moved, other }, scene.B.LayerManager.ManagerData.Items);
        }

        [Fact]
        public void MoveItemToManager_AnIndexPastTheEnd_AppendsTheLayer()
        {
            var scene = CreateScene();
            var moved = scene.LayerOfA(0);
            var other = scene.LayerOfB(0);

            scene.B.LayerManager.MoveItemToManager(moved, scene.A.LayerManager, 99);

            Assert.Equal(new IControl[] { other, moved }, scene.B.LayerManager.ManagerData.Items);
        }

        [Fact]
        public void MoveItemToManager_ALayer_GivesTheLayerAndItsItemsTheCompositionIdOfTheTarget()
        {
            var scene = CreateScene();
            var moved = scene.LayerOfA(0);
            moved.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (Entity)moved.ModelEntityManager.ManagerData.Items[0];
            Assert.Equal(scene.A.ID, entity.CompositionID);

            scene.B.LayerManager.MoveItemToManager(moved, scene.A.LayerManager, 0);

            Assert.Equal(scene.B.ID, moved.CompositionID);
            Assert.Equal(scene.B.ID, entity.CompositionID);
        }

        [Fact]
        public void MoveItemToManager_ALayer_KeepsItsItems()
        {
            var scene = CreateScene();
            var repository = scene.Provider.GetRequiredService<ControlRepository>();
            var moved = scene.LayerOfA(0);
            moved.ModelEntityManager.AddItem(typeof(Entity));
            var entity = moved.ModelEntityManager.ManagerData.Items[0];

            scene.B.LayerManager.MoveItemToManager(moved, scene.A.LayerManager, 0);

            Assert.Same(moved, repository.GetControl(moved.ID));
            Assert.Same(entity, repository.GetControl(entity.ID));
            Assert.Same(entity, Assert.Single(moved.ModelEntityManager.ManagerData.Items));
        }

        [Fact]
        public void MoveItemToManager_ALayer_IsOneUndoStep()
        {
            var scene = CreateScene();
            var moved = scene.LayerOfA(0);
            var stays = scene.LayerOfA(1);
            var other = scene.LayerOfB(0);

            scene.B.LayerManager.MoveItemToManager(moved, scene.A.LayerManager, 0);
            scene.Undo.Undo();

            Assert.Equal(new IControl[] { moved, stays }, scene.A.LayerManager.ManagerData.Items);
            Assert.Equal(new IControl[] { other }, scene.B.LayerManager.ManagerData.Items);
            Assert.Equal(scene.A.ID, moved.CompositionID);
        }

        [Fact]
        public void MoveItemToManager_AnEntitySharedWithAnotherLayerOfTheSource_ChangesNothing()
        {
            var scene = CreateScene();
            var moved = scene.LayerOfA(0);
            var stays = scene.LayerOfA(1);
            moved.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (IPrefab)moved.ModelEntityManager.ManagerData.Items[0];
            stays.ModelEntityManager.AddExistingItem(entity);
            scene.Undo.Clear();

            var result = scene.B.LayerManager.MoveItemToManager(moved, scene.A.LayerManager, 0);

            Assert.False(result.Moved);
            Assert.Same(entity, Assert.Single(result.Blocking));
            Assert.Equal(2, scene.A.LayerManager.ManagerData.Items.Count);
            Assert.Single(scene.B.LayerManager.ManagerData.Items);
            Assert.Equal(scene.A.ID, moved.CompositionID);
            Assert.False(scene.Undo.CanUndo);
        }

        [Fact]
        public void MoveItemToManager_AnEntitySharedWithALayerOfTheTarget_IsAllowed()
        {
            var scene = CreateScene();
            var moved = scene.LayerOfA(0);
            moved.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (IPrefab)moved.ModelEntityManager.ManagerData.Items[0];
            scene.LayerOfB(0).ModelEntityManager.AddExistingItem(entity);

            var result = scene.B.LayerManager.MoveItemToManager(moved, scene.A.LayerManager, 0);

            Assert.True(result.Moved);
            Assert.Contains(moved, scene.B.LayerManager.ManagerData.Items);
        }

        [Fact]
        public void MoveItemToManager_TheSameList_DoesNothing()
        {
            var scene = CreateScene();
            var moved = scene.LayerOfA(0);

            var result = scene.A.LayerManager.MoveItemToManager(moved, scene.A.LayerManager, 1);

            Assert.Same(MoveResult.None, result);
            Assert.Equal(2, scene.A.LayerManager.ManagerData.Items.Count);
            Assert.False(scene.Undo.CanUndo);
        }

        [Fact]
        public void MoveItemToManager_AnItemThatIsNotInTheSource_DoesNothing()
        {
            var scene = CreateScene();
            var notInA = scene.LayerOfB(0);

            scene.B.LayerManager.MoveItemToManager(notInA, scene.A.LayerManager, 0);

            Assert.Single(scene.B.LayerManager.ManagerData.Items);
            Assert.Equal(2, scene.A.LayerManager.ManagerData.Items.Count);
            Assert.False(scene.Undo.CanUndo);
        }

        [Fact]
        public void MoveItemToManager_ASourceWithoutTheItem_ReportsNothingHappened()
        {
            var scene = CreateScene();
            var notInA = scene.LayerOfB(0);

            var result = scene.B.LayerManager.MoveItemToManager(notInA, scene.A.LayerManager, 0);

            Assert.Same(MoveResult.None, result);
            Assert.False(result.Moved);
            Assert.Empty(result.Blocking);
        }

        [Fact]
        public void MoveItemToManager_ARefusedItem_IsNotMovedAndWritesNoUndoStep()
        {
            var scene = CreateScene();
            var moved = scene.LayerOfA(0);
            scene.B.LayerManager.CanAddExistingItem = control => !ReferenceEquals(control, moved);

            var result = scene.B.LayerManager.MoveItemToManager(moved, scene.A.LayerManager, 0);

            Assert.Same(MoveResult.None, result);
            Assert.Equal(2, scene.A.LayerManager.ManagerData.Items.Count);
            Assert.DoesNotContain(moved, scene.B.LayerManager.ManagerData.Items);
            Assert.False(scene.Undo.CanUndo);
        }

        [Fact]
        public void Receive_AnAddThenARemoveOfTheSameLayer_MovesTheSameInstance()
        {
            var sender = TestServiceProviderFactory.Create();
            var senderA = sender.GetRequiredService<PrefabManager>();
            var senderB = sender.GetRequiredService<PrefabManager>();
            senderA.AddItem(typeof(Layer));
            var layer = (Layer)senderA.SelectedItem;
            layer.ModelEntityManager.AddItem(typeof(Entity));
            var factory = sender.GetRequiredService<MessageFactory>();

            var receiver = TestServiceProviderFactory.Create();
            var repository = receiver.GetRequiredService<ControlRepository>();
            var a = receiver.GetRequiredService<PrefabManager>();
            var b = receiver.GetRequiredService<PrefabManager>();
            a.ManagerData.ID = senderA.ManagerData.ID;
            b.ManagerData.ID = senderB.ManagerData.ID;
            a.Receive(factory.CreateMessage<MessageAddItem>(senderA.ManagerData.ID, layer, 0));
            var received = a.ManagerData.Items[0];

            // The sender adds the layer to the target first, then removes it from the source.
            b.Receive(factory.CreateMessage<MessageAddItem>(senderB.ManagerData.ID, layer, 0));
            a.Receive(factory.CreateMessage<MessageRemoveItem>(senderA.ManagerData.ID, layer, -1));

            Assert.Empty(a.ManagerData.Items);
            Assert.Same(received, Assert.Single(b.ManagerData.Items));
            Assert.Same(received, repository.GetControl(layer.ID));
            Assert.Single(repository.Entities);
        }

        [Fact]
        public void GetSharedItems_NothingIsShared_IsEmpty()
        {
            var (first, second, _) = ThreeLayersAsTuple();
            first.ModelEntityManager.AddItem(typeof(Entity));
            second.ModelEntityManager.AddItem(typeof(Entity));

            Assert.Empty(first.GetSharedItems(new[] { first, second }));
        }

        [Fact]
        public void GetSharedItems_AnEntityHeldByAnotherLayer_IsListed()
        {
            var (first, second, _) = ThreeLayersAsTuple();
            first.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (IPrefab)first.ModelEntityManager.SelectedItem;
            second.ModelEntityManager.AddExistingItem(entity);

            var shared = first.GetSharedItems(new[] { first, second });

            Assert.Same(entity, Assert.Single(shared));
        }

        [Fact]
        public void GetSharedItems_ACameraAndALightHeldByAnotherLayer_AreListed()
        {
            var (first, second, _) = ThreeLayersAsTuple();
            first.CameraManager.AddItem(typeof(Camera));
            first.LightManager.AddItem(typeof(LightEntity));
            var camera = (IPrefab)first.CameraManager.SelectedItem;
            var light = (IPrefab)first.LightManager.SelectedItem;
            second.CameraManager.AddExistingItem(camera);
            second.LightManager.AddExistingItem(light);

            var shared = first.GetSharedItems(new[] { second });

            Assert.Equal(new[] { camera, light }, shared);
        }

        [Fact]
        public void GetSharedItems_ALayerOutsideTheList_IsNotCounted()
        {
            var (first, second, third) = ThreeLayersAsTuple();
            first.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (IPrefab)first.ModelEntityManager.SelectedItem;
            second.ModelEntityManager.AddExistingItem(entity);

            Assert.Empty(first.GetSharedItems(new[] { third }));
        }

        [Fact]
        public void GetSharedItems_TheLayerItselfInTheList_IsIgnored()
        {
            var (first, _, _) = ThreeLayersAsTuple();
            first.ModelEntityManager.AddItem(typeof(Entity));

            Assert.Empty(first.GetSharedItems(new[] { first }));
        }

        [Fact]
        public void GetLayersHolding_AnEntityHeldByTwoOtherLayers_ListsBoth()
        {
            var (first, second, third) = ThreeLayersAsTuple();
            first.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (IPrefab)first.ModelEntityManager.SelectedItem;
            second.ModelEntityManager.AddExistingItem(entity);
            third.ModelEntityManager.AddExistingItem(entity);

            var layers = first.GetLayersHolding(entity, new[] { first, second, third });

            Assert.Equal(new[] { second, third }, layers);
        }

        [Fact]
        public void GetLayersHolding_TheLayerItselfInTheList_IsIgnored()
        {
            var (first, _, _) = ThreeLayersAsTuple();
            first.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (IPrefab)first.ModelEntityManager.SelectedItem;

            Assert.Empty(first.GetLayersHolding(entity, new[] { first }));
        }

        [Fact]
        public void GetLayersHolding_AnItemNobodyElseHolds_IsEmpty()
        {
            var (first, second, third) = ThreeLayersAsTuple();
            first.ModelEntityManager.AddItem(typeof(Entity));
            second.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (IPrefab)first.ModelEntityManager.ManagerData.Items[0];

            Assert.Empty(first.GetLayersHolding(entity, new[] { first, second, third }));
        }
    }
}
