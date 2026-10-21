using System.Reflection;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    public class SelectionEchoTests
    {
        private sealed class MessageRecorder : IMessageSender
        {
            public List<IMessage> Sent { get; } = new();
            public void SendMessage(IMessage message) => Sent.Add(message);
        }

        private sealed class Harness
        {
            private Harness() { }

            public UndoManager Undo { get; private init; } = null!;
            public PrefabManager Compositions { get; private init; } = null!;
            public PrefabManager Layers { get; private init; } = null!;
            public ListBox List { get; private init; } = null!;
            public MessageRecorder Recorder { get; } = new();

            public int UndoSteps => StackSize("_undoStack");
            public int RedoSteps => StackSize("_redoStack");
            public int Selected => Layers.ManagerData.SelectedIndex;
            public IReadOnlyList<Guid> Ids => Layers.ManagerData.Items.Select(item => item.ID).ToList();

            public static Harness Create(int layerCount = 0) => Create(layerCount, mirror: null);

            public static Harness CreatePeerOf(Harness sender) => Create(0, sender);

            private static Harness Create(int layerCount, Harness? mirror)
            {
                var provider = TestServiceProviderFactory.Create();
                var compositions = provider.GetRequiredService<PrefabManager>();
                if (mirror == null)
                    compositions.AddItem(typeof(Composition));
                else
                    compositions.Receive(new MessageAddItem(compositions.ManagerData.ID, mirror.Compositions.SelectedItem.ToModel(), 0));
                var layers = ((Composition)compositions.ManagerData.Items.Single()).LayerManager;

                var view = new ComposableManager { ItemType = typeof(Layer), DataContext = layers };
                new Window { Content = view, Width = 400, Height = 300 }.Show();
                Dispatcher.UIThread.RunJobs();

                var harness = new Harness
                {
                    Undo = provider.GetRequiredService<UndoManager>(),
                    Compositions = compositions,
                    Layers = layers,
                    List = view.GetVisualDescendants().OfType<ListBox>().First()
                };
                for (var i = 0; i < layerCount; i++)
                    layers.AddItem(typeof(Layer));
                Dispatcher.UIThread.RunJobs();

                harness.Undo.Clear();
                provider.GetRequiredService<ControlMessenger>().Register(harness.Recorder);
                return harness;
            }

            public void Do(Action action)
            {
                Recorder.Sent.Clear();
                action();
                Dispatcher.UIThread.RunJobs();
            }

            public IReadOnlyList<T> Sent<T>() => Recorder.Sent.OfType<T>().ToList();

            private int StackSize(string field) =>
                ((System.Collections.ICollection)typeof(UndoManager)
                    .GetField(field, BindingFlags.NonPublic | BindingFlags.Instance)!
                    .GetValue(Undo)!).Count;
        }

        [AvaloniaFact]
        public void Add_RecordsOneUndoStepAndSelectsTheNewItem()
        {
            var harness = Harness.Create();

            for (var i = 0; i < 3; i++)
            {
                harness.Do(() => harness.Layers.AddItem(typeof(Layer)));

                Assert.Equal(i + 1, harness.UndoSteps);
                Assert.Equal(i, harness.Selected);
                Assert.Equal(i, harness.List.SelectedIndex);
                Assert.Equal(i, Assert.Single(harness.Sent<MessageAddItem>()).SelectedIndex);
                Assert.Empty(harness.Sent<MessageRemoveItem>());
                Assert.Empty(harness.Sent<MessageMoveItem>());
            }
        }

        [AvaloniaFact]
        public void UserSelect_RecordsOneUndoStepAndSendsOneSelectMessage()
        {
            var harness = Harness.Create(3);

            harness.Do(() => harness.List.SelectedIndex = 0);

            Assert.Equal(1, harness.UndoSteps);
            Assert.Equal(0, harness.Selected);
            Assert.Equal(0, Assert.Single(harness.Sent<MessageSelectItem>()).Index);
            Assert.Single(harness.Recorder.Sent);
        }

        [AvaloniaFact]
        public void UndoAndRedoOfSelects_MoveTheSelectionAndRecordNothingNew()
        {
            var harness = Harness.Create(3);
            harness.Do(() => harness.List.SelectedIndex = 0);
            harness.Do(() => harness.List.SelectedIndex = 1);
            Assert.Equal(2, harness.UndoSteps);

            harness.Do(harness.Undo.Undo);
            Assert.Equal((0, 1, 1), (harness.Selected, harness.UndoSteps, harness.RedoSteps));
            Assert.Equal(0, harness.List.SelectedIndex);

            harness.Do(harness.Undo.Undo);
            Assert.Equal((2, 0, 2), (harness.Selected, harness.UndoSteps, harness.RedoSteps));
            Assert.Equal(2, harness.List.SelectedIndex);

            harness.Do(harness.Undo.Redo);
            Assert.Equal((0, 1, 1), (harness.Selected, harness.UndoSteps, harness.RedoSteps));
            Assert.Equal(0, harness.List.SelectedIndex);
        }

        [AvaloniaFact]
        public void UndoAndRedoOfAnAdd_BringBackThePreviousSelection()
        {
            var harness = Harness.Create(2);
            harness.Do(() => harness.Layers.AddItem(typeof(Layer)));
            var ids = harness.Ids;

            harness.Do(harness.Undo.Undo);

            Assert.Equal(ids.Take(2), harness.Ids);
            Assert.Equal((1, 0, 1), (harness.Selected, harness.UndoSteps, harness.RedoSteps));
            Assert.Equal(1, harness.List.SelectedIndex);

            harness.Do(harness.Undo.Redo);

            Assert.Equal(ids, harness.Ids);
            Assert.Equal((2, 1, 0), (harness.Selected, harness.UndoSteps, harness.RedoSteps));
            Assert.Equal(2, harness.List.SelectedIndex);
        }

        [AvaloniaFact]
        public void Duplicate_IsOneUndoStepAndUndoBringsBackTheSelection()
        {
            var harness = Harness.Create(3);
            var ids = harness.Ids;

            harness.Do(() => harness.Layers.DuplicateItem(harness.Layers.ManagerData.Items[0]));

            Assert.Equal(4, harness.Ids.Count);
            Assert.Equal(ids[0], harness.Ids[0]);
            Assert.Equal(ids.Skip(1), harness.Ids.Skip(2));
            Assert.Equal((1, 1), (harness.Selected, harness.UndoSteps));
            Assert.Equal(1, harness.List.SelectedIndex);

            harness.Do(harness.Undo.Undo);

            Assert.Equal(ids, harness.Ids);
            Assert.Equal((2, 0, 1), (harness.Selected, harness.UndoSteps, harness.RedoSteps));
            Assert.Equal(2, harness.List.SelectedIndex);
        }

        [AvaloniaFact]
        public void Delete_IsOneUndoStepAndUndoBringsTheItemBack()
        {
            var harness = Harness.Create(3);
            var ids = harness.Ids;

            harness.Do(() => harness.Layers.RemoveItem(harness.Layers.ManagerData.Items[1]));

            Assert.Equal(new[] { ids[0], ids[2] }, harness.Ids);
            Assert.Equal(1, harness.UndoSteps);
            Assert.Single(harness.Sent<MessageRemoveItem>());

            harness.Do(harness.Undo.Undo);

            Assert.Equal(ids, harness.Ids);
            Assert.Equal((0, 1), (harness.UndoSteps, harness.RedoSteps));
            Assert.Equal(harness.Selected, harness.List.SelectedIndex);
        }

        [AvaloniaFact]
        public void Move_IsOneUndoStepAndUndoPutsTheItemBack()
        {
            var harness = Harness.Create(3);
            var ids = harness.Ids;

            harness.Do(() => harness.Layers.MoveItemTo(harness.Layers.ManagerData.Items[0], 2));

            Assert.Equal(new[] { ids[1], ids[2], ids[0] }, harness.Ids);
            Assert.Equal(1, harness.UndoSteps);
            Assert.Single(harness.Sent<MessageMoveItem>());

            harness.Do(harness.Undo.Undo);

            Assert.Equal(ids, harness.Ids);
            Assert.Equal((0, 1), (harness.UndoSteps, harness.RedoSteps));
        }

        private static (Harness Sender, Harness Peer) ReplayOnAPeer()
        {
            var sender = Harness.Create();
            var peer = Harness.CreatePeerOf(sender);
            Assert.Equal(sender.Layers.ManagerData.ID, peer.Layers.ManagerData.ID);

            sender.Do(() =>
            {
                sender.Layers.AddItem(typeof(Layer));
                sender.Layers.AddItem(typeof(Layer));
                sender.Layers.AddItem(typeof(Layer));
                sender.List.SelectedIndex = 0;
            });

            peer.Do(() =>
            {
                foreach (var message in sender.Recorder.Sent.ToList())
                    peer.Layers.Receive(message);
            });
            return (sender, peer);
        }

        [AvaloniaFact]
        public void PeerReceivingTheMessages_EndsWithTheSameItemsAndSelection()
        {
            var (sender, peer) = ReplayOnAPeer();

            Assert.Equal(sender.Ids, peer.Ids);
            Assert.Equal(sender.Selected, peer.Selected);
            Assert.Equal(sender.Selected, peer.List.SelectedIndex);
        }

        [AvaloniaFact]
        public void PeerReceivingTheMessages_RecordsNoUndoStep()
        {
            var (_, peer) = ReplayOnAPeer();

            Assert.Equal(0, peer.UndoSteps);
        }

        [AvaloniaFact]
        public void PeerReceivingTheMessages_SendsNothingBack()
        {
            var (_, peer) = ReplayOnAPeer();

            Assert.Empty(peer.Recorder.Sent);
        }
    }
}
