using CMiX.Core.Prefabs.Managers;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ManagerDataTests
    {
        private sealed class Item : IControl
        {
            public Guid ID { get; set; } = Guid.NewGuid();
            public IControlModel ToModel() => throw new NotSupportedException();
            public void FromModel(IControlModel model) => throw new NotSupportedException();
        }

        private static (ManagerData Data, Item[] Items) WithItems(int count)
        {
            var data = new ManagerData();
            var items = Enumerable.Range(0, count).Select(_ => new Item()).ToArray();
            foreach (var item in items)
                data.Insert(data.Items.Count, item);
            return (data, items);
        }

        [Fact]
        public void ANewList_HasNoSelection()
        {
            var data = new ManagerData();

            Assert.Null(data.SelectedItem);
            Assert.Equal(-1, data.SelectedIndex);
        }

        [Fact]
        public void Insert_PutsTheItemAtTheIndexAndSelectsIt()
        {
            var (data, items) = WithItems(2);
            var inserted = new Item();

            data.Insert(1, inserted);

            Assert.Equal(new IControl[] { items[0], inserted, items[1] }, data.Items);
            Assert.Same(inserted, data.SelectedItem);
            Assert.Equal(1, data.SelectedIndex);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(2, 1)]
        public void Remove_SelectsTheItemBeforeTheRemovedOne_OrTheNewFirstItem(int removed, int expectedIndex)
        {
            var (data, items) = WithItems(3);

            var index = data.Remove(items[removed]);

            Assert.Equal(removed, index);
            Assert.Equal(expectedIndex, data.SelectedIndex);
            Assert.Same(data.Items[expectedIndex], data.SelectedItem);
        }

        [Fact]
        public void Remove_OfTheOnlyItem_LeavesNothingSelected()
        {
            var (data, items) = WithItems(1);

            data.Remove(items[0]);

            Assert.Null(data.SelectedItem);
            Assert.Equal(-1, data.SelectedIndex);
        }

        [Fact]
        public void Remove_OfAnItemThatIsNotInTheList_ChangesNothing()
        {
            var (data, items) = WithItems(2);

            var index = data.Remove(new Item());

            Assert.Equal(-1, index);
            Assert.Equal(2, data.Items.Count);
            Assert.Same(items[1], data.SelectedItem);
        }

        [Fact]
        public void Move_KeepsTheSelectionOnTheSameItem_AndTheIndexFollowsIt()
        {
            var (data, items) = WithItems(3);
            data.Select(items[0]);

            Assert.True(data.Move(0, 2));

            Assert.Same(items[0], data.SelectedItem);
            Assert.Equal(2, data.SelectedIndex);
        }

        [Fact]
        public void Move_OfAnotherItem_ShiftsTheIndexOfTheSelection()
        {
            var (data, items) = WithItems(3);
            data.Select(items[1]);

            data.Move(0, 2);

            Assert.Same(items[1], data.SelectedItem);
            Assert.Equal(0, data.SelectedIndex);
        }

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(3, 0)]
        [InlineData(0, -1)]
        [InlineData(0, 3)]
        public void Move_WithAnIndexOutsideTheList_IsRefused(int oldIndex, int newIndex)
        {
            var (data, items) = WithItems(3);

            Assert.False(data.Move(oldIndex, newIndex));

            Assert.Equal(items, data.Items);
        }

        [Fact]
        public void Select_OfAnItemOfTheList_SelectsIt()
        {
            var (data, items) = WithItems(3);

            data.Select(items[0]);

            Assert.Same(items[0], data.SelectedItem);
            Assert.Equal(0, data.SelectedIndex);
        }

        [Fact]
        public void Select_OfAnItemThatIsNotInTheList_LeavesNothingSelected()
        {
            var (data, _) = WithItems(3);

            data.Select(new Item());

            Assert.Null(data.SelectedItem);
            Assert.Equal(-1, data.SelectedIndex);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        public void SelectAt_SelectsTheItemAtTheIndex(int index)
        {
            var (data, items) = WithItems(3);

            data.SelectAt(index);

            Assert.Same(items[index], data.SelectedItem);
            Assert.Equal(index, data.SelectedIndex);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(3)]
        public void SelectAt_WithAnIndexOutsideTheList_LeavesNothingSelected(int index)
        {
            var (data, _) = WithItems(3);

            data.SelectAt(index);

            Assert.Null(data.SelectedItem);
            Assert.Equal(-1, data.SelectedIndex);
        }

        [Fact]
        public void ClearSelection_LeavesTheItemsInTheList()
        {
            var (data, _) = WithItems(3);

            data.ClearSelection();

            Assert.Null(data.SelectedItem);
            Assert.Equal(-1, data.SelectedIndex);
            Assert.Equal(3, data.Items.Count);
        }

        [Fact]
        public void ASelectionChange_RaisesTheItemBeforeTheIndex()
        {
            var (data, items) = WithItems(3);
            var raised = new List<string>();
            data.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

            data.Select(items[0]);

            Assert.Equal(new[] { nameof(ManagerData.SelectedItem), nameof(ManagerData.SelectedIndex) }, raised);
        }

        [Fact]
        public void SelectingTheSelectedItemAgain_RaisesNothing()
        {
            var (data, items) = WithItems(3);
            var raised = new List<string>();
            data.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

            data.Select(items[2]);

            Assert.Empty(raised);
        }

        [Fact]
        public void AMoveOfAnotherItem_RaisesTheIndexOfTheSelection()
        {
            var (data, items) = WithItems(3);
            data.Select(items[1]);
            var raised = new List<string>();
            data.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

            data.Move(0, 2);

            Assert.Contains(nameof(ManagerData.SelectedIndex), raised);
        }
    }
}
