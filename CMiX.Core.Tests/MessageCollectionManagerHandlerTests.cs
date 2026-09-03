using CMiX.Core.Compositing;
using CMiX.Core.Networking.Messages;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // A late engine replays stream entries after it applies a snapshot. An add message for an
    // item the snapshot already holds must not add the item a second time.
    public class MessageCollectionManagerHandlerTests
    {
        private static IControlModel CompositionModel()
        {
            var project = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));
            return project.CompositionManager.ManagerData.Items[0].ToModel();
        }

        [Fact]
        public void MessageAddItem_ForItemAlreadyInCollection_UpdatesSelectedIndexOnly()
        {
            var project = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            var manager = project.CompositionManager;
            var model = CompositionModel();
            var message = new MessageAddItem(manager.ManagerData.ID, model, 0);

            manager.Receive(message);
            manager.Receive(message);

            var item = Assert.Single(manager.ManagerData.Items);
            Assert.Equal(model.ID, item.ID);
            Assert.Equal(message.SelectedIndex, manager.ManagerData.SelectedIndex);
        }

        [Fact]
        public void MessageAddItem_ForDifferentItems_AddsBoth()
        {
            var project = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            var manager = project.CompositionManager;
            var firstModel = CompositionModel();
            var secondModel = CompositionModel();

            manager.Receive(new MessageAddItem(manager.ManagerData.ID, firstModel, 0));
            manager.Receive(new MessageAddItem(manager.ManagerData.ID, secondModel, 1));

            Assert.Equal(2, manager.ManagerData.Items.Count);
        }
    }
}
