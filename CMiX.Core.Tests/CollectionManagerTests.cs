using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Sources;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Covers the guards added in commit 59a147eb: MoveItem and ResetItem used to trust their
    // arguments and would throw or corrupt state on stale or absent controls.
    public class CollectionManagerTests
    {
        private static CollectionManager CreateManagerWithItems(IServiceProvider provider, int count, out List<TouchBlob> items)
        {
            var manager = provider.GetRequiredService<CollectionManager>();
            items = new List<TouchBlob>();
            for (int i = 0; i < count; i++)
            {
                var (prefab, _) = manager.AddItem(typeof(TouchBlob));
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
        public void ResetItem_NullControl_IsNoOp()
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = CreateManagerWithItems(provider, 1, out var items);

            var exception = Record.Exception(() => manager.ResetItem(null));

            Assert.Null(exception);
            Assert.Same(items[0], manager.ManagerData.Items[0]);
        }

        [Fact]
        public void ResetItem_ControlNotInCollection_IsNoOp()
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = CreateManagerWithItems(provider, 1, out var items);
            var otherManager = provider.GetRequiredService<CollectionManager>();
            var (foreignControl, _) = otherManager.AddItem(typeof(TouchBlob));

            var exception = Record.Exception(() => manager.ResetItem(foreignControl));

            Assert.Null(exception);
            Assert.Same(items[0], manager.ManagerData.Items[0]);
        }

        [Fact]
        public void DeleteItem_AbsentControl_ReturnsNullAndNegativeOne()
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = CreateManagerWithItems(provider, 1, out _);
            var otherManager = provider.GetRequiredService<CollectionManager>();
            var (foreignControl, _) = otherManager.AddItem(typeof(TouchBlob));

            var (removed, index) = manager.DeleteItem(foreignControl);

            Assert.Null(removed);
            Assert.Equal(-1, index);
            Assert.Single(manager.ManagerData.Items);
        }

        [Fact]
        public void DeleteItem_NullControl_ReturnsNullAndNegativeOne()
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = CreateManagerWithItems(provider, 1, out _);

            var (removed, index) = manager.DeleteItem(null);

            Assert.Null(removed);
            Assert.Equal(-1, index);
        }
    }
}
