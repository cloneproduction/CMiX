using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Sources;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ControlRepositoryTests
    {
        [Fact]
        public void GetControl_ReturnsTheRegisteredInstance_AndNullForUnknownIds()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var referencer = Guid.NewGuid();

            var first = factory.Create(typeof(CheckerBoard));
            var second = factory.Create(typeof(CheckerBoard));
            repository.AddControl(first, referencer);
            repository.AddControl(second, referencer);

            Assert.Same(first, repository.GetControl(first.ID));
            Assert.Same(second, repository.GetControl(second.ID));
            Assert.Null(repository.GetControl(Guid.NewGuid()));

            repository.RemoveControl(first, referencer);

            Assert.Null(repository.GetControl(first.ID));
            Assert.Same(second, repository.GetControl(second.ID));
            Assert.DoesNotContain(first, repository.Controls);
        }

        [Fact]
        public void AddControl_IgnoresASecondControlWithAnAlreadyRegisteredId()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var referencer = Guid.NewGuid();

            var first = factory.Create(typeof(CheckerBoard));
            var second = factory.Create(typeof(CheckerBoard));
            second.ID = first.ID;

            repository.AddControl(first, referencer);
            repository.AddControl(second, referencer);

            Assert.Single(repository.Controls);
            Assert.Same(first, repository.GetControl(first.ID));
        }

        // The factory caches the names it has to stay unique against, so a rename made after a
        // control was created has to invalidate that cache or the next control gets a duplicate.
        [Fact]
        public void NameControl_StaysUnique_AfterAControlWasRenamed()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var referencer = Guid.NewGuid();

            var first = (IPrefab)factory.Create(typeof(CheckerBoard));
            repository.AddControl((IControl)first, referencer);
            var second = (IPrefab)factory.Create(typeof(CheckerBoard));
            repository.AddControl((IControl)second, referencer);

            Assert.Equal("Checker Board", first.PrefabService.Name.Value);
            Assert.Equal("Checker Board.001", second.PrefabService.Name.Value);

            second.PrefabService.Name.Value = "Checker Board.002";

            var third = (IPrefab)factory.Create(typeof(CheckerBoard));
            repository.AddControl((IControl)third, referencer);

            Assert.Equal("Checker Board.001", third.PrefabService.Name.Value);
        }

        // EntitiesAndTexts merges two independent source collections into the single list the layer
        // entity slot swap popup lists from, keeping entities ahead of texts. Add and remove both
        // kinds interleaved and check the merge stays consistent rather than only checking a single
        // snapshot.
        [Fact]
        public void EntitiesAndTexts_KeepsEntitiesAheadOfTextsAsBothAreAddedAndRemovedInterleaved()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var referencer = Guid.NewGuid();

            var entity1 = factory.Create(typeof(Entity));
            var text1 = factory.Create(typeof(TextEntity));
            var entity2 = factory.Create(typeof(Entity));
            var text2 = factory.Create(typeof(TextEntity));
            var entity3 = factory.Create(typeof(Entity));

            repository.AddControl(entity1, referencer);
            repository.AddControl(text1, referencer);
            repository.AddControl(entity2, referencer);
            repository.AddControl(text2, referencer);

            Assert.Equal(new[] { entity1, entity2, text1, text2 }, repository.EntitiesAndTexts);

            repository.RemoveControl(entity1, referencer);
            repository.RemoveControl(text1, referencer);

            Assert.Equal(new[] { entity2, text2 }, repository.EntitiesAndTexts);

            repository.AddControl(entity3, referencer);

            // A newly added entity keeps inserting ahead of the texts block, not at the tail.
            Assert.Equal(new[] { entity2, entity3, text2 }, repository.EntitiesAndTexts);
        }

        // Nothing in production code clears Entities or Texts directly today; every removal goes
        // through RemoveControl one item at a time. The Reset branch of the merge exists for a bulk
        // clear anyway, so it needs its own coverage rather than riding along on the Remove branch.
        [Fact]
        public void EntitiesAndTexts_ResyncsWhenASourceCollectionRaisesAReset()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var referencer = Guid.NewGuid();

            var entity = factory.Create(typeof(Entity));
            var text = factory.Create(typeof(TextEntity));
            repository.AddControl(entity, referencer);
            repository.AddControl(text, referencer);

            Assert.Equal(new[] { entity, text }, repository.EntitiesAndTexts);

            repository.Entities.Clear();

            Assert.Equal(new[] { text }, repository.EntitiesAndTexts);

            repository.Texts.Clear();

            Assert.Empty(repository.EntitiesAndTexts);
        }
    }
}
