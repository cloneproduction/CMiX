using System;
using System.Linq;
using CMiX.Core;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Studio.Avalonia.Themes;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.UI.Themes;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // Each kind of group gives its header, its icon, its category and the types that its add menu offers.
    public class TreeGroupTests
    {
        // A layer has the groups of a layer, the filters and the modifiers. An entity has textures.
        private static TreeGroup GroupOfKind(Type kind)
        {
            var factory = TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>();
            var owners = new IControl[] { factory.Create(typeof(Layer)), factory.Create(typeof(Entity)) };
            return owners.SelectMany(TreeGroup.GroupsOf).First(group => group.GetType() == kind);
        }

        [Theory]
        [InlineData(typeof(EntitiesGroup))]
        [InlineData(typeof(CamerasGroup))]
        [InlineData(typeof(LightsGroup))]
        [InlineData(typeof(TexturesGroup))]
        [InlineData(typeof(FiltersGroup))]
        [InlineData(typeof(ModifiersGroup))]
        public void Members_EveryKind_GiveAHeaderAKnownIconACategoryAndTypesThatTheOwnerCanHold(Type kind)
        {
            StudioIcons.Register();
            var group = GroupOfKind(kind);

            Assert.False(string.IsNullOrWhiteSpace(group.Header));
            Assert.True(Icons.Kinds.ContainsKey(group.IconKey), group.IconKey);
            Assert.True(Enum.IsDefined(group.Category));
            Assert.NotEmpty(group.AddableTypes);
            Assert.All(group.AddableTypes, type => Assert.True(ListRules.CanHold(group.OwnerType, type), type.Name));
        }

        [Fact]
        public void For_AListOfTheOwner_GivesTheGroupOfThatList()
        {
            var layer = (Layer)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(Layer));

            Assert.IsType<CamerasGroup>(TreeGroup.For(layer, layer.CameraManager));
            Assert.IsType<FiltersGroup>(TreeGroup.For(layer, layer.TextureModifierManager));
            Assert.Null(TreeGroup.For(layer, null));
        }
    }
}
