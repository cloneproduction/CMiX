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
using CMiX.Core;
using CMiX.Core.Prefabs;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Entity = CMiX.Core.Compositing.Entity;
using LightEntity = CMiX.Core.Rendering.Lights.LightEntity;
using Project = CMiX.Core.Compositing.Project;

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

        // The state of the five commands: add, duplicate, move up, move down, remove.
        private static bool[] CommandsEnabled(ProjectTree tree) => new[]
        {
            tree.AddItemCommand.CanExecute(null),
            tree.DuplicateItemCommand.CanExecute(null),
            tree.MoveItemUpCommand.CanExecute(null),
            tree.MoveItemDownCommand.CanExecute(null),
            tree.RemoveItemCommand.CanExecute(null)
        };

        [Fact]
        public void CanExecute_NothingSelected_EnablesOnlyAddForAComposition()
        {
            var scene = TreeFixtures.CreateScene();
            scene.Tree.SelectedNode = null;

            Assert.Equal(new[] { true, false, false, false, false }, CommandsEnabled(scene.Tree));

            scene.Tree.AddItemCommand.Execute(scene.Tree.TargetAdds.Single());

            Assert.Equal(3, scene.Project.CompositionManager.ManagerData.Items.Count);
            Assert.IsType<Composition>(scene.Project.CompositionManager.ManagerData.Items[2]);
        }

        [Fact]
        public void AddAtTargetCommand_PointerOverTheTree_AsksTheViewForTheMenu()
        {
            var scene = TreeFixtures.CreateScene();
            scene.Tree.AttachView(new TreeViewHooks { IsPointerOverTree = () => true });
            var asked = 0;
            scene.Tree.AddRequested += (_, _) => asked++;

            Assert.True(scene.Tree.AddAtTargetCommand.CanExecute(null));
            scene.Tree.AddAtTargetCommand.Execute(null);

            Assert.Equal(1, asked);
            Assert.Equal(2, scene.Project.CompositionManager.ManagerData.Items.Count);
        }

        [Fact]
        public void AddAtTargetCommand_NothingSelected_AddsNothingAtOnce()
        {
            var scene = TreeFixtures.CreateScene();
            scene.Tree.SelectedNode = null;
            scene.Tree.AttachView(new TreeViewHooks { IsPointerOverTree = () => true });

            scene.Tree.AddAtTargetCommand.Execute(null);

            Assert.Equal(2, scene.Project.CompositionManager.ManagerData.Items.Count);
        }

        [Fact]
        public void AddAtTargetCommand_PointerAwayFromTheTree_CannotRun()
        {
            var scene = TreeFixtures.CreateScene();

            Assert.False(scene.Tree.AddAtTargetCommand.CanExecute(null));
        }

        [Fact]
        public void Add_WithACompositionSelected_AddsALayerToIt()
        {
            var scene = TreeFixtures.CreateScene();
            scene.Tree.SelectedNode = scene.Tree.CompositionNodes()[1];

            // The toolbar opens a menu for a selected node. Its first row is the layer.
            scene.Tree.GetAddChoices(scene.Tree.SelectedNode).Single(choice => choice.Header == "Add Layer").Execute!.Invoke();

            Assert.Equal(2, scene.B.LayerManager.ManagerData.Items.Count);
            Assert.Equal(2, scene.A.LayerManager.ManagerData.Items.Count);
        }

        [Fact]
        public void CanExecute_TheFirstAndTheLastComposition_StopsMovesPastTheEdge()
        {
            var scene = TreeFixtures.CreateScene();

            scene.Tree.SelectedNode = scene.Tree.CompositionNodes()[0];
            Assert.Equal(new[] { true, true, false, true, true }, CommandsEnabled(scene.Tree));

            scene.Tree.SelectedNode = scene.Tree.CompositionNodes()[1];
            Assert.Equal(new[] { true, true, true, false, true }, CommandsEnabled(scene.Tree));
        }

        [Fact]
        public void GetAdds_ALayerSelected_OffersFiveThings()
        {
            var scene = TreeFixtures.CreateScene();
            scene.Tree.SelectedNode = scene.LayerNodeOfA(0);

            var choices = scene.Tree.GetAddChoices(scene.Tree.SelectedNode);
            scene.Tree.AddItemCommand.Execute(null);

            Assert.Equal(new[] { "Entity", "Fullscreen Quad", "Text", "Camera", "Light", "Add Filter", "Add Modifier", "Add Composition" }, choices.Select(choice => choice.Header));
            Assert.True(scene.Tree.AddItemCommand.CanExecute(null));
            Assert.Empty(scene.LayerOfA(0).ModelEntityManager.ManagerData.Items);
            Assert.Empty(scene.LayerOfA(0).CameraManager.ManagerData.Items);
        }

        [Fact]
        public void GetAdds_TheEntitiesGroup_OffersThreeThings()
        {
            var scene = TreeFixtures.CreateScene();

            var choices = scene.Tree.GetAddChoices(scene.Group<EntitiesGroup>(0));

            Assert.Equal(new[] { "Add Entity", "Add Fullscreen Quad", "Add Text", "Add Composition" }, choices.Select(choice => choice.Header));
        }

        [Fact]
        public void GetAdds_AnySelection_PutsTheCompositionLast()
        {
            var scene = TreeFixtures.CreateScene();
            scene.A.LayerManager.AddItem(typeof(Layer));
            scene.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            string[] Headers(CMiX.Studio.Avalonia.ViewModels.Tree.TreeNode? node) => scene.Tree.GetAddChoices(node).Select(choice => choice.Header).ToArray();

            Assert.Equal(new[] { "Add Composition" }, Headers(null));
            Assert.Equal(new[] { "Add Layer", "Add Filter", "Add Modifier", "Add Composition" }, Headers(scene.Tree.CompositionNodes()[0]));
            Assert.Equal("Add Composition", Headers(scene.LayerNodeOfA(0)).Last());
            Assert.Equal(new[] { "Add Camera", "Add Composition" }, Headers(scene.Group<CamerasGroup>(0)));
            Assert.Equal(new[] { "Add Texture", "Add Modifier", "Add Composition" }, Headers(scene.Group<EntitiesGroup>(0).Children[0]));
        }

        [Fact]
        public void Add_WithTheCamerasOrLightsGroupSelected_AddsACameraOrALight()
        {
            var scene = TreeFixtures.CreateScene();
            var layer = scene.LayerOfA(0);

            // The toolbar opens a menu for a group, so the test takes the row of the kind from the choices.
            scene.Tree.GetAddChoices(scene.Group<CamerasGroup>(0)).Single(choice => choice.Header == "Add Camera").Execute!.Invoke();
            scene.Tree.GetAddChoices(scene.Group<LightsGroup>(0)).Single(choice => choice.Header == "Add Light").Execute!.Invoke();

            Assert.IsType<Camera>(Assert.Single(layer.CameraManager.ManagerData.Items));
            Assert.IsType<LightEntity>(Assert.Single(layer.LightManager.ManagerData.Items));
        }

        [Theory]
        [InlineData(typeof(EntitiesGroup))]
        [InlineData(typeof(CamerasGroup))]
        [InlineData(typeof(LightsGroup))]
        public void CanExecute_AGroupSelected_EnablesOnlyAdd(System.Type group)
        {
            var scene = TreeFixtures.CreateScene();
            scene.Tree.SelectedNode = scene.Group(0, group);

            Assert.Equal(new[] { true, false, false, false, false }, CommandsEnabled(scene.Tree));
        }

        [Fact]
        public void CanExecute_AnEntitySelected_EnablesEveryCommand()
        {
            var scene = TreeFixtures.CreateScene();
            scene.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            scene.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            var group = scene.Group<EntitiesGroup>(0);

            // Add is on: a composition can be added whatever is selected.
            scene.Tree.SelectedNode = group.Children[0];
            Assert.Equal(new[] { true, true, false, true, true }, CommandsEnabled(scene.Tree));

            scene.Tree.SelectedNode = group.Children[1];
            Assert.Equal(new[] { true, true, true, false, true }, CommandsEnabled(scene.Tree));
        }

        [Fact]
        public void Duplicate_ALayer_PutsTheCopyRightAfterIt()
        {
            var scene = TreeFixtures.CreateScene();
            var original = scene.LayerOfA(0);
            scene.Tree.SelectedNode = scene.LayerNodeOfA(0);

            scene.Tree.DuplicateItemCommand.Execute(null);

            Assert.Equal(3, scene.A.LayerManager.ManagerData.Items.Count);
            Assert.Same(original, scene.A.LayerManager.ManagerData.Items[0]);
            Assert.NotSame(original, scene.A.LayerManager.ManagerData.Items[1]);
        }

        [Fact]
        public void MoveDownAndMoveUp_AComposition_MoveIt()
        {
            var scene = TreeFixtures.CreateScene();
            scene.Tree.SelectedNode = scene.Tree.CompositionNodes()[0];

            scene.Tree.MoveItemDownCommand.Execute(null);
            Assert.Equal(new IControl[] { scene.B, scene.A }, scene.Project.CompositionManager.ManagerData.Items);
            Assert.Same(scene.A, scene.Tree.SelectedNode!.Item);

            scene.Tree.MoveItemUpCommand.Execute(null);
            Assert.Equal(new IControl[] { scene.A, scene.B }, scene.Project.CompositionManager.ManagerData.Items);
        }

        [Fact]
        public void Remove_ALayer_TakesItOutOfItsComposition()
        {
            var scene = TreeFixtures.CreateScene();
            var second = scene.LayerOfA(1);
            scene.Tree.SelectedNode = scene.LayerNodeOfA(0);

            scene.Tree.RemoveItemCommand.Execute(null);

            Assert.Equal(new IControl[] { second }, scene.A.LayerManager.ManagerData.Items);
        }

        [Fact]
        public void Select_AnotherNode_RaisesCanExecuteChanged()
        {
            var scene = TreeFixtures.CreateScene();
            var raised = 0;
            scene.Tree.MoveItemDownCommand.CanExecuteChanged += (_, _) => raised++;

            scene.Tree.SelectedNode = scene.Tree.CompositionNodes()[0];

            Assert.True(raised > 0);
        }

        [Fact]
        public void CanExecute_AChangeOfTheLists_FollowsTheTree()
        {
            var scene = TreeFixtures.CreateScene();
            scene.Tree.SelectedNode = scene.LayerNodeOfA(0);
            Assert.True(scene.Tree.MoveItemDownCommand.CanExecute(null));
            var raised = 0;
            scene.Tree.MoveItemDownCommand.CanExecuteChanged += (_, _) => raised++;

            // The second layer leaves: the selected layer is now the last one.
            scene.A.LayerManager.RemoveItem(scene.LayerOfA(1));

            Assert.False(scene.Tree.MoveItemDownCommand.CanExecute(null));
            Assert.True(raised > 0);
        }

        [Fact]
        public void CanExecute_TheCursorOnAnotherRow_FollowsTheCursorRow()
        {
            var scene = TreeFixtures.CreateScene();
            scene.Tree.SelectedNode = scene.LayerNodeOfA(0);
            var raised = 0;
            scene.Tree.MoveItemUpCommand.CanExecuteChanged += (_, _) => raised++;

            scene.Tree.CursorNode = scene.LayerNodeOfA(1);

            Assert.True(raised > 0);
            Assert.Same(scene.LayerNodeOfA(1), scene.Tree.TargetNode);
            Assert.Equal(new[] { true, true, true, false, true }, CommandsEnabled(scene.Tree));
        }

        [Fact]
        public void TargetAdds_TheCursorOnAGroup_FollowTheCursorRow()
        {
            var scene = TreeFixtures.CreateScene();
            scene.Tree.SelectedNode = scene.LayerNodeOfA(0);

            scene.Tree.CursorNode = scene.Group<CamerasGroup>(0);

            Assert.Equal(new[] { "Add Camera", "Add Composition" }, scene.Tree.TargetAdds.Select(add => add.Header));
            Assert.Equal(new[] { true, false, false, false, false }, CommandsEnabled(scene.Tree));
        }

        [Fact]
        public void TargetAdds_SelectionChanges_FollowsTheAddsOfTheSelectedNode()
        {
            var provider = TestServiceProviderFactory.Create();
            var project = provider.GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));
            var tree = TreeFixtures.CreateTree(provider);

            Assert.Equal(new[] { "Add Composition" }, tree.TargetAdds.Select(add => add.Header));

            tree.SelectedNode = tree.CompositionNodes()[0];

            Assert.Equal(tree.GetAddChoices(tree.CompositionNodes()[0]).Select(add => add.Header), tree.TargetAdds.Select(add => add.Header));
            Assert.True(tree.AddItemCommand.CanExecute(null));
        }
    }
}
