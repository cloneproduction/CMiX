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
    }
}
