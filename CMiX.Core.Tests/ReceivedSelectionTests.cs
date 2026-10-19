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
    // A select message from a peer selects the control with that id, if the control is in the list.
    public class ReceivedSelectionTests
    {
        private sealed class Setup
        {
            public IServiceProvider Provider { get; init; } = null!;
            public PrefabManager Manager { get; init; } = null!;
            public RecordingMessageSender Sent { get; init; } = null!;
            public List<IControl> Items { get; init; } = null!;

            public void Receive(Guid controlID, int index = 0) =>
                Manager.Receive(new MessageSelectedItemChanged(Manager.ManagerData.ID, controlID, index));
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
        public void AControlOfTheList_BecomesTheSelectedItem(int index)
        {
            var setup = ThreeItems();

            setup.Receive(setup.Items[index].ID);

            Assert.Same(setup.Items[index], setup.Manager.SelectedItem);
            Assert.Equal(index, setup.Manager.ManagerData.SelectedIndex);
        }

        [Fact]
        public void AnEmptyID_LeavesNothingSelected()
        {
            var setup = ThreeItems();

            setup.Receive(Guid.Empty);

            Assert.Null(setup.Manager.SelectedItem);
            Assert.Equal(-1, setup.Manager.ManagerData.SelectedIndex);
        }

        [Fact]
        public void AnUnknownID_LeavesNothingSelected()
        {
            var setup = ThreeItems();

            setup.Receive(Guid.NewGuid());

            Assert.Null(setup.Manager.SelectedItem);
        }

        [Fact]
        public void AControlOfAnotherList_LeavesNothingSelected()
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
        public void AReceivedSelection_SendsNothingBack_AndLeavesNoUndoStep()
        {
            var setup = ThreeItems();
            var undo = setup.Provider.GetRequiredService<UndoManager>();

            setup.Receive(setup.Items[0].ID);
            undo.Undo();

            Assert.Empty(setup.Sent.Sent);
            Assert.Same(setup.Items[0], setup.Manager.SelectedItem);
        }
    }
}
