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
    // The selected index goes over the wire in the messages and into the file of a project.
    // Both formats stay the same, whatever the manager stores inside.
    public class SelectionIndexContractTests
    {
        [Fact]
        public void TheMessagesCarryTheIndexOfTheItemThatIsSelectedAfterTheEdit()
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = provider.GetRequiredService<PrefabManager>();
            var undo = provider.GetRequiredService<UndoManager>();
            var recorder = new RecordingMessageSender();
            provider.GetRequiredService<ControlMessenger>().Register(recorder);

            for (var i = 0; i < 3; i++)
                manager.AddItem(typeof(CheckerBoard));
            Assert.Equal(new[] { 0, 1, 2 }, recorder.Sent.OfType<MessageAddItem>().Select(message => message.SelectedIndex));

            recorder.Sent.Clear();
            manager.DeleteItem(manager.ManagerData.Items[2]);
            Assert.Equal(1, Assert.Single(recorder.Sent.OfType<MessageRemoveItem>()).SelectedIndex);

            recorder.Sent.Clear();
            undo.Undo();
            Assert.Equal(2, Assert.Single(recorder.Sent.OfType<MessageAddItem>()).SelectedIndex);

            recorder.Sent.Clear();
            manager.SelectedItemChanged(0);
            Assert.Equal(0, Assert.Single(recorder.Sent.OfType<MessageSelectedItemChanged>()).Index);

            recorder.Sent.Clear();
            manager.DeleteItem(manager.ManagerData.Items[0]);
            Assert.Equal(0, Assert.Single(recorder.Sent.OfType<MessageRemoveItem>()).SelectedIndex);
        }

        private static (Layer Layer, ControlFactory Factory) LayerWithEntities(int selectedIndex)
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var layer = (Layer)factory.Create(typeof(Layer));
            for (var i = 0; i < 3; i++)
                layer.ModelEntityManager.AddItem(typeof(Entity));
            if (selectedIndex < 0)
                layer.ModelEntityManager.RemoveSelectedItem();
            else
                layer.ModelEntityManager.SelectedItemChanged(selectedIndex);
            return (layer, factory);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void TheSelectionOfAManagerSurvivesASaveAndALoad(int selectedIndex)
        {
            var (layer, factory) = LayerWithEntities(selectedIndex);
            var model = (LayerModel)layer.ToModel();
            Assert.Equal(selectedIndex, model.ModelEntityManager.ManagerData.SelectedIndex);

            var loaded = (Layer)factory.Create(ControlCloner.CloneWithNewGuids(model));

            Assert.Equal(selectedIndex, loaded.ModelEntityManager.ManagerData.SelectedIndex);
            Assert.Same(loaded.ModelEntityManager.ManagerData.Items[selectedIndex], loaded.ModelEntityManager.SelectedItem);
        }

        [Fact]
        public void ANoSelectionSurvivesASaveAndALoad()
        {
            var (layer, factory) = LayerWithEntities(-1);
            var model = (LayerModel)layer.ToModel();
            Assert.Equal(-1, model.ModelEntityManager.ManagerData.SelectedIndex);

            var loaded = (Layer)factory.Create(ControlCloner.CloneWithNewGuids(model));

            Assert.Equal(-1, loaded.ModelEntityManager.ManagerData.SelectedIndex);
            Assert.Null(loaded.ModelEntityManager.SelectedItem);
        }
    }
}
