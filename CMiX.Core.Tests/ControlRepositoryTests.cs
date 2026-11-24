using System.Collections;
using CMiX.Core.Colors.Modifiers;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // The index by id, the typed lists, no class in two lists, and the collection managers that hold a control.
    public class ControlRepositoryTests
    {
        [Fact]
        public void GetControl_ReturnsTheRegisteredInstance_AndNullForUnknownIds()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var collectionManager = provider.GetRequiredService<CollectionManager>();

            var first = factory.Create(typeof(CheckerBoard));
            var second = factory.Create(typeof(CheckerBoard));
            repository.AddControl(first, collectionManager);
            repository.AddControl(second, collectionManager);

            Assert.Same(first, repository.GetControl(first.ID));
            Assert.Same(second, repository.GetControl(second.ID));
            Assert.Null(repository.GetControl(Guid.NewGuid()));

            repository.RemoveControl(first, collectionManager);

            Assert.Null(repository.GetControl(first.ID));
            Assert.Same(second, repository.GetControl(second.ID));
            Assert.DoesNotContain(first, repository.Controls);
        }

        [Fact]
        public void AddControl_OfASecondControlWithTheSameId_IgnoresIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var collectionManager = provider.GetRequiredService<CollectionManager>();

            var first = factory.Create(typeof(CheckerBoard));
            var second = factory.Create(typeof(CheckerBoard));
            second.ID = first.ID;

            repository.AddControl(first, collectionManager);
            repository.AddControl(second, collectionManager);

            Assert.Single(repository.Controls);
            Assert.Same(first, repository.GetControl(first.ID));
        }

        [Fact]
        public void ModelEntities_OfAddedEntitiesAndTexts_KeepTheOrderOfTheAdds()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var collectionManager = provider.GetRequiredService<CollectionManager>();

            var entity1 = factory.Create(typeof(Entity));
            var text1 = factory.Create(typeof(TextEntity));
            var entity2 = factory.Create(typeof(Entity));
            var text2 = factory.Create(typeof(TextEntity));
            var entity3 = factory.Create(typeof(Entity));

            repository.AddControl(entity1, collectionManager);
            repository.AddControl(text1, collectionManager);
            repository.AddControl(entity2, collectionManager);
            repository.AddControl(text2, collectionManager);

            Assert.Equal(new[] { entity1, text1, entity2, text2 }, repository.ModelEntities);

            repository.RemoveControl(entity1, collectionManager);
            repository.RemoveControl(text1, collectionManager);

            Assert.Equal(new[] { entity2, text2 }, repository.ModelEntities);

            repository.AddControl(entity3, collectionManager);

            Assert.Equal(new[] { entity2, text2, entity3 }, repository.ModelEntities);
        }

        [Fact]
        public void ModelEntities_AfterControlsIsCleared_IsEmpty()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var collectionManager = provider.GetRequiredService<CollectionManager>();

            var entity = factory.Create(typeof(Entity));
            var text = factory.Create(typeof(TextEntity));
            repository.AddControl(entity, collectionManager);
            repository.AddControl(text, collectionManager);

            Assert.Equal(new[] { entity, text }, repository.ModelEntities);

            repository.Controls.Clear();

            Assert.Empty(repository.ModelEntities);
            Assert.Empty(repository.Entities);
            Assert.Empty(repository.Texts);
        }

        [Fact]
        public void RemoveControl_OfAControl_ReleasesItFromThatCollectionManager()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();
            var manager = provider.GetRequiredService<PrefabManager>();

            manager.AddItem(typeof(CheckerBoard));
            var texture = manager.ManagerData.Items[0];

            Assert.Same(manager.Collection, Assert.Single(repository.GetCollectionManagers(texture)));

            manager.RemoveItem(texture);

            Assert.Empty(repository.GetCollectionManagers(texture));
        }

        [Fact]
        public void Dispose_OfACollectionManager_ReleasesEveryControlItHeld()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();
            var manager = provider.GetRequiredService<PrefabManager>();
            manager.AddItem(typeof(CheckerBoard));
            manager.AddItem(typeof(CheckerBoard));
            var items = manager.ManagerData.Items.ToList();

            manager.Dispose();

            Assert.All(items, item => Assert.Empty(repository.GetCollectionManagers(item)));
            Assert.Equal(0, HeldCollectionManagers.Count(repository));
        }

        [Fact]
        public void RemoveItem_OfLayersRepeatedly_DoesNotGrowTheHeldCollectionManagers()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var compositionManager = provider.GetRequiredService<PrefabManager>();

            compositionManager.AddItem(typeof(Composition));
            var composition = (Composition)compositionManager.SelectedItem;

            AddAndDropALayer(composition, undoManager);
            var baseline = HeldCollectionManagers.Count(repository);

            for (int i = 0; i < 5; i++)
                AddAndDropALayer(composition, undoManager);

            Assert.Equal(baseline, HeldCollectionManagers.Count(repository));
        }

        [Fact]
        public void Clear_OfTheUndoEntryOfARemovedLayer_ReleasesItsNestedManagers()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var compositionManager = provider.GetRequiredService<PrefabManager>();

            compositionManager.AddItem(typeof(Composition));
            var composition = (Composition)compositionManager.SelectedItem;

            var beforeTheLayer = HeldCollectionManagers.Count(repository);
            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;
            layer.ModelEntityManager.AddItem(typeof(Entity));

            Assert.True(HeldCollectionManagers.Count(repository) > beforeTheLayer);

            composition.LayerManager.RemoveItem(layer);

            Assert.True(HeldCollectionManagers.Count(repository) > beforeTheLayer);

            undoManager.Clear();

            Assert.Equal(beforeTheLayer, HeldCollectionManagers.Count(repository));
        }

        [Fact]
        public void Clear_OfTheUndoEntryOfARemovedTexture_TakesItsFiltersWithIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var textureManager = provider.GetRequiredService<PrefabManager>();

            var beforeTheTexture = HeldCollectionManagers.Count(repository);

            textureManager.AddItem(typeof(CheckerBoard));
            var texture = (CheckerBoard)textureManager.SelectedItem;
            texture.TextureModifierManager.AddItem(typeof(Blur));
            var filter = (Blur)texture.TextureModifierManager.SelectedItem;

            Assert.Same(filter, repository.GetControl(filter.ID));
            Assert.True(HeldCollectionManagers.Count(repository) > beforeTheTexture);

            textureManager.RemoveItem(texture);

            Assert.Same(filter, repository.GetControl(filter.ID));

            undoManager.Clear();

            Assert.Null(repository.GetControl(texture.ID));
            Assert.Null(repository.GetControl(filter.ID));
            Assert.Equal(beforeTheTexture, HeldCollectionManagers.Count(repository));
        }

        private static void AddAndDropALayer(Composition composition, UndoManager undoManager)
        {
            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;
            layer.ModelEntityManager.AddItem(typeof(Entity));
            composition.LayerManager.RemoveItem(layer);
            undoManager.Clear();
        }

        [Fact]
        public void Project_WhenCreated_IsInTheRepository()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();

            var project = provider.GetRequiredService<Project>();

            Assert.Same(project, repository.GetControl(project.ID));
            Assert.Contains(project, repository.Controls);
        }

        public static TheoryData<Type, string> Lists => new()
        {
            { typeof(Composition), nameof(ControlRepository.Compositions) },
            { typeof(Layer), nameof(ControlRepository.Layers) },
            { typeof(CheckerBoard), nameof(ControlRepository.Textures) },
            { typeof(Entity), nameof(ControlRepository.Entities) },
            { typeof(Camera), nameof(ControlRepository.Cameras) },
            { typeof(LightEntity), nameof(ControlRepository.Lights) },
            { typeof(TextEntity), nameof(ControlRepository.Texts) },
            { typeof(ColorPaletteModifier), nameof(ControlRepository.ColorPalettes) }
        };

        private static readonly string[] AllListNames =
        {
            nameof(ControlRepository.Compositions), nameof(ControlRepository.Layers), nameof(ControlRepository.Textures),
            nameof(ControlRepository.Entities), nameof(ControlRepository.Cameras), nameof(ControlRepository.Lights),
            nameof(ControlRepository.Texts), nameof(ControlRepository.ColorPalettes)
        };

        private static IReadOnlyList<IControl> ListOf(ControlRepository repository, string name) =>
            ((IEnumerable)typeof(ControlRepository).GetProperty(name)!.GetValue(repository)!).Cast<IControl>().ToList();

        private static (ControlRepository Repository, CollectionManager CollectionManager, List<IControl> Controls) Add(Type type, int count)
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var collectionManager = provider.GetRequiredService<CollectionManager>();

            var controls = new List<IControl>();
            for (var i = 0; i < count; i++)
            {
                var control = factory.Create(type);
                repository.AddControl(control, collectionManager);
                controls.Add(control);
            }
            return (repository, collectionManager, controls);
        }

        [Theory]
        [MemberData(nameof(Lists))]
        public void AControl_IsInControlsAndInItsOwnList_InTheOrderItWasAdded(Type type, string listName)
        {
            var (repository, _, controls) = Add(type, 3);

            Assert.Equal(controls, repository.Controls.Where(control => control is not Project));
            Assert.Equal(controls, ListOf(repository, listName));
        }

        [Theory]
        [MemberData(nameof(Lists))]
        public void AddControl_OfAControl_PutsItInNoOtherList(Type type, string listName)
        {
            var (repository, _, _) = Add(type, 2);

            foreach (var other in AllListNames.Where(name => name != listName))
                Assert.Empty(ListOf(repository, other));
        }

        [Theory]
        [MemberData(nameof(Lists))]
        public void RemoveControl_OfAControl_TakesItOutOfControlsAndItsList(Type type, string listName)
        {
            var (repository, collectionManager, controls) = Add(type, 3);

            repository.RemoveControl(controls[1], collectionManager);

            Assert.Equal(new[] { controls[0], controls[2] }, repository.Controls.Where(control => control is not Project));
            Assert.Equal(new[] { controls[0], controls[2] }, ListOf(repository, listName));
        }

        private static readonly Type[] ListTypes =
        {
            typeof(Composition), typeof(Layer), typeof(ITextureSource), typeof(Entity),
            typeof(Camera), typeof(LightEntity), typeof(TextEntity), typeof(ColorPaletteModifier)
        };

        [Fact]
        public void TypedLists_OfEveryControlClass_HoldItInOneListAtMost()
        {
            var controlClasses = typeof(IControl).Assembly.GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && typeof(IControl).IsAssignableFrom(type))
                .ToList();

            var offenders = controlClasses
                .Select(type => (Type: type, Lists: ListTypes.Where(list => list.IsAssignableFrom(type)).Select(list => list.Name).ToList()))
                .Where(entry => entry.Lists.Count > 1)
                .Select(entry => $"{entry.Type.Name} matches {string.Join(", ", entry.Lists)}")
                .ToList();

            Assert.True(controlClasses.Count > 50, $"The scan found only {controlClasses.Count} control classes. The scan is broken.");
            Assert.True(offenders.Count == 0, string.Join(Environment.NewLine, offenders));
        }
    }
}
