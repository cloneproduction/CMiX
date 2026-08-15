using CMiX.Core;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Sources;
using CMiX.Studio.Avalonia.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // Covers the drop index math of ManagerReorderService exhaustively over a five item list,
    // which is the D6 blind spot of the post parity audit. The audit reported finding A10, an
    // off by one when dragging upward onto the second to last slot. It does not reproduce:
    // ListBoxReorderDragDrop only ever calls Dropped with a source index that is a realized
    // container index, so sourceIndex is always at most itemCount - 1, which makes the two
    // itemCount clauses of the condition subsets of the targetIndex >= sourceIndex clause.
    // These tests pin the whole table so a future edit of that condition cannot regress it.
    public class ManagerReorderServiceTests
    {
        private const int ItemCount = 5;

        private static CollectionManager CreateManagerWithItems(out List<IControl> items)
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = provider.GetRequiredService<CollectionManager>();
            items = new List<IControl>();
            for (var i = 0; i < ItemCount; i++)
            {
                var (prefab, _) = manager.AddItem(typeof(TouchBlob));
                items.Add(prefab);
            }
            return manager;
        }

        // The order the list must end up in, expressed as original indices. Insert index i means
        // insert in front of the item that currently sits at i, and ItemCount means append.
        private static List<int> ExpectedOrder(int sourceIndex, int insertIndex)
        {
            var order = Enumerable.Range(0, ItemCount).ToList();
            var moved = order[sourceIndex];
            order.RemoveAt(sourceIndex);
            order.Insert(insertIndex > sourceIndex ? insertIndex - 1 : insertIndex, moved);
            return order;
        }

        private static List<int> ActualOrder(CollectionManager manager, List<IControl> items)
            => manager.ManagerData.Items.Select(control => items.IndexOf(control)).ToList();

        private static string Describe(int sourceIndex, int insertIndex, IEnumerable<int> order)
            => $"source {sourceIndex} insert {insertIndex} to {string.Join(",", order)}";

        // ComputeInsertIndex in ListBoxReorderDragDrop returns 0 to ItemCount inclusive, and the
        // source index always comes from an item container, so it never reaches ItemCount.
        public static IEnumerable<(int sourceIndex, int insertIndex)> AllReachablePairs()
        {
            for (var sourceIndex = 0; sourceIndex < ItemCount; sourceIndex++)
                for (var insertIndex = 0; insertIndex <= ItemCount; insertIndex++)
                    yield return (sourceIndex, insertIndex);
        }

        [Fact]
        public void CanDrop_RejectsOnlyTheTwoNoOpInsertPositions()
        {
            var manager = CreateManagerWithItems(out _);
            var service = new ManagerReorderService(manager, (_, _) => { });

            var expected = new List<string>();
            var actual = new List<string>();
            foreach (var (sourceIndex, insertIndex) in AllReachablePairs())
            {
                var isNoOp = insertIndex == sourceIndex || insertIndex == sourceIndex + 1;
                expected.Add($"source {sourceIndex} insert {insertIndex} canDrop {!isNoOp}");
                actual.Add($"source {sourceIndex} insert {insertIndex} canDrop {service.CanDrop(sourceIndex, insertIndex)}");
            }

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Dropped_ProducesTheInsertBeforeOrdering_ForEveryReachablePair()
        {
            var expected = new List<string>();
            var actual = new List<string>();

            foreach (var (sourceIndex, insertIndex) in AllReachablePairs())
            {
                var manager = CreateManagerWithItems(out var items);
                var service = new ManagerReorderService(manager, (_, _) => { });
                if (!service.CanDrop(sourceIndex, insertIndex))
                    continue;

                service.Dropped(sourceIndex, insertIndex);

                expected.Add(Describe(sourceIndex, insertIndex, ExpectedOrder(sourceIndex, insertIndex)));
                actual.Add(Describe(sourceIndex, insertIndex, ActualOrder(manager, items)));
            }

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Dropped_ReportsTheResultingIndexToTheMoveCallback()
        {
            var expected = new List<string>();
            var actual = new List<string>();

            foreach (var (sourceIndex, insertIndex) in AllReachablePairs())
            {
                var manager = CreateManagerWithItems(out var items);
                var moves = new List<(int oldIndex, int newIndex)>();
                var service = new ManagerReorderService(manager, (oldIndex, newIndex) => moves.Add((oldIndex, newIndex)));
                if (!service.CanDrop(sourceIndex, insertIndex))
                    continue;

                service.Dropped(sourceIndex, insertIndex);

                var landedAt = manager.ManagerData.Items.IndexOf(items[sourceIndex]);
                expected.Add($"source {sourceIndex} insert {insertIndex} move {sourceIndex},{landedAt}");
                actual.Add($"source {sourceIndex} insert {insertIndex} move {moves.Single().oldIndex},{moves.Single().newIndex}");
            }

            Assert.Equal(expected, actual);
        }

        // The exact case finding A10 called out. With five items, dragging the last item onto the
        // slot in front of the fourth one must land it at index 3, not at index 2.
        [Fact]
        public void Dropped_DraggingTheLastItemOntoTheSecondToLastSlot_MovesItOneSlot()
        {
            var manager = CreateManagerWithItems(out var items);
            var moves = new List<(int oldIndex, int newIndex)>();
            var service = new ManagerReorderService(manager, (oldIndex, newIndex) => moves.Add((oldIndex, newIndex)));

            service.Dropped(4, 3);

            Assert.Equal((4, 3), moves.Single());
            Assert.Equal(new List<int> { 0, 1, 2, 4, 3 }, ActualOrder(manager, items));
        }

        // Dropping past the last item appends, which is the only insert index that equals the count.
        [Fact]
        public void Dropped_DraggingTheFirstItemPastTheEnd_MovesItToTheLastSlot()
        {
            var manager = CreateManagerWithItems(out var items);
            var moves = new List<(int oldIndex, int newIndex)>();
            var service = new ManagerReorderService(manager, (oldIndex, newIndex) => moves.Add((oldIndex, newIndex)));

            service.Dropped(0, ItemCount);

            Assert.Equal((0, 4), moves.Single());
            Assert.Equal(new List<int> { 1, 2, 3, 4, 0 }, ActualOrder(manager, items));
        }

        [Fact]
        public void Dropped_ClearsTheDragHandlerFlag()
        {
            var manager = CreateManagerWithItems(out _);
            var service = new ManagerReorderService(manager, (_, _) => { }) { DragHandlerIsPressed = true };

            service.Dropped(0, 2);

            Assert.False(service.DragHandlerIsPressed);
        }
    }
}
