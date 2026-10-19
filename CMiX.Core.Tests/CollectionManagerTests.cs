using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Sources;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Covers the guards added in commit 59a147eb: MoveItem used to trust their
    // arguments and would throw or corrupt state on stale or absent controls.
    public class CollectionManagerTests
    {
        private static CollectionManager CreateManagerWithItems(IServiceProvider provider, int count, out List<TouchBlob> items)
        {
            var manager = provider.GetRequiredService<CollectionManager>();
            items = new List<TouchBlob>();
            for (int i = 0; i < count; i++)
            {
                var prefab = provider.GetRequiredService<ControlFactory>().Create(typeof(TouchBlob));
                manager.InsertItem(prefab, manager.ManagerData.Items.Count);
                items.Add((TouchBlob)prefab);
            }
            return manager;
        }

        [Fact]
        public void MoveItem_OutOfRangeOldIndex_IsNoOp()
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = CreateManagerWithItems(provider, 2, out var items);

            var exception = Record.Exception(() => manager.MoveItem(5, 0));

            Assert.Null(exception);
            Assert.Equal(items, manager.ManagerData.Items);
        }

        [Fact]
        public void MoveItem_OutOfRangeNewIndex_IsNoOp()
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = CreateManagerWithItems(provider, 2, out var items);

            var exception = Record.Exception(() => manager.MoveItem(0, 9));

            Assert.Null(exception);
            Assert.Equal(items, manager.ManagerData.Items);
        }

        [Fact]
        public void MoveItem_NegativeIndices_IsNoOp()
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = CreateManagerWithItems(provider, 2, out var items);

            var exception = Record.Exception(() => manager.MoveItem(-1, -1));

            Assert.Null(exception);
            Assert.Equal(items, manager.ManagerData.Items);
        }

        [Fact]
        public void DeleteItem_AbsentControl_ReturnsNegativeOne()
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = CreateManagerWithItems(provider, 1, out _);
            var otherManager = provider.GetRequiredService<CollectionManager>();
            var foreignControl = provider.GetRequiredService<ControlFactory>().Create(typeof(TouchBlob));
            otherManager.InsertItem(foreignControl, 0);

            var index = manager.DeleteItem(foreignControl);

            Assert.Equal(-1, index);
            Assert.Single(manager.ManagerData.Items);
        }

        [Fact]
        public void DeleteItem_NullControl_ReturnsNegativeOne()
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = CreateManagerWithItems(provider, 1, out _);

            var index = manager.DeleteItem(null);

            Assert.Equal(-1, index);
        }

        [Fact]
        public void InsertItem_FromModel_PutsTheItemAtTheIndexAndSelectsIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = CreateManagerWithItems(provider, 3, out var items);
            var model = (TouchBlobModel)items[0].ToModel();

            manager.InsertItem(model with { ID = Guid.NewGuid() }, 1);

            Assert.Equal(4, manager.ManagerData.Items.Count);
            Assert.Same(items[0], manager.ManagerData.Items[0]);
            Assert.NotSame(items[1], manager.ManagerData.Items[1]);
            Assert.Same(items[1], manager.ManagerData.Items[2]);
            Assert.Equal(1, manager.ManagerData.SelectedIndex);
        }

        [Fact]
        public void InsertItem_FromModel_WithIndexOutOfRange_AddsAtTheEnd()
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = CreateManagerWithItems(provider, 2, out var items);
            var model = (TouchBlobModel)items[0].ToModel();

            manager.InsertItem(model with { ID = Guid.NewGuid() }, 9);

            Assert.Equal(3, manager.ManagerData.Items.Count);
            Assert.Equal(2, manager.ManagerData.SelectedIndex);
        }
    }
}
