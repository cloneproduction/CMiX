// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Text.Json;
using System.Text.RegularExpressions;
using CMiX.Core.Compositing;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Duplicate puts a deep copy with new ids and a new name right after the source, as one undo step.
    public class DuplicateItemTests
    {
        private static readonly Type[] LayerContentKinds =
            { typeof(Entity), typeof(TextEntity), typeof(FullscreenQuad), typeof(LightEntity), typeof(Camera) };

        public static IEnumerable<object[]> Kinds() =>
            new[] { typeof(Composition), typeof(Layer) }.Concat(LayerContentKinds).Select(kind => new object[] { kind });

        public static IEnumerable<object[]> LayerKinds() =>
            LayerContentKinds.Select(kind => new object[] { kind });

        // The list that holds the kind, on a new owner of the given provider.
        private static PrefabManager ListOf(IServiceProvider provider, Type kind)
        {
            if (kind == typeof(Composition))
                return provider.GetRequiredService<PrefabManager>();
            var factory = provider.GetRequiredService<ControlFactory>();
            if (kind == typeof(Layer))
                return ((Composition)factory.Create(typeof(Composition))).LayerManager;
            var layer = (Layer)factory.Create(typeof(Layer));
            return kind == typeof(LightEntity) ? layer.LightManager
                : kind == typeof(Camera) ? layer.CameraManager
                : layer.ModelEntityManager;
        }

        private static (IServiceProvider Provider, PrefabManager Manager, List<IControl> Items) ThreeOf(Type kind)
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = ListOf(provider, kind);
            for (var i = 0; i < 3; i++)
                manager.AddItem(kind);
            return (provider, manager, manager.ManagerData.Items.ToList());
        }

        private static (IServiceProvider provider, Composition composition, List<Layer> layers, PrefabManager compositions) CompositionWithLayers(int layerCount)
        {
            var provider = TestServiceProviderFactory.Create();
            var compositions = provider.GetRequiredService<PrefabManager>();
            compositions.AddItem(typeof(Composition));
            var composition = (Composition)compositions.SelectedItem;
            for (var i = 0; i < layerCount; i++)
                composition.LayerManager.AddItem(typeof(Layer));
            return (provider, composition, composition.LayerManager.ManagerData.Items.Cast<Layer>().ToList(), compositions);
        }

        private static (IServiceProvider Provider, Layer Layer, Entity Entity) EntityWithATextureAndAModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var layer = (Layer)provider.GetRequiredService<ControlFactory>().Create(typeof(Layer));
            layer.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (Entity)layer.ModelEntityManager.ManagerData.Items[0];
            entity.Material.DiffuseTexture.TextureManager.AddItem(typeof(CheckerBoard));
            entity.ModifierManager.AddItem(typeof(AlphaModifier));
            provider.GetRequiredService<UndoManager>().Clear();
            return (provider, layer, entity);
        }

        private static HashSet<string> IdsOf(IControl control)
        {
            var json = JsonSerializer.Serialize(control.ToModel(), typeof(IControlModel), ProjectSerializer.Options);
            return Regex.Matches(json, "[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}")
                .Select(match => match.Value)
                .ToHashSet();
        }

        [Theory]
        [MemberData(nameof(Kinds))]
        public void DuplicateItem_OfEveryKind_PutsTheCopyRightAfterTheSourceAndSelectsIt(Type kind)
        {
            var (_, manager, items) = ThreeOf(kind);

            var copy = manager.DuplicateItem(items[0]);

            Assert.IsType(kind, copy);
            Assert.Equal(new[] { items[0], copy, items[1], items[2] }, manager.ManagerData.Items);
            Assert.Equal(1, manager.ManagerData.SelectedIndex);
            Assert.Same(copy, manager.SelectedItem);
        }

        [Theory]
        [MemberData(nameof(Kinds))]
        public void DuplicateItem_OfEveryKind_GivesTheCopyTheNextFreeName(Type kind)
        {
            var (_, manager, items) = ThreeOf(kind);
            ((IPrefab)items[0]).PrefabService.Name.Value = "Hero";

            var copy = (IPrefab)manager.DuplicateItem(items[0]);

            Assert.Equal("Hero.001", copy.PrefabService.Name.Value);
        }

        [Theory]
        [MemberData(nameof(LayerKinds))]
        public void DuplicateItem_OfEveryKind_SharesNoIdWithTheSource(Type kind)
        {
            var (_, manager, items) = ThreeOf(kind);

            var copy = manager.DuplicateItem(items[0]);

            Assert.NotEmpty(IdsOf(copy));
            Assert.Empty(IdsOf(items[0]).Intersect(IdsOf(copy)));
        }

        [Theory]
        [MemberData(nameof(Kinds))]
        public void DuplicateItem_OfEveryKind_UndoRemovesTheCopyAndRedoPutsItBack(Type kind)
        {
            var (provider, manager, items) = ThreeOf(kind);
            var undo = provider.GetRequiredService<UndoManager>();
            undo.Clear();

            var copy = manager.DuplicateItem(items[0]);
            undo.Undo();
            Assert.Equal(items, manager.ManagerData.Items);

            undo.Redo();
            Assert.Equal(new[] { items[0], copy, items[1], items[2] }, manager.ManagerData.Items);
        }

        [Theory]
        [MemberData(nameof(Kinds))]
        public void DuplicateItem_OfEveryKind_SendsOneAddMessageWithTheIndexOfTheCopy(Type kind)
        {
            var (provider, manager, items) = ThreeOf(kind);
            var sender = new RecordingMessageSender();
            provider.GetRequiredService<ControlMessenger>().Register(sender);

            var copy = manager.DuplicateItem(items[0]);

            var message = Assert.Single(sender.Sent.OfType<MessageAddItem>());
            Assert.Equal(manager.ManagerData.ID, message.ID);
            Assert.Equal(copy.ID, message.Model.ID);
            Assert.Equal(1, message.SelectedIndex);
        }

        [Theory]
        [MemberData(nameof(Kinds))]
        public void DuplicateItem_OfEveryKind_AnotherPeerShowsTheSameOrder(Type kind)
        {
            var (provider, manager, items) = ThreeOf(kind);
            var peer = ListOf(TestServiceProviderFactory.Create(), kind);
            peer.ManagerData.ID = manager.ManagerData.ID;
            for (var i = 0; i < items.Count; i++)
                peer.Receive(new MessageAddItem(manager.ManagerData.ID, items[i].ToModel(), i));
            var sender = new RecordingMessageSender();
            provider.GetRequiredService<ControlMessenger>().Register(sender);

            manager.DuplicateItem(items[0]);
            peer.Receive(sender.Sent.OfType<MessageAddItem>().Single());

            Assert.Equal(
                manager.ManagerData.Items.Select(item => item.ID),
                peer.ManagerData.Items.Select(item => item.ID));
        }

        [Theory]
        [MemberData(nameof(LayerKinds))]
        public void DuplicateItem_ThroughTheRepositoryManager_PutsTheCopyAtTheEndOfThatList(Type kind)
        {
            var (provider, layerManager, items) = ThreeOf(kind);
            var repositoryManager = provider.GetRequiredService<PrefabManager>();
            var repository = provider.GetRequiredService<ControlRepository>();

            var copy = repositoryManager.DuplicateItem(items[1]);

            Assert.IsType(kind, copy);
            Assert.Same(copy, Assert.Single(repositoryManager.ManagerData.Items));
            Assert.Same(copy, repository.GetControl(copy.ID));
            Assert.Equal(items, layerManager.ManagerData.Items);
        }

        [Fact]
        public void DuplicateItem_OfAnEntityWithATextureAndAModifier_GivesTheCopyItsOwnOnes()
        {
            var (provider, layer, entity) = EntityWithATextureAndAModifier();
            var repository = provider.GetRequiredService<ControlRepository>();
            var texture = entity.Material.DiffuseTexture.TextureManager.ManagerData.Items.Single();
            var modifier = entity.ModifierManager.ManagerData.Items.Single();
            var controlsBefore = repository.Controls.Count;

            var copy = (Entity)layer.ModelEntityManager.DuplicateItem(entity);

            var copyTexture = copy.Material.DiffuseTexture.TextureManager.ManagerData.Items.Single();
            var copyModifier = copy.ModifierManager.ManagerData.Items.Single();
            Assert.NotSame(texture, copyTexture);
            Assert.NotSame(modifier, copyModifier);
            Assert.NotEqual(texture.ID, copyTexture.ID);
            Assert.NotEqual(modifier.ID, copyModifier.ID);
            Assert.Same(copyTexture, repository.GetControl(copyTexture.ID));
            Assert.Same(copyModifier, repository.GetControl(copyModifier.ID));
            Assert.Equal(controlsBefore + 3, repository.Controls.Count);
        }

        [Fact]
        public void DuplicateItem_OfAnEntityWithATextureAndAModifier_UndoFreesTheCopyWhenTheStepIsDropped()
        {
            var (provider, layer, entity) = EntityWithATextureAndAModifier();
            var repository = provider.GetRequiredService<ControlRepository>();
            var undo = provider.GetRequiredService<UndoManager>();
            var controlsBefore = repository.Controls.Count;

            var copy = (Entity)layer.ModelEntityManager.DuplicateItem(entity);
            var copyTextureID = copy.Material.DiffuseTexture.TextureManager.ManagerData.Items.Single().ID;
            undo.Undo();
            undo.Clear();

            Assert.Equal(controlsBefore, repository.Controls.Count);
            Assert.Null(repository.GetControl(copyTextureID));
            Assert.Null(repository.GetControl(copy.ID));
        }

        [Fact]
        public void DuplicateItem_OfAnEntityThatTwoLayersShare_GivesTheCopyToThisLayerOnly()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var first = (Layer)factory.Create(typeof(Layer));
            var second = (Layer)factory.Create(typeof(Layer));
            first.ModelEntityManager.AddItem(typeof(Entity));
            var entity = first.ModelEntityManager.ManagerData.Items[0];
            second.ModelEntityManager.AddExistingItem(entity);

            var copy = first.ModelEntityManager.DuplicateItem(entity);

            Assert.Equal(new[] { entity, copy }, first.ModelEntityManager.ManagerData.Items);
            Assert.Same(entity, Assert.Single(second.ModelEntityManager.ManagerData.Items));
            Assert.Equal(2, repository.GetCollectionManagers(entity).Count);
            Assert.Single(repository.GetCollectionManagers(copy));
        }

        [Fact]
        public void DuplicateItem_OfALayer_GivesTheCopyNewIdsTheValuesAndTheNextName()
        {
            var (_, composition, layers, _) = CompositionWithLayers(1);
            layers[0].Compositing.Opacity.Value = 0.25f;

            var copy = (Layer)composition.LayerManager.DuplicateItem(layers[0]);

            Assert.NotEqual(layers[0].ID, copy.ID);
            Assert.NotEqual(layers[0].Compositing.ID, copy.Compositing.ID);
            Assert.NotEqual(layers[0].Compositing.Opacity.ID, copy.Compositing.Opacity.ID);
            Assert.Equal(0.25f, copy.Compositing.Opacity.Value);
            Assert.Equal(layers[0].PrefabService.Name.Value + ".001", copy.PrefabService.Name.Value);
        }

        [Fact]
        public void DuplicateItem_OfACopy_CountsTheNameUp()
        {
            var (_, composition, layers, _) = CompositionWithLayers(1);
            var manager = composition.LayerManager;

            var first = manager.DuplicateItem(layers[0]);
            var second = (Layer)manager.DuplicateItem(first);

            Assert.EndsWith(".002", second.PrefabService.Name.Value);
        }

        [Fact]
        public void DuplicateItem_ForAnItemOfAnotherList_PutsTheCopyAtTheEndOfThisListAsOneUndoStep()
        {
            var (provider, composition, layers, compositions) = CompositionWithLayers(1);
            compositions.AddItem(typeof(Composition));
            var otherComposition = (Composition)compositions.SelectedItem;
            otherComposition.LayerManager.AddItem(typeof(Layer));
            var foreign = otherComposition.LayerManager.ManagerData.Items[0];
            var undo = provider.GetRequiredService<UndoManager>();
            undo.Clear();

            var copy = composition.LayerManager.DuplicateItem(foreign);

            Assert.Equal(new[] { layers[0], copy }, composition.LayerManager.ManagerData.Items);
            Assert.NotEqual(foreign.ID, copy.ID);
            Assert.Same(copy, composition.LayerManager.SelectedItem);
            Assert.Same(foreign, Assert.Single(otherComposition.LayerManager.ManagerData.Items));

            undo.Undo();

            Assert.Single(composition.LayerManager.ManagerData.Items);
        }

        [Fact]
        public void DuplicateItem_OfNothing_DoesNothing()
        {
            var (_, composition, _, _) = CompositionWithLayers(1);

            Assert.Null(composition.LayerManager.DuplicateItem(null));
            Assert.Single(composition.LayerManager.ManagerData.Items);
        }

        [Fact]
        public void DuplicateItem_OfALayer_AddsNoUndoStepForTheName()
        {
            var (provider, composition, layers, _) = CompositionWithLayers(1);
            var undo = provider.GetRequiredService<UndoManager>();
            composition.LayerManager.DuplicateItem(layers[0]);

            undo.Undo();

            Assert.Single(composition.LayerManager.ManagerData.Items);
            Assert.Equal(layers[0].PrefabService.Name.Value, ((Layer)composition.LayerManager.ManagerData.Items[0]).PrefabService.Name.Value);
        }

        [Fact]
        public void DuplicateItem_OfAComposition_CopiesItsLayersWithNewIdsAndKeepsTheOutputMapping()
        {
            var (_, composition, layers, compositions) = CompositionWithLayers(2);

            var copy = (Composition)compositions.DuplicateItem(composition);

            Assert.Equal(2, copy.LayerManager.ManagerData.Items.Count);
            Assert.Empty(copy.LayerManager.ManagerData.Items.Select(item => item.ID).Intersect(layers.Select(layer => layer.ID)));
            Assert.Equal(composition.SelectedOutputMappingID.Value, copy.SelectedOutputMappingID.Value);
            Assert.Equal(new IControl[] { composition, copy }, compositions.ManagerData.Items);
        }

        [Fact]
        public void DuplicateItemCommand_OfAnItem_PutsTheCopyRightAfterTheSource()
        {
            var manager = TestServiceProviderFactory.Create().GetRequiredService<PrefabManager>();
            manager.AddItem(typeof(Entity));
            manager.AddItem(typeof(Entity));
            var source = manager.ManagerData.Items[0];

            manager.DuplicateItemCommand.Execute(source);

            Assert.Equal(3, manager.ManagerData.Items.Count);
            Assert.NotSame(source, manager.ManagerData.Items[1]);
            Assert.IsType<Entity>(manager.ManagerData.Items[1]);
            Assert.Same(source, manager.ManagerData.Items[0]);
        }
    }
}
