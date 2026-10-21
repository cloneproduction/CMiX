using CMiX.Core.Compositing;
using CMiX.Core.Prefabs.Managers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // The command that the Duplicate buttons run.
    public class DuplicateItemCommandTests
    {
        private static PrefabManager Manager() => TestServiceProviderFactory.Create().GetRequiredService<PrefabManager>();

        [Fact]
        public void DuplicateItemCommand_AddsACopyRightAfterTheSource()
        {
            var manager = Manager();
            manager.AddItem(typeof(Entity));
            manager.AddItem(typeof(Entity));
            var source = manager.ManagerData.Items[0];

            manager.DuplicateItemCommand.Execute(source);

            Assert.Equal(3, manager.ManagerData.Items.Count);
            Assert.NotSame(source, manager.ManagerData.Items[1]);
            Assert.IsType<Entity>(manager.ManagerData.Items[1]);
            Assert.Same(source, manager.ManagerData.Items[0]);
        }

        [Fact]
        public void DuplicateItemCommand_WithoutAnItem_DoesNothing()
        {
            var manager = Manager();
            manager.AddItem(typeof(Entity));

            manager.DuplicateItemCommand.Execute(null);

            Assert.Single(manager.ManagerData.Items);
        }
    }
}
