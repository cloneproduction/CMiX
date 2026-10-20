using System.Text.Json;
using System.Text.RegularExpressions;
using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Modulation.Modifiers;
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
    // Duplicate works for every kind of entity: the entities, the texts and the quads of a layer, and its lights and cameras.
    public class DuplicateEntityTests
    {
        public static IEnumerable<object[]> Kinds() =>
            new[] { typeof(Entity), typeof(TextEntity), typeof(FullscreenQuad), typeof(LightEntity), typeof(Camera) }
                .Select(kind => new object[] { kind });

        private static PrefabManager ManagerOf(Layer layer, Type kind) =>
            kind == typeof(LightEntity) ? layer.LightManager
            : kind == typeof(Camera) ? layer.CameraManager
            : layer.ModelEntityManager;

        private static (IServiceProvider Provider, PrefabManager Manager, List<IControl> Items) ThreeOf(Type kind)
        {
            var provider = TestServiceProviderFactory.Create();
            var layer = (Layer)provider.GetRequiredService<ControlFactory>().Create(typeof(Layer));
            var manager = ManagerOf(layer, kind);
            for (var i = 0; i < 3; i++)
                manager.AddItem(kind);
            return (provider, manager, manager.ManagerData.Items.ToList());
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
        [MemberData(nameof(Kinds))]
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
            var peerLayer = (Layer)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(Layer));
            var peer = ManagerOf(peerLayer, kind);
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

        [Theory]
        [MemberData(nameof(Kinds))]
        public void DuplicateItem_OfAnItemThatLivesInALayer_ThroughTheManagerOfTheRepository_PutsTheCopyAtTheEndOfItsList(Type kind)
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
    }
}
