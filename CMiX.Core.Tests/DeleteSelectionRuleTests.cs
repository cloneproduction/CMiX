using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Deleting an item moves the selection only when the deleted item was the selected one.
    public class DeleteSelectionRuleTests
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

        [Theory]
        [InlineData(2, 0, 1)]
        [InlineData(2, 1, 1)]
        [InlineData(0, 1, 0)]
        [InlineData(0, 2, 0)]
        [InlineData(1, 0, 0)]
        [InlineData(1, 2, 1)]
        public void DeletingAnItemThatIsNotSelected_KeepsTheSelectionOnTheSameItem(int selected, int deleted, int expectedIndex)
        {
            var setup = ThreeItemsWithTheSelectionOn(selected);
            var selectedItem = setup.Items[selected];

            setup.Manager.RemoveItem(setup.Items[deleted]);

            Assert.Same(selectedItem, setup.Manager.SelectedItem);
            Assert.Equal(expectedIndex, setup.Manager.ManagerData.SelectedIndex);
        }

        [Fact]
        public void DeletingTheSelectedItem_SelectsTheItemBeforeIt()
        {
            var setup = ThreeItemsWithTheSelectionOn(2);

            setup.Manager.RemoveItem(setup.Items[2]);

            Assert.Same(setup.Items[1], setup.Manager.SelectedItem);
        }

        [Fact]
        public void DeletingAnItemThatIsNotSelected_SendsTheIndexOfTheKeptSelection()
        {
            var setup = ThreeItemsWithTheSelectionOn(2);

            setup.Manager.RemoveItem(setup.Items[0]);

            Assert.Equal(1, Assert.Single(setup.Sent.Sent.OfType<MessageRemoveItem>()).SelectedIndex);
            Assert.Empty(setup.Sent.Sent.OfType<MessageSelectItem>());
        }

        [Fact]
        public void UndoingTheDeleteOfAnItemThatWasNotSelected_SelectsTheSameItemAsBefore()
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
        public void UndoingTheDeleteOfTheSelectedItem_SelectsTheRestoredItem()
        {
            var setup = ThreeItemsWithTheSelectionOn(1);
            setup.Manager.RemoveItem(setup.Items[1]);

            setup.Undo.Undo();

            Assert.Same(setup.Items[1], setup.Manager.SelectedItem);
            Assert.Equal(1, setup.Manager.ManagerData.SelectedIndex);
        }

        [Fact]
        public void RedoingTheDeleteOfAnItemThatWasNotSelected_KeepsTheSelectionOnTheSameItem()
        {
            var setup = ThreeItemsWithTheSelectionOn(2);
            setup.Manager.RemoveItem(setup.Items[0]);
            setup.Undo.Undo();

            setup.Undo.Redo();

            Assert.Same(setup.Items[2], setup.Manager.SelectedItem);
            Assert.Equal(1, setup.Manager.ManagerData.SelectedIndex);
        }

        [Fact]
        public void ARemoveFromAPeer_KeepsTheSelectionOnTheSameItem()
        {
            var setup = ThreeItemsWithTheSelectionOn(2);

            setup.Manager.Receive(new MessageRemoveItem(setup.Manager.ManagerData.ID, setup.Items[0].ID, 1));

            Assert.Same(setup.Items[2], setup.Manager.SelectedItem);
            Assert.Equal(1, setup.Manager.ManagerData.SelectedIndex);
        }
    }
}
