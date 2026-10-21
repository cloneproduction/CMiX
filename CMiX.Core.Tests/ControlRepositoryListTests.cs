using System.Collections;
using CMiX.Core.Colors.Modifiers;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Texturing.Sources;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ControlRepositoryListTests
    {
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

            Assert.Equal(controls, repository.Controls);
            Assert.Equal(controls, ListOf(repository, listName));
        }

        [Theory]
        [MemberData(nameof(Lists))]
        public void AControl_IsInNoOtherList(Type type, string listName)
        {
            var (repository, _, _) = Add(type, 2);

            foreach (var other in AllListNames.Where(name => name != listName))
                Assert.Empty(ListOf(repository, other));
        }

        [Theory]
        [MemberData(nameof(Lists))]
        public void ARemovedControl_LeavesControlsAndItsList(Type type, string listName)
        {
            var (repository, collectionManager, controls) = Add(type, 3);

            repository.RemoveControl(controls[1], collectionManager);

            Assert.Equal(new[] { controls[0], controls[2] }, repository.Controls);
            Assert.Equal(new[] { controls[0], controls[2] }, ListOf(repository, listName));
        }
    }
}
