using CMiX.Core;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // A move from the buttons or the drag handle is one change of the list, one message for the peers and one undo step.
    public class ReorderThroughManagerTests
    {
        private sealed class Setup
        {
            public PrefabManager Manager { get; init; } = null!;
            public ManagerReorderService Service { get; init; } = null!;
            public UndoManager Undo { get; init; } = null!;
            public List<MessageMoveItem> Moves { get; init; } = null!;
            public List<IControl> Items { get; init; } = null!;

            public List<int> Order => Manager.ManagerData.Items.Select(item => Items.IndexOf(item)).ToList();
        }

        private sealed class MoveRecorder : IMessageSender
        {
            public List<MessageMoveItem> Moves { get; } = new();

            public void SendMessage(IMessage message)
            {
                if (message is MessageMoveItem move) Moves.Add(move);
            }
        }

        private static Setup FourItems()
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = provider.GetRequiredService<PrefabManager>();
            for (var i = 0; i < 4; i++)
                manager.AddItem(typeof(TouchBlob));

            var recorder = new MoveRecorder();
            provider.GetRequiredService<ControlMessenger>().Register(recorder);
            var undo = provider.GetRequiredService<UndoManager>();
            undo.Clear();
            return new Setup
            {
                Manager = manager,
                Service = (ManagerReorderService)manager.ManagerReorderService,
                Undo = undo,
                Moves = recorder.Moves,
                Items = manager.ManagerData.Items.ToList()
            };
        }

        [Fact]
        public void AMoveDownFromTheButton_MovesTheItemSendsOneMessageAndIsOneUndoStep()
        {
            var setup = FourItems();

            setup.Service.ItemDown(setup.Items[0]);

            Assert.Equal(new List<int> { 1, 0, 2, 3 }, setup.Order);
            var move = Assert.Single(setup.Moves);
            Assert.Equal((setup.Manager.ManagerData.ID, 0, 1), (move.ID, move.OldIndex, move.NewIndex));

            setup.Undo.Undo();

            Assert.Equal(new List<int> { 0, 1, 2, 3 }, setup.Order);

            setup.Undo.Undo();

            Assert.Equal(new List<int> { 0, 1, 2, 3 }, setup.Order);
        }

        [Fact]
        public void AMoveUpFromTheButton_MovesTheItemSendsOneMessageAndIsOneUndoStep()
        {
            var setup = FourItems();

            setup.Service.ItemUp(setup.Items[3]);

            Assert.Equal(new List<int> { 0, 1, 3, 2 }, setup.Order);
            var move = Assert.Single(setup.Moves);
            Assert.Equal((3, 2), (move.OldIndex, move.NewIndex));

            setup.Undo.Undo();

            Assert.Equal(new List<int> { 0, 1, 2, 3 }, setup.Order);
        }

        [Fact]
        public void ADrop_MovesTheItemSendsOneMessageAndIsOneUndoStep()
        {
            var setup = FourItems();

            setup.Service.Dropped(0, 3);

            Assert.Equal(new List<int> { 1, 2, 0, 3 }, setup.Order);
            var move = Assert.Single(setup.Moves);
            Assert.Equal((0, 2), (move.OldIndex, move.NewIndex));

            setup.Undo.Undo();

            Assert.Equal(new List<int> { 0, 1, 2, 3 }, setup.Order);
        }

        [Fact]
        public void RedoingAMove_MovesTheItemAgain()
        {
            var setup = FourItems();
            setup.Service.ItemDown(setup.Items[0]);
            setup.Undo.Undo();

            setup.Undo.Redo();

            Assert.Equal(new List<int> { 1, 0, 2, 3 }, setup.Order);
        }

        [Fact]
        public void MovingTheSelectedItem_KeepsItSelected()
        {
            var setup = FourItems();
            setup.Manager.SelectAt(0);
            setup.Undo.Clear();

            setup.Service.ItemDown(setup.Items[0]);

            Assert.Same(setup.Items[0], setup.Manager.SelectedItem);
            Assert.Equal(1, setup.Manager.ManagerData.SelectedIndex);
        }
    }
}
