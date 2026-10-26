using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Processors;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ActiveItemsProcessorTests
    {
        private static (ActiveItemsProcessor Processor, Layer Layer) Create(int entityCount)
        {
            var provider = TestServiceProviderFactory.Create();
            var processor = provider.GetRequiredService<ActiveItemsProcessor>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var collectionManager = provider.GetRequiredService<CollectionManager>();
            var layer = (Layer)provider.GetRequiredService<ControlFactory>().Create(typeof(Layer));
            repository.AddControl(layer, collectionManager);
            for (var i = 0; i < entityCount; i++)
                layer.ModelEntityManager.AddItem(typeof(Entity));

            return (processor, layer);
        }

        private static List<IControl> Items(PrefabManager manager) => manager.ManagerData.Items.ToList();

        [Fact]
        public void GetActiveIds_AList_ListsTheItemsInTheOrderOfTheList()
        {
            var (processor, layer) = Create(3);

            Assert.Equal(Items(layer.ModelEntityManager).Select(item => item.ID), processor.GetActiveIds(layer.ModelEntityManager));
        }

        [Fact]
        public void GetActiveIds_AHiddenItem_LeavesTheListAndComesBack()
        {
            var (processor, layer) = Create(3);
            var items = Items(layer.ModelEntityManager);
            var hidden = (IPrefab)items[1];

            hidden.PrefabService.Visibility.Value = true;
            Assert.Equal(new[] { items[0].ID, items[2].ID }, processor.GetActiveIds(layer.ModelEntityManager));

            hidden.PrefabService.Visibility.Value = false;
            Assert.Equal(items.Select(item => item.ID), processor.GetActiveIds(layer.ModelEntityManager));
        }

        [Fact]
        public void GetActiveIds_AnItemAddedOrRemoved_FollowsTheList()
        {
            var (processor, layer) = Create(2);
            var first = Items(layer.ModelEntityManager)[0];
            Assert.Equal(2, processor.GetActiveIds(layer.ModelEntityManager).Count);

            layer.ModelEntityManager.AddItem(typeof(Entity));
            Assert.Equal(3, processor.GetActiveIds(layer.ModelEntityManager).Count);

            layer.ModelEntityManager.RemoveItem(first);
            Assert.DoesNotContain(first.ID, processor.GetActiveIds(layer.ModelEntityManager));
        }

        [Fact]
        public void GetActiveIds_TwoLists_DoNotTouchEachOther()
        {
            var (processor, layer) = Create(2);

            Assert.Equal(2, processor.GetActiveIds(layer.ModelEntityManager).Count);
            Assert.Empty(processor.GetActiveIds(layer.CameraManager));
        }

        [Fact]
        public void GetActiveIds_ANullManager_IsEmpty()
        {
            var (processor, _) = Create(0);

            Assert.Empty(processor.GetActiveIds(null));
        }
    }
}
