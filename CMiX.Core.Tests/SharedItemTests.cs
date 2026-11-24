// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Compositing;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // One item can sit in the lists of two layers. Every peer holds it as one object, and the repository drops it with the last list.
    public class SharedItemTests
    {
        private static (Composition composition, Layer first, Layer second) CompositionWithTwoLayers(IServiceProvider provider)
        {
            var compositionManager = provider.GetRequiredService<PrefabManager>();
            compositionManager.AddItem(typeof(Composition));
            var composition = (Composition)compositionManager.SelectedItem;
            composition.LayerManager.AddItem(typeof(Layer));
            composition.LayerManager.AddItem(typeof(Layer));
            var layers = composition.LayerManager.ManagerData.Items;
            return (composition, (Layer)layers[0], (Layer)layers[1]);
        }

        private static (Composition composition, Layer first, Layer second, Entity entity) CreateSharedEntity(IServiceProvider provider)
        {
            var (composition, first, second) = CompositionWithTwoLayers(provider);
            first.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (Entity)first.ModelEntityManager.SelectedItem;
            second.ModelEntityManager.AddExistingItem(entity);
            return (composition, first, second, entity);
        }

        // Two receiving lists that follow the two sending lists by ID.
        private static (PrefabManager first, PrefabManager second, ControlRepository repository, PrefabManager senderFirst, PrefabManager senderSecond, Entity entity, MessageFactory factory) CreateReceivingPair()
        {
            var sender = TestServiceProviderFactory.Create();
            var senderFirst = sender.GetRequiredService<PrefabManager>();
            var senderSecond = sender.GetRequiredService<PrefabManager>();
            senderFirst.AddItem(typeof(Entity));
            var entity = (Entity)senderFirst.SelectedItem;
            senderSecond.AddExistingItem(entity);

            var receiver = TestServiceProviderFactory.Create();
            var first = receiver.GetRequiredService<PrefabManager>();
            var second = receiver.GetRequiredService<PrefabManager>();
            first.ManagerData.ID = senderFirst.ManagerData.ID;
            second.ManagerData.ID = senderSecond.ManagerData.ID;

            return (first, second, receiver.GetRequiredService<ControlRepository>(), senderFirst, senderSecond, entity,
                sender.GetRequiredService<MessageFactory>());
        }

        // The second layer is the slot. The entity of the first layer is the item of the repository.
        private static (UndoManager Undo, PrefabManager Slot, Entity Shared) SlotAndAnEntityOfTheRepository()
        {
            var provider = TestServiceProviderFactory.Create();
            var (_, first, second) = CompositionWithTwoLayers(provider);
            first.ModelEntityManager.AddItem(typeof(Entity));
            var shared = (Entity)first.ModelEntityManager.SelectedItem;
            return (provider.GetRequiredService<UndoManager>(), second.ModelEntityManager, shared);
        }

        [Fact]
        public void RemoveItem_OfASharedEntityFromBothLayers_TakesItOutOfTheRepository()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();
            var (_, first, second, entity) = CreateSharedEntity(provider);

            first.ModelEntityManager.RemoveItem(entity);
            Assert.Same(entity, repository.GetControl(entity.ID));

            second.ModelEntityManager.RemoveItem(entity);
            Assert.Null(repository.GetControl(entity.ID));
            Assert.Empty(repository.Entities);
        }

        [Fact]
        public void Receive_ASecondAddOfTheSameEntity_ReusesTheFirstInstance()
        {
            var (first, second, repository, senderFirst, senderSecond, entity, factory) = CreateReceivingPair();

            first.Receive(factory.CreateMessage<MessageAddItem>(senderFirst.ManagerData.ID, entity, 0));
            second.Receive(factory.CreateMessage<MessageAddItem>(senderSecond.ManagerData.ID, entity, 0));

            Assert.Same(first.ManagerData.Items[0], second.ManagerData.Items[0]);
            Assert.Single(repository.Entities);
        }

        [Fact]
        public void Receive_TheRemoveOfASharedEntityFromBothLists_TakesItOutOfTheRepository()
        {
            var (first, second, repository, senderFirst, senderSecond, entity, factory) = CreateReceivingPair();
            first.Receive(factory.CreateMessage<MessageAddItem>(senderFirst.ManagerData.ID, entity, 0));
            second.Receive(factory.CreateMessage<MessageAddItem>(senderSecond.ManagerData.ID, entity, 0));

            first.Receive(factory.CreateMessage<MessageRemoveItem>(senderFirst.ManagerData.ID, entity, -1));
            Assert.NotNull(repository.GetControl(entity.ID));
            Assert.Single(second.ManagerData.Items);

            second.Receive(factory.CreateMessage<MessageRemoveItem>(senderSecond.ManagerData.ID, entity, -1));
            Assert.Null(repository.GetControl(entity.ID));
            Assert.Empty(repository.Entities);
            Assert.Empty(repository.Controls);
        }

        [Fact]
        public void AddItemFromModel_OfAnEntityInTwoLayers_BuildsOneEntity()
        {
            var sender = TestServiceProviderFactory.Create();
            var (senderComposition, _, _, _) = CreateSharedEntity(sender);
            var compositionModel = senderComposition.ToModel();

            var receiver = TestServiceProviderFactory.Create();
            var repository = receiver.GetRequiredService<ControlRepository>();
            var project = receiver.GetRequiredService<Project>();
            project.CompositionManager.AddItemFromModel(compositionModel);

            var composition = (Composition)project.CompositionManager.SelectedItem;
            var layers = composition.LayerManager.ManagerData.Items.Cast<Layer>().ToList();
            Assert.Equal(2, layers.Count);
            Assert.Same(layers[0].ModelEntityManager.ManagerData.Items[0], layers[1].ModelEntityManager.ManagerData.Items[0]);
            Assert.Single(repository.Entities);

            var entity = layers[0].ModelEntityManager.ManagerData.Items[0];
            layers[0].ModelEntityManager.RemoveItem(entity);
            layers[1].ModelEntityManager.RemoveItem(entity);
            Assert.Empty(repository.Entities);
        }

        [Fact]
        public void RemoveItem_OfASharedQuadFromBothLayers_TakesItOutOfTheRepository()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();
            var (_, first, second) = CompositionWithTwoLayers(provider);

            first.ModelEntityManager.AddItem(typeof(FullscreenQuad));
            var quad = first.ModelEntityManager.ManagerData.Items[0];
            second.ModelEntityManager.AddExistingItem(quad);

            Assert.Same(quad, second.ModelEntityManager.ManagerData.Items[0]);
            Assert.Same(quad, Assert.Single(repository.ModelEntities));

            first.ModelEntityManager.RemoveItem(quad);
            Assert.Same(quad, repository.GetControl(quad.ID));
            Assert.Same(quad, Assert.Single(repository.ModelEntities));

            second.ModelEntityManager.RemoveItem(quad);
            Assert.Null(repository.GetControl(quad.ID));
            Assert.Empty(repository.ModelEntities);
        }

        [Fact]
        public void Clear_OfTheUndoEntryOfARemovedQuad_DisposesBothTextureSlotManagers()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var (_, layer, _) = CompositionWithTwoLayers(provider);

            var heldBeforeTheQuad = HeldCollectionManagers.Count(repository);

            layer.ModelEntityManager.AddItem(typeof(FullscreenQuad));
            var quad = (FullscreenQuad)layer.ModelEntityManager.SelectedItem;
            quad.Texture.DiffuseTexture.TextureManager.AddItem(typeof(CheckerBoard));
            quad.Texture.MaskTexture.TextureManager.AddItem(typeof(CheckerBoard));
            var textures = quad.Texture.DiffuseTexture.TextureManager.ManagerData.Items
                .Concat(quad.Texture.MaskTexture.TextureManager.ManagerData.Items)
                .ToList();

            Assert.True(HeldCollectionManagers.Count(repository) > heldBeforeTheQuad);

            layer.ModelEntityManager.RemoveItem(quad);
            undoManager.Clear();

            Assert.Null(repository.GetControl(quad.ID));
            Assert.All(textures, texture => Assert.Null(repository.GetControl(texture.ID)));
            Assert.Equal(heldBeforeTheQuad, HeldCollectionManagers.Count(repository));
        }

        [Fact]
        public void Undo_AfterAddExistingAndRemove_BringsTheItemBack()
        {
            var (undo, slot, shared) = SlotAndAnEntityOfTheRepository();

            slot.AddExistingItem(shared);
            Assert.Contains(shared, slot.ManagerData.Items);

            slot.RemoveItem(shared);
            Assert.DoesNotContain(shared, slot.ManagerData.Items);

            undo.Undo();

            Assert.Contains(shared, slot.ManagerData.Items);
        }

        [Fact]
        public void Redo_AfterAddExistingRemoveAndUndo_RemovesTheItemAgain()
        {
            var (undo, slot, shared) = SlotAndAnEntityOfTheRepository();
            slot.AddExistingItem(shared);
            slot.RemoveItem(shared);

            undo.Undo();
            undo.Redo();

            Assert.DoesNotContain(shared, slot.ManagerData.Items);
        }

        [Fact]
        public void Redo_AfterAddExistingAndUndo_PutsTheItemBack()
        {
            var (undo, slot, shared) = SlotAndAnEntityOfTheRepository();
            slot.AddExistingItem(shared);

            undo.Undo();
            Assert.DoesNotContain(shared, slot.ManagerData.Items);
            Assert.True(undo.CanRedo);

            undo.Redo();

            Assert.Contains(shared, slot.ManagerData.Items);
        }
    }
}
