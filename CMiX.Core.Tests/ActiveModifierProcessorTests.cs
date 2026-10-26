using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Processors;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ActiveModifierProcessorTests
    {
        private static (ActiveModifierProcessor Processor, Entity Entity, List<IControl> Modifiers) Create(int count)
        {
            var provider = TestServiceProviderFactory.Create();
            var processor = provider.GetRequiredService<ActiveModifierProcessor>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var collectionManager = provider.GetRequiredService<CollectionManager>();
            var entity = (Entity)provider.GetRequiredService<ControlFactory>().Create(typeof(Entity));
            repository.AddControl(entity, collectionManager);
            for (var i = 0; i < count; i++)
                entity.ModifierManager.AddItem(typeof(AlphaModifier));

            return (processor, entity, entity.ModifierManager.ManagerData.Items.ToList());
        }

        [Fact]
        public void GetActiveIds_ATargetWithModifiers_ListsThemInTheOrderOfTheList()
        {
            var (processor, entity, modifiers) = Create(3);

            Assert.Equal(modifiers.Select(modifier => modifier.ID), processor.GetActiveIds(entity.ID));
        }

        [Fact]
        public void GetActiveIds_AHiddenModifier_LeavesTheListAndComesBack()
        {
            var (processor, entity, modifiers) = Create(3);
            var hidden = (IPrefab)modifiers[1];

            hidden.PrefabService.Visibility.Value = true;
            Assert.Equal(new[] { modifiers[0].ID, modifiers[2].ID }, processor.GetActiveIds(entity.ID));

            hidden.PrefabService.Visibility.Value = false;
            Assert.Equal(modifiers.Select(modifier => modifier.ID), processor.GetActiveIds(entity.ID));
        }

        [Fact]
        public void GetActiveIds_AModifierAddedOrRemoved_FollowsTheList()
        {
            var (processor, entity, modifiers) = Create(2);
            Assert.Equal(2, processor.GetActiveIds(entity.ID).Count);

            entity.ModifierManager.AddItem(typeof(AlphaModifier));
            Assert.Equal(3, processor.GetActiveIds(entity.ID).Count);

            entity.ModifierManager.RemoveItem(modifiers[0]);
            Assert.DoesNotContain(modifiers[0].ID, processor.GetActiveIds(entity.ID));
        }

        [Fact]
        public void GetActiveIds_AnUnknownTarget_IsEmpty()
        {
            var (processor, _, _) = Create(0);

            Assert.Empty(processor.GetActiveIds(Guid.NewGuid()));
        }
    }
}
