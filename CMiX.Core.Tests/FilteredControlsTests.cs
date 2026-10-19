using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CMiX.Core.Prefabs;
using Xunit;

namespace CMiX.Core.Tests
{
    public class FilteredControlsTests
    {
        private interface IPet : IControl { }

        private abstract class Animal : IControl
        {
            public Guid ID { get; set; } = Guid.NewGuid();
            public IControlModel ToModel() => throw new NotSupportedException();
            public void FromModel(IControlModel model) => throw new NotSupportedException();
        }

        private sealed class Cat : Animal, IPet
        {
            public int Lives { get; init; } = 9;
        }

        private sealed class Dog : Animal, IPet { }

        private sealed class Fish : Animal { }

        private static ObservableCollection<IControl> Source(params IControl[] items) => new(items);

        [Fact]
        public void AtTheStart_ItHoldsTheMatchingControlsOfTheSource_InOrder()
        {
            var cat1 = new Cat();
            var cat2 = new Cat();
            var source = Source(cat1, new Dog(), cat2);

            var cats = new FilteredControls<Cat>(source);

            Assert.Equal(new[] { cat1, cat2 }, cats);
        }

        [Fact]
        public void AMatchingControlAddedToTheSource_IsAddedAtTheEndWithOneAddEvent()
        {
            var source = Source(new Cat());
            var cats = new FilteredControls<Cat>(source);
            var events = new List<NotifyCollectionChangedAction>();
            ((INotifyCollectionChanged)cats).CollectionChanged += (_, e) => events.Add(e.Action);
            var cat = new Cat();

            source.Add(cat);

            Assert.Same(cat, cats[^1]);
            Assert.Equal(new[] { NotifyCollectionChangedAction.Add }, events);
        }

        [Fact]
        public void AControlOfAnotherTypeAddedToTheSource_RaisesNoEvent()
        {
            var source = Source(new Cat());
            var cats = new FilteredControls<Cat>(source);
            var events = 0;
            ((INotifyCollectionChanged)cats).CollectionChanged += (_, _) => events++;

            source.Add(new Dog());

            Assert.Single(cats);
            Assert.Equal(0, events);
        }

        [Fact]
        public void AControlRemovedFromTheSource_LeavesTheList()
        {
            var cat1 = new Cat();
            var cat2 = new Cat();
            var source = Source(cat1, new Dog(), cat2);
            var cats = new FilteredControls<Cat>(source);

            source.Remove(cat1);

            Assert.Equal(new[] { cat2 }, cats);
        }

        [Fact]
        public void AControlOfAnotherTypeRemovedFromTheSource_RaisesNoEvent()
        {
            var dog = new Dog();
            var source = Source(new Cat(), dog);
            var cats = new FilteredControls<Cat>(source);
            var events = 0;
            ((INotifyCollectionChanged)cats).CollectionChanged += (_, _) => events++;

            source.Remove(dog);

            Assert.Equal(0, events);
        }

        [Fact]
        public void ClearingTheSource_EmptiesTheList()
        {
            var source = Source(new Cat(), new Cat());
            var cats = new FilteredControls<Cat>(source);

            source.Clear();

            Assert.Empty(cats);
        }

        [Fact]
        public void AControlInsertedInTheMiddleOfTheSource_KeepsTheOrderOfTheSource()
        {
            var cat1 = new Cat();
            var cat2 = new Cat();
            var inserted = new Cat();
            var source = Source(cat1, new Dog(), cat2);
            var cats = new FilteredControls<Cat>(source);

            source.Insert(1, inserted);

            Assert.Equal(new[] { cat1, inserted, cat2 }, cats);
        }

        [Fact]
        public void MovingAControlInTheSource_KeepsTheOrderOfTheSource()
        {
            var cat1 = new Cat();
            var cat2 = new Cat();
            var source = Source(cat1, cat2, new Dog());
            var cats = new FilteredControls<Cat>(source);

            source.Move(0, 2);

            Assert.Equal(new[] { cat2, cat1 }, cats);
        }

        [Fact]
        public void ReplacingAControlInTheSource_UpdatesTheList()
        {
            var cat1 = new Cat();
            var replacement = new Cat();
            var source = Source(cat1, new Cat());
            var cats = new FilteredControls<Cat>(source);

            source[0] = replacement;

            Assert.DoesNotContain(cat1, cats);
            Assert.Same(replacement, cats[0]);
        }

        [Fact]
        public void TheRule_KeepsOutControlsThatFailItAtTheStartAndWhenAdded()
        {
            var rested = new Cat { Lives = 9 };
            var tired = new Cat { Lives = 1 };
            var source = Source(rested, tired);
            var cats = new FilteredControls<Cat>(source, cat => cat.Lives > 3);

            source.Add(new Cat { Lives = 2 });
            var lucky = new Cat { Lives = 7 };
            source.Add(lucky);

            Assert.Equal(new[] { rested, lucky }, cats);
        }

        [Fact]
        public void TheRule_AppliesAgainWhenTheListIsRebuilt()
        {
            var rested = new Cat { Lives = 9 };
            var source = Source(rested, new Cat { Lives = 1 });
            var cats = new FilteredControls<Cat>(source, cat => cat.Lives > 3);

            source.Move(0, 1);

            Assert.Equal(new[] { rested }, cats);
        }

        [Fact]
        public void AnInterface_CollectsEveryClassThatImplementsIt()
        {
            var cat = new Cat();
            var dog = new Dog();
            var source = Source(cat, new Fish(), dog);

            var pets = new FilteredControls<IPet>(source);

            Assert.Equal(new IPet[] { cat, dog }, pets);
        }

        [Fact]
        public void TheList_CannotBeChangedFromOutside()
        {
            var cats = new FilteredControls<Cat>(Source(new Cat()));

            Assert.Throws<NotSupportedException>(() => ((IList<Cat>)cats).Add(new Cat()));
        }
    }
}
