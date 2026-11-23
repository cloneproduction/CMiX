using CMiX.Core.Compositing;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Camera = CMiX.Core.Rendering.Cameras.Camera;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // ProjectTree.GetAddGroups gives the Add panel what it shows. Every node gets the same groups in the same order. An entry that the node
    // cannot take is there, not enabled. What a node can take comes from the interfaces of the node.
    public class ProjectTreeAddGroupsTests
    {
        private static readonly string[] AllGroups = { "Layer", "Content", "Texture", "Filter", "Modifier", "Project" };

        // A composition with two layers.
        private static Scene CreateScene() => TreeFixtures.CreateScene([2]);

        private static string[] Headers(IEnumerable<TreeAddGroup> groups) => groups.Select(group => group.Header).ToArray();

        private static TreeAddGroup GroupOf(IEnumerable<TreeAddGroup> groups, string header) => groups.Single(group => group.Header == header);

        // The headers of the entries of a group that the node can take.
        private static string[] Enabled(IEnumerable<TreeAddGroup> groups, string header) =>
            GroupOf(groups, header).Items.Where(item => item.Enabled).Select(item => item.Header).ToArray();

        private static int EnabledCount(IEnumerable<TreeAddGroup> groups, string header) => Enabled(groups, header).Length;

        // The entities of a layer: an entity, a fullscreen quad, a text, a camera and a light.
        private static Scene SceneWithContent()
        {
            var scene = CreateScene();
            scene.Layer.ModelEntityManager.AddItem(typeof(Entity));
            scene.Layer.ModelEntityManager.AddItem(typeof(FullscreenQuad));
            scene.Layer.ModelEntityManager.AddItem(typeof(TextEntity));
            scene.Layer.CameraManager.AddItem(typeof(Camera));
            scene.Layer.LightManager.AddItem(typeof(LightEntity));
            scene.Layer.TextureModifierManager.AddItem(typeof(Blur));
            return scene;
        }

        [Fact]
        public void GetAddGroups_AnyNode_ListsTheSameGroupsInTheSameOrder()
        {
            var scene = SceneWithContent();

            foreach (var node in scene.All().Cast<TreeNode?>().Append(null))
                Assert.Equal(AllGroups, Headers(scene.Tree.GetAddGroups(node)));
        }

        [Fact]
        public void GetAddGroups_AnyNode_ListsTheSameEntries()
        {
            var scene = SceneWithContent();
            var expected = scene.Tree.GetAddGroups(null).SelectMany(group => group.Items).Select(item => item.Id).ToList();

            foreach (var node in scene.All())
                Assert.Equal(expected, scene.Tree.GetAddGroups(node).SelectMany(group => group.Items).Select(item => item.Id));
        }

        [Fact]
        public void GetAddGroups_NoNode_EnablesOnlyTheProject()
        {
            var scene = CreateScene();

            var groups = scene.Tree.GetAddGroups(null);

            Assert.Equal(new[] { "Composition" }, groups.SelectMany(group => group.Items).Where(item => item.Enabled).Select(item => item.Header));
        }

        [Fact]
        public void GetAddGroups_ACompositionNode_EnablesTheLayerTheFiltersAndTheModifiersOfItsType()
        {
            var scene = CreateScene();

            var groups = scene.Tree.GetAddGroups(scene.CompositionNode);

            Assert.Equal(new[] { "Layer" }, Enabled(groups, "Layer"));
            Assert.Empty(Enabled(groups, "Content"));
            Assert.Empty(Enabled(groups, "Texture"));
            Assert.Equal(AddableTypes.ForInterface(typeof(ITextureFilter)).Count, EnabledCount(groups, "Filter"));
            Assert.Equal(AddableTypes.ForOwner(typeof(Composition)).Count, EnabledCount(groups, "Modifier"));
            Assert.Equal(new[] { "Composition" }, Enabled(groups, "Project"));
        }

        [Fact]
        public void GetAddGroups_ALayerNode_EnablesTheContentTheFiltersAndTheModifiersOfItsType()
        {
            var scene = CreateScene();

            var groups = scene.Tree.GetAddGroups(scene.LayerNode);

            Assert.Empty(Enabled(groups, "Layer"));
            Assert.Equal(new[] { "Entity", "Fullscreen Quad", "Text", "Camera", "Light" }, Enabled(groups, "Content"));
            Assert.Empty(Enabled(groups, "Texture"));
            Assert.Equal(AddableTypes.ForInterface(typeof(ITextureFilter)).Count, EnabledCount(groups, "Filter"));
            Assert.Equal(AddableTypes.ForOwner(typeof(Layer)).Count, EnabledCount(groups, "Modifier"));
        }

        [Theory]
        [InlineData(typeof(Entity), true, true)]
        [InlineData(typeof(FullscreenQuad), true, false)]
        [InlineData(typeof(TextEntity), false, true)]
        public void GetAddGroups_AnEntityNode_EnablesWhatTheEntityHolds(Type type, bool textures, bool modifiers)
        {
            var scene = CreateScene();
            scene.Layer.ModelEntityManager.AddItem(type);
            var entity = Scene.Group<EntitiesGroup>(scene.LayerNode).Children[0];

            var groups = scene.Tree.GetAddGroups(entity);

            Assert.Equal(textures, EnabledCount(groups, "Texture") > 0);
            Assert.Equal(modifiers, EnabledCount(groups, "Modifier") > 0);
            Assert.Empty(Enabled(groups, "Content"));
            Assert.Empty(Enabled(groups, "Filter"));
        }

        [Fact]
        public void GetAddGroups_ACameraOrALightNode_EnablesOnlyModifiers()
        {
            var scene = CreateScene();
            scene.Layer.CameraManager.AddItem(typeof(Camera));
            scene.Layer.LightManager.AddItem(typeof(LightEntity));

            foreach (var node in new[] { Scene.Group<CamerasGroup>(scene.LayerNode).Children[0], Scene.Group<LightsGroup>(scene.LayerNode).Children[0] })
            {
                var groups = scene.Tree.GetAddGroups(node);

                Assert.True(EnabledCount(groups, "Modifier") > 0);
                Assert.Empty(Enabled(groups, "Texture"));
                Assert.Empty(Enabled(groups, "Filter"));
                Assert.Empty(Enabled(groups, "Content"));
            }
        }

        [Fact]
        public void GetAddGroups_AFilterNode_HasNoModulatorGroup()
        {
            var scene = SceneWithContent();
            var filter = Scene.Group<FiltersGroup>(scene.LayerNode).Children[0];

            var groups = scene.Tree.GetAddGroups(filter);

            Assert.DoesNotContain(groups, group => group.Category == TreeCategory.Modulator || group.Header == "Modulator");
        }

        [Fact]
        public void GetAddGroups_AGroupRow_EnablesOnlyTheKindOfItsOwnList()
        {
            var scene = CreateScene();

            Assert.Equal(new[] { "Camera" }, Enabled(scene.Tree.GetAddGroups(Scene.Group<CamerasGroup>(scene.LayerNode)), "Content"));
            Assert.Equal(new[] { "Light" }, Enabled(scene.Tree.GetAddGroups(Scene.Group<LightsGroup>(scene.LayerNode)), "Content"));
            Assert.Equal(new[] { "Entity", "Fullscreen Quad", "Text" }, Enabled(scene.Tree.GetAddGroups(Scene.Group<EntitiesGroup>(scene.LayerNode)), "Content"));

            var filters = scene.Tree.GetAddGroups(Scene.Group<FiltersGroup>(scene.LayerNode));
            Assert.Equal(AddableTypes.ForInterface(typeof(ITextureFilter)).Count, EnabledCount(filters, "Filter"));
            Assert.Empty(Enabled(filters, "Content"));
        }

        [Fact]
        public void GetAddGroups_TheModifiersGroupOfAnEntity_EnablesTheModifiersOfThatEntity()
        {
            var scene = CreateScene();
            scene.Layer.ModelEntityManager.AddItem(typeof(Entity));
            var group = Scene.Group<ModifiersGroup>(Scene.Group<EntitiesGroup>(scene.LayerNode).Children[0]);

            var groups = scene.Tree.GetAddGroups(group);

            Assert.Equal(AddableTypes.ForOwner(typeof(Entity)).Count, EnabledCount(groups, "Modifier"));
            Assert.Empty(Enabled(groups, "Texture"));
        }

        [Fact]
        public void GetAddGroups_EveryNode_EnablesTheProjectAndEveryEnabledEntryHasAnAction()
        {
            var scene = SceneWithContent();

            foreach (var node in scene.All())
            {
                var groups = scene.Tree.GetAddGroups(node);

                Assert.Equal(new[] { "Composition" }, Enabled(groups, "Project"));
                foreach (var item in groups.SelectMany(group => group.Items))
                    Assert.Equal(item.Enabled, item.Execute != null);
            }
        }

        [Fact]
        public void GetAddGroups_TheEntries_HaveNoWordAddAndTheirKind()
        {
            var scene = CreateScene();

            var groups = scene.Tree.GetAddGroups(scene.LayerNode);

            Assert.DoesNotContain(groups.SelectMany(group => group.Items), item => item.Header.StartsWith("Add ", StringComparison.Ordinal));
            Assert.Equal(
                new TreeCategory?[] { TreeCategory.Entity, TreeCategory.Entity, TreeCategory.Entity, TreeCategory.Camera, TreeCategory.Light },
                GroupOf(groups, "Content").Items.Select(item => item.Category));
            Assert.Null(GroupOf(groups, "Content").Category);
            Assert.All(GroupOf(groups, "Filter").Items, item => Assert.Equal(TreeCategory.Filter, item.Category));
            Assert.All(GroupOf(groups, "Modifier").Items, item => Assert.Equal(TreeCategory.Modifier, item.Category));
            Assert.All(GroupOf(groups, "Texture").Items, item => Assert.Equal(TreeCategory.Texture, item.Category));
        }

        [Fact]
        public void GetAddGroups_ForEveryNode_EnablesTheSameAddsAsGetAddChoices()
        {
            var scene = SceneWithContent();

            foreach (var node in scene.All())
            {
                var choices = scene.Tree.GetAddChoices(node).Where(add => add.Id != "row.add-composition").ToList();
                var expectedIds = choices.Where(add => add.Execute != null).Select(add => add.Id).OrderBy(id => id, StringComparer.Ordinal).ToList();
                var expectedKinds = choices.Where(add => add.Execute == null).Select(add => add.Header["Add ".Length..]).OrderBy(kind => kind, StringComparer.Ordinal).ToList();
                var enabled = scene.Tree.GetAddGroups(node)
                    .SelectMany(group => group.Items.Where(item => item.Enabled && item.Id != "row.add-composition").Select(item => new { group.Header, item.Id }))
                    .ToList();
                var enabledIds = enabled.Where(entry => !expectedKinds.Contains(entry.Header)).Select(entry => entry.Id).OrderBy(id => id, StringComparer.Ordinal).ToList();
                var enabledKinds = enabled.Where(entry => expectedKinds.Contains(entry.Header)).Select(entry => entry.Header).Distinct().OrderBy(kind => kind, StringComparer.Ordinal).ToList();

                Assert.Equal(expectedIds, enabledIds);
                Assert.Equal(expectedKinds, enabledKinds);
            }
        }

        [Fact]
        public void Execute_TheEntriesOfALayerNode_AddWhatTheyName()
        {
            var scene = CreateScene();
            var content = GroupOf(scene.Tree.GetAddGroups(scene.LayerNode), "Content");

            content.Items.Single(item => item.Header == "Camera").Execute!();
            content.Items.Single(item => item.Header == "Entity").Execute!();

            Assert.Single(scene.Layer.CameraManager.ManagerData.Items);
            Assert.Single(scene.Layer.ModelEntityManager.ManagerData.Items);
        }

        [Fact]
        public void Execute_TheLayerAndTheProjectEntries_AddALayerAndAComposition()
        {
            var scene = CreateScene();
            var layers = scene.A.LayerManager.ManagerData.Items.Count;
            var compositions = scene.Project.CompositionManager.ManagerData.Items.Count;
            var groups = scene.Tree.GetAddGroups(scene.CompositionNode);

            GroupOf(groups, "Layer").Items[0].Execute!();
            GroupOf(groups, "Project").Items[0].Execute!();

            Assert.Equal(layers + 1, scene.A.LayerManager.ManagerData.Items.Count);
            Assert.Equal(compositions + 1, scene.Project.CompositionManager.ManagerData.Items.Count);
        }
    }
}
