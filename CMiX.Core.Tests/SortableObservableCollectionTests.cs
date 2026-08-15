using System.Collections.Specialized;
using CMiX.Core;
using CMiX.Core.Assets;
using Xunit;

namespace CMiX.Core.Tests
{
    public class SortableObservableCollectionTests
    {
        [Fact]
        public void AddSorted_KeepsTheCollectionSorted_WithOneEventPerAdd()
        {
            var collection = new SortableObservableCollection<string>();
            var events = 0;
            collection.CollectionChanged += (s, e) => events++;

            foreach (var name in new[] { "delta", "alpha", "echo", "bravo", "charlie" })
                collection.AddSorted(name, x => x);

            Assert.Equal(new[] { "alpha", "bravo", "charlie", "delta", "echo" }, collection);
            Assert.Equal(5, events);
        }

        [Fact]
        public void AddSorted_KeepsEqualKeysInInsertionOrder()
        {
            var collection = new SortableObservableCollection<string>();

            collection.AddSorted("b1", x => x[0].ToString());
            collection.AddSorted("a1", x => x[0].ToString());
            collection.AddSorted("b2", x => x[0].ToString());
            collection.AddSorted("b3", x => x[0].ToString());

            Assert.Equal(new[] { "a1", "b1", "b2", "b3" }, collection);
        }

        [Fact]
        public void Sort_ReordersOutOfPlaceItems_AndLeavesASortedCollectionUntouched()
        {
            var collection = new SortableObservableCollection<string>(
                new List<string> { "delta", "alpha", "charlie", "bravo" });

            collection.Sort(x => x);
            Assert.Equal(new[] { "alpha", "bravo", "charlie", "delta" }, collection);

            var events = 0;
            collection.CollectionChanged += (s, e) => events++;
            collection.Sort(x => x);

            Assert.Equal(new[] { "alpha", "bravo", "charlie", "delta" }, collection);
            Assert.Equal(0, events);
        }

        [Fact]
        public void SortDescending_StillOrdersTheWholeCollection()
        {
            var collection = new SortableObservableCollection<string>(
                new List<string> { "bravo", "delta", "alpha" });

            collection.SortDescending(x => x);

            Assert.Equal(new[] { "delta", "bravo", "alpha" }, collection);
        }

        [Fact]
        public void AssetDirectory_KeepsItsAssetsSorted_ThroughAddsAndRenames()
        {
            var directory = new AssetDirectory("root");

            var first = new AssetDirectory("delta");
            var second = new AssetDirectory("alpha");
            var third = new AssetDirectory("charlie");
            directory.AddAsset(first);
            directory.AddAsset(second);
            directory.AddAsset(third);

            Assert.Equal(new[] { "alpha", "charlie", "delta" }, directory.Assets.Select(x => x.Name));

            // Renaming raises the property changed the directory resorts on.
            first.Name = "bravo";

            Assert.Equal(new[] { "alpha", "bravo", "charlie" }, directory.Assets.Select(x => x.Name));
        }
    }
}
