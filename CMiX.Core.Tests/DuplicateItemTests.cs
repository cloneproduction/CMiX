using CMiX.Core.Compositing;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Duplicate puts a deep copy with new ids and a new name right after the source, as one undo step.
    public class DuplicateItemTests
    {
        private static (IServiceProvider provider, Composition composition, List<Layer> layers, PrefabManager compositions) Create(int layerCount = 3)
        {
            var provider = TestServiceProviderFactory.Create();
            var compositions = provider.GetRequiredService<PrefabManager>();
            compositions.AddItem(typeof(Composition));
            var composition = (Composition)compositions.SelectedItem;
            for (var i = 0; i < layerCount; i++)
                composition.LayerManager.AddItem(typeof(Layer));
            return (provider, composition, composition.LayerManager.ManagerData.Items.Cast<Layer>().ToList(), compositions);
        }

        [Fact]
        public void DuplicateItem_InTheMiddle_PutsTheCopyRightAfterTheSourceAndSelectsIt()
        {
            var (_, composition, layers, _) = Create();
            var manager = composition.LayerManager;

            var copy = (Layer)manager.DuplicateItem(layers[0]);

            Assert.Equal(new[] { layers[0], copy, layers[1], layers[2] }, manager.ManagerData.Items);
            Assert.Equal(1, manager.ManagerData.SelectedIndex);
            Assert.Same(copy, manager.SelectedItem);
        }

        [Fact]
        public void DuplicateItem_GivesTheCopyNewIdsAndTheNextFreeName()
        {
            var (_, composition, layers, _) = Create(1);
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
            var (_, composition, layers, _) = Create(1);
            var manager = composition.LayerManager;

            var first = manager.DuplicateItem(layers[0]);
            var second = (Layer)manager.DuplicateItem(first);

            Assert.EndsWith(".002", second.PrefabService.Name.Value);
        }

        [Fact]
        public void DuplicateItem_ForAnItemOfAnotherList_DoesNothing()
        {
            var (_, composition, layers, _) = Create(1);
            var other = Create(1).layers[0];

            Assert.Null(composition.LayerManager.DuplicateItem(other));
            Assert.Single(composition.LayerManager.ManagerData.Items);
        }

        [Fact]
        public void DuplicateItem_UndoRemovesTheCopyAndRedoPutsItBackAtTheSameIndex()
        {
            var (provider, composition, layers, _) = Create();
            var manager = composition.LayerManager;
            var undo = provider.GetRequiredService<UndoManager>();

            var copy = manager.DuplicateItem(layers[0]);
            undo.Undo();
            Assert.Equal(layers, manager.ManagerData.Items);

            undo.Redo();
            Assert.Equal(new[] { layers[0], copy, layers[1], layers[2] }, manager.ManagerData.Items);
        }

        [Fact]
        public void DuplicateItem_TheNameChangeAddsNoUndoStepOfItsOwn()
        {
            var (provider, composition, layers, _) = Create(1);
            var undo = provider.GetRequiredService<UndoManager>();
            composition.LayerManager.DuplicateItem(layers[0]);

            undo.Undo();

            Assert.Single(composition.LayerManager.ManagerData.Items);
            Assert.Equal(layers[0].PrefabService.Name.Value, ((Layer)composition.LayerManager.ManagerData.Items[0]).PrefabService.Name.Value);
        }

        [Fact]
        public void DuplicateItem_SendsOneAddMessageWithTheIndexOfTheCopy()
        {
            var (provider, composition, layers, _) = Create();
            var sender = new RecordingMessageSender();
            provider.GetRequiredService<CMiX.Core.Networking.ControlMessenger>().Register(sender);

            var copy = composition.LayerManager.DuplicateItem(layers[0]);

            var message = Assert.Single(sender.Sent.OfType<MessageAddItem>());
            Assert.Equal(composition.LayerManager.ManagerData.ID, message.ID);
            Assert.Equal(copy.ID, message.Model.ID);
            Assert.Equal(1, message.SelectedIndex);
        }

        [Fact]
        public void DuplicateItem_AnotherPeerShowsTheSameOrder()
        {
            var (provider, composition, layers, _) = Create();
            var peerProject = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            peerProject.CompositionManager.Receive(new MessageAddItem(
                peerProject.CompositionManager.ManagerData.ID, composition.ToModel(), 0));
            var peerLayers = ((Composition)peerProject.CompositionManager.SelectedItem).LayerManager;
            var sender = new RecordingMessageSender();
            provider.GetRequiredService<CMiX.Core.Networking.ControlMessenger>().Register(sender);

            composition.LayerManager.DuplicateItem(layers[0]);
            peerLayers.Receive(sender.Sent.OfType<MessageAddItem>().Single());

            Assert.Equal(
                composition.LayerManager.ManagerData.Items.Select(item => item.ID),
                peerLayers.ManagerData.Items.Select(item => item.ID));
        }

        [Fact]
        public void DuplicateItem_OfAComposition_CopiesItsLayersWithNewIdsAndKeepsTheOutputMapping()
        {
            var (_, composition, layers, compositions) = Create(2);

            var copy = (Composition)compositions.DuplicateItem(composition);

            Assert.Equal(2, copy.LayerManager.ManagerData.Items.Count);
            Assert.Empty(copy.LayerManager.ManagerData.Items.Select(item => item.ID).Intersect(layers.Select(layer => layer.ID)));
            Assert.Equal(composition.SelectedOutputMappingID.Value, copy.SelectedOutputMappingID.Value);
            Assert.Equal(new IControl[] { composition, copy }, compositions.ManagerData.Items);
        }
    }
}
