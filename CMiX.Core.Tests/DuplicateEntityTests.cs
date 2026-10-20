using System.Text.Json;
using System.Text.RegularExpressions;
using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
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
    }
}
