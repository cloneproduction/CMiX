using CMiX.Core.Compositing;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Prefabs.Messages;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // One entity can sit in the managers of two layers. Every peer must hold it as one object,
    // and the repository must drop it when the last layer removes it.
    public class SharedControlTests
    {
        private static (Composition composition, Layer first, Layer second, Entity entity) CreateSharedEntity(IServiceProvider provider)
        {
            var compositionManager = provider.GetRequiredService<PrefabManager>();
            compositionManager.AddItem(typeof(Composition));
            var composition = (Composition)compositionManager.SelectedItem;

            composition.LayerManager.AddItem(typeof(Layer));
            var first = (Layer)composition.LayerManager.SelectedItem;
            composition.LayerManager.AddItem(typeof(Layer));
            var second = (Layer)composition.LayerManager.SelectedItem;

            first.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (Entity)first.ModelEntityManager.SelectedItem;
            second.ModelEntityManager.AddExistingItem(entity);
            return (composition, first, second, entity);
        }

        [Fact]
        public void SendingPeer_EntityRemovedFromBothLayers_LeavesTheRepository()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();
            var (_, first, second, entity) = CreateSharedEntity(provider);

            first.ModelEntityManager.DeleteItem(entity);
            Assert.Same(entity, repository.GetControl(entity.ID));

            second.ModelEntityManager.DeleteItem(entity);
            Assert.Null(repository.GetControl(entity.ID));
            Assert.Empty(repository.Entities);
        }

        // Builds two receiving managers that follow the two sending managers by ID.
        private static (PrefabManager first, PrefabManager second, ControlRepository repository, PrefabManager senderFirst, PrefabManager senderSecond, Entity entity, MessageFactory factory) CreateReceivingPair()
        {
            var sender = TestServiceProviderFactory.Create();
            var senderFirst = sender.GetRequiredService<PrefabManager>();
            var senderSecond = sender.GetRequiredService<PrefabManager>();
            senderFirst.AddItem(typeof(Entity));
            var entity = (Entity)senderFirst.SelectedItem;
            senderSecond.AddExistingItem(entity);

            var receiver = TestServiceProviderFactory.Create();
            var first = receiver.GetRequiredService<PrefabManager>();
            var second = receiver.GetRequiredService<PrefabManager>();
            first.ManagerData.ID = senderFirst.ManagerData.ID;
            second.ManagerData.ID = senderSecond.ManagerData.ID;

            return (first, second, receiver.GetRequiredService<ControlRepository>(), senderFirst, senderSecond, entity,
                sender.GetRequiredService<MessageFactory>());
        }

        [Fact]
        public void ReceivingPeer_SecondAddOfTheSameEntity_ReusesTheFirstInstance()
        {
            var (first, second, repository, senderFirst, senderSecond, entity, factory) = CreateReceivingPair();

            first.Receive(factory.CreateMessage<MessageAddItem>(senderFirst.ManagerData.ID, entity, 0));
            second.Receive(factory.CreateMessage<MessageAddItem>(senderSecond.ManagerData.ID, entity, 0));

            Assert.Same(first.ManagerData.Items[0], second.ManagerData.Items[0]);
            Assert.Single(repository.Entities);
        }

        [Fact]
        public void ReceivingPeer_EntityRemovedFromBothManagers_LeavesTheRepository()
        {
            var (first, second, repository, senderFirst, senderSecond, entity, factory) = CreateReceivingPair();
            first.Receive(factory.CreateMessage<MessageAddItem>(senderFirst.ManagerData.ID, entity, 0));
            second.Receive(factory.CreateMessage<MessageAddItem>(senderSecond.ManagerData.ID, entity, 0));

            first.Receive(factory.CreateMessage<MessageRemoveItem>(senderFirst.ManagerData.ID, entity, -1));
            Assert.NotNull(repository.GetControl(entity.ID));
            Assert.Single(second.ManagerData.Items);

            second.Receive(factory.CreateMessage<MessageRemoveItem>(senderSecond.ManagerData.ID, entity, -1));
            Assert.Null(repository.GetControl(entity.ID));
            Assert.Empty(repository.Entities);
            Assert.Empty(repository.Controls);
        }

        [Fact]
        public void LoadedModel_WithAnEntityInTwoLayers_BuildsOneEntity()
        {
            var sender = TestServiceProviderFactory.Create();
            var (senderComposition, _, _, _) = CreateSharedEntity(sender);
            var compositionModel = senderComposition.ToModel();

            var receiver = TestServiceProviderFactory.Create();
            var repository = receiver.GetRequiredService<ControlRepository>();
            var project = receiver.GetRequiredService<Project>();
            project.CompositionManager.AddItem(compositionModel);

            var composition = (Composition)project.CompositionManager.SelectedItem;
            var layers = composition.LayerManager.ManagerData.Items.Cast<Layer>().ToList();
            Assert.Equal(2, layers.Count);
            Assert.Same(layers[0].ModelEntityManager.ManagerData.Items[0], layers[1].ModelEntityManager.ManagerData.Items[0]);
            Assert.Single(repository.Entities);

            var entity = layers[0].ModelEntityManager.ManagerData.Items[0];
            layers[0].ModelEntityManager.DeleteItem(entity);
            layers[1].ModelEntityManager.DeleteItem(entity);
            Assert.Empty(repository.Entities);
        }
    }
}
