using CMiX.Core;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // The tree of compositions, layers and the items of a layer follows the lists and selects through them.
    public class ProjectTreeTests
    {
        // Composition A holds two layers, composition B holds one. B is the selected composition.
        private static Scene CreateScene()
        {
            var scene = TreeFixtures.CreateScene();
            scene.ClearUndo();
            return scene;
        }

        private static string NameOf(IControl item) => ((IPrefab)item).PrefabService.Name.Value;

        // The node of a layer of A.
        private static TreeNode LayerNode(Scene scene, int index) => scene.Tree.CompositionNodes()[0].Children[0].Children[index];

        // The layers of a composition: the rows under its Layers group, which is the first row under the composition.
        private static IEnumerable<TreeNode> LayersOf(TreeNode composition) => composition.Children[0].Children.Where(child => child.Item != null);

        private static TreeNode GroupNode<T>(TreeNode layerNode) where T : TreeGroup =>
            layerNode.Children.Single(group => group.Group is T);

        [Fact]
        public void Add_AnItem_StartsExpandedAndOpensItsParent()
        {
            var scene = CreateScene();
            var composition = scene.Tree.CompositionNodes()[0];
            var layers = composition.Children[0];
            var before = layers.Children.Count;
            composition.IsExpanded = false;
            layers.IsExpanded = false;

            scene.A.LayerManager.AddItem(typeof(Layer));

            Assert.True(composition.IsExpanded);
            Assert.True(layers.IsExpanded);
            var added = layers.Children[before];
            Assert.NotNull(added.Item);
            Assert.True(added.IsExpanded);
            Assert.All(added.Children, group => Assert.True(group.IsExpanded));
        }

        [Fact]
        public void Inspect_ANode_MarksOnlyThatNode()
        {
            var scene = CreateScene();
            var first = LayerNode(scene, 0);
            var second = LayerNode(scene, 1);

            scene.Tree.Inspect(first);
            scene.Tree.Inspect(second);

            Assert.False(first.IsInspected);
            Assert.True(second.IsInspected);
            Assert.Single(scene.All(), node => node.IsInspected);

            scene.Tree.Inspect(null);

            Assert.DoesNotContain(scene.All(), node => node.IsInspected);
        }

        [Fact]
        public void ApplySnapshot_AProjectLoads_EveryRowStartsCollapsed()
        {
            var scene = CreateScene();
            var model = (ProjectModel)scene.Project.ToModel();

            scene.Project.ApplySnapshot(model);

            // Every row of the loaded project is collapsed: the compositions, the Layers rows and the layers. The Compositions row is the top of the
            // tree and it stays open, so the compositions are in view.
            Assert.NotEmpty(scene.Tree.CompositionNodes());
            Assert.True(scene.Tree.CompositionsNode().IsExpanded);
            Assert.All(ListTreeTestScreen.All(scene.Tree.CompositionNodes()), node => Assert.False(node.IsExpanded, node.Header));

            // An item that the user adds after the load is expanded again, and so are the rows above it.
            var loaded = (Composition)scene.Project.CompositionManager.ManagerData.Items[0];
            loaded.LayerManager.AddItem(typeof(Layer));
            Assert.True(scene.Tree.CompositionNodes()[0].IsExpanded);
            Assert.True(scene.Tree.CompositionNodes()[0].Children[0].IsExpanded);
        }

        [Fact]
        public void Build_TheTree_PutsTheCompositionsUnderTheCompositionsRowInListOrder()
        {
            var scene = CreateScene();

            var root = Assert.Single(scene.Tree.Roots);
            Assert.IsType<CompositionsGroup>(root.Group);
            Assert.Equal("Compositions", root.Header);
            Assert.Same(scene.Project.CompositionManager, root.ChildManager);
            Assert.Equal(new IControl[] { scene.A, scene.B }, scene.Tree.CompositionNodes().Select(node => node.Item));
            Assert.Equal(new[] { NameOf(scene.A), NameOf(scene.B) }, scene.Tree.CompositionNodes().Select(node => node.Header));
        }

        [Fact]
        public void Build_ACompositionNode_HasItsLayersUnderTheLayersRow()
        {
            var scene = CreateScene();

            Assert.Equal(scene.A.LayerManager.ManagerData.Items, LayersOf(scene.Tree.CompositionNodes()[0]).Select(node => node.Item));
            Assert.Null(scene.Tree.CompositionNodes()[0].ChildManager);
            Assert.Same(scene.A.LayerManager, scene.Tree.CompositionNodes()[0].Children[0].ChildManager);
            Assert.Same(scene.A.LayerManager, LayerNode(scene, 0).Owner);
        }

        [Fact]
        public void Build_AGroupNode_HasTheItemsOfItsListAsChildren()
        {
            var scene = CreateScene();
            var layer = scene.LayerOfA(0);
            layer.ModelEntityManager.AddItem(typeof(Entity));
            layer.CameraManager.AddItem(typeof(Camera));
            layer.LightManager.AddItem(typeof(LightEntity));

            var node = LayerNode(scene, 0);

            Assert.Same(layer.ModelEntityManager.ManagerData.Items[0], Assert.Single(GroupNode<EntitiesGroup>(node).Children).Item);
            Assert.Same(layer.CameraManager.ManagerData.Items[0], Assert.Single(GroupNode<CamerasGroup>(node).Children).Item);
            Assert.Same(layer.LightManager.ManagerData.Items[0], Assert.Single(GroupNode<LightsGroup>(node).Children).Item);
        }

        [Fact]
        public void Add_AnItem_AddsANode()
        {
            var scene = CreateScene();

            scene.A.LayerManager.AddItem(typeof(Layer));
            scene.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));

            Assert.Equal(3, LayersOf(scene.Tree.CompositionNodes()[0]).Count());
            Assert.Single(GroupNode<EntitiesGroup>(LayerNode(scene, 0)).Children);
        }

        [Fact]
        public void Remove_AnItem_RemovesItsNode()
        {
            var scene = CreateScene();

            scene.A.LayerManager.RemoveItem(scene.LayerOfA(0));

            Assert.Single(LayersOf(scene.Tree.CompositionNodes()[0]));
        }

        [Fact]
        public void Move_AnItem_MovesItsNode()
        {
            var scene = CreateScene();
            var first = scene.LayerOfA(0);

            scene.A.LayerManager.MoveItem(0, 1);

            Assert.Same(first, scene.Tree.CompositionNodes()[0].Children[0].Children[1].Item);
            Assert.Equal(scene.A.LayerManager.ManagerData.Items, LayersOf(scene.Tree.CompositionNodes()[0]).Select(node => node.Item));
        }

        [Fact]
        public void Move_FromAPeer_MovesTheNode()
        {
            var scene = CreateScene();
            var first = scene.LayerOfA(0);

            scene.A.LayerManager.ManagerData.Move(0, 1);

            Assert.Same(first, scene.Tree.CompositionNodes()[0].Children[0].Children[1].Item);
        }

        [Fact]
        public void Rename_AnItem_ChangesTheHeader()
        {
            var scene = CreateScene();

            scene.LayerOfA(0).PrefabService.Name.Value = "Back";

            Assert.Equal("Back", LayerNode(scene, 0).Header);
        }

        [Fact]
        public void Move_ALayerToAnotherComposition_MovesItsNode()
        {
            var scene = CreateScene();
            var moved = scene.LayerOfA(0);

            scene.B.LayerManager.MoveItemToManager(moved, scene.A.LayerManager, 0);

            Assert.Single(LayersOf(scene.Tree.CompositionNodes()[0]));
            Assert.Equal(2, LayersOf(scene.Tree.CompositionNodes()[1]).Count());
            Assert.Same(moved, scene.Tree.CompositionNodes()[1].Children[0].Children[0].Item);
            Assert.Same(scene.B.LayerManager, scene.Tree.CompositionNodes()[1].Children[0].Children[0].Owner);
        }

        [Fact]
        public void Dispose_TheTree_StopsFollowingTheLists()
        {
            var scene = CreateScene();
            var node = scene.Tree.CompositionNodes()[0];

            scene.Tree.Dispose();
            scene.A.LayerManager.AddItem(typeof(Layer));

            Assert.Equal(2, LayersOf(node).Count());
        }

        [Fact]
        public void Select_ALayerNode_SelectsTheCompositionAndTheLayer()
        {
            var scene = CreateScene();
            var layer = scene.LayerOfA(0);

            scene.Tree.SelectedNode = LayerNode(scene, 0);

            Assert.Same(scene.A, scene.Project.CompositionManager.SelectedItem);
            Assert.Same(layer, scene.A.LayerManager.SelectedItem);
        }

        [Fact]
        public void Select_AnEntityNode_SelectsTheWholeChain()
        {
            var scene = CreateScene();
            var layer = scene.LayerOfA(0);
            layer.ModelEntityManager.AddItem(typeof(Entity));
            layer.ModelEntityManager.AddItem(typeof(Entity));
            var first = layer.ModelEntityManager.ManagerData.Items[0];

            scene.Tree.SelectedNode = GroupNode<EntitiesGroup>(LayerNode(scene, 0)).Children[0];

            Assert.Same(scene.A, scene.Project.CompositionManager.SelectedItem);
            Assert.Same(layer, scene.A.LayerManager.SelectedItem);
            Assert.Same(first, layer.ModelEntityManager.SelectedItem);
        }

        [Fact]
        public void Select_AGroupNode_SelectsOnlyItsAncestors()
        {
            var scene = CreateScene();
            var layer = scene.LayerOfA(0);
            layer.ModelEntityManager.AddItem(typeof(Entity));
            layer.ModelEntityManager.ClearSelection();

            scene.Tree.SelectedNode = GroupNode<EntitiesGroup>(LayerNode(scene, 0));

            Assert.Same(layer, scene.A.LayerManager.SelectedItem);
            Assert.Null(layer.ModelEntityManager.SelectedItem);
        }

        [Fact]
        public void Select_ANode_IsOneUndoStep()
        {
            var scene = CreateScene();
            var secondLayer = scene.LayerOfA(1);
            scene.Undo.Clear();

            scene.Tree.SelectedNode = LayerNode(scene, 0);
            Assert.Same(scene.A, scene.Project.CompositionManager.SelectedItem);
            Assert.Same(scene.LayerOfA(0), scene.A.LayerManager.SelectedItem);

            scene.Undo.Undo();

            Assert.Same(scene.B, scene.Project.CompositionManager.SelectedItem);
            Assert.Same(secondLayer, scene.A.LayerManager.SelectedItem);
        }

        [Fact]
        public void Select_AnItemInTheLists_MovesTheSelectedNode()
        {
            var scene = CreateScene();

            scene.A.LayerManager.SelectAt(0);

            Assert.Same(scene.LayerOfA(0), scene.Tree.SelectedNode!.Item);
        }

        [Fact]
        public void Select_NoNode_KeepsTheSelectionOfTheLists()
        {
            var scene = CreateScene();
            scene.Tree.SelectedNode = LayerNode(scene, 0);
            scene.Undo.Clear();

            scene.Tree.SelectedNode = LayerNode(scene, 0);

            Assert.False(scene.Undo.CanUndo);
        }

        [Fact]
        public void Remove_TheSelectedNode_SelectsTheParent()
        {
            var scene = CreateScene();
            var layer = scene.LayerOfA(0);
            layer.ModelEntityManager.AddItem(typeof(Entity));
            var entity = layer.ModelEntityManager.ManagerData.Items[0];
            scene.Tree.SelectedNode = GroupNode<EntitiesGroup>(LayerNode(scene, 0)).Children[0];

            layer.ModelEntityManager.RemoveItem(entity);

            Assert.Equal("Entities", scene.Tree.SelectedNode!.Header);
        }

        [Fact]
        public void Remove_TheSelectedComposition_SelectsTheNeighbor()
        {
            var scene = CreateScene();
            scene.Tree.SelectedNode = scene.Tree.CompositionNodes()[1];

            scene.Project.CompositionManager.RemoveItem(scene.B);

            Assert.Same(scene.A, scene.Project.CompositionManager.SelectedItem);
            Assert.Same(scene.A, scene.Tree.SelectedNode!.Item);
        }

        [Fact]
        public void EnsureCursor_NoCursorAndASelection_PutsTheCursorOnTheSelectedNode()
        {
            var scene = CreateScene();
            var selected = LayerNode(scene, 1);
            scene.Tree.SelectedNode = selected;
            scene.Tree.CursorNode = null;

            scene.Tree.EnsureCursor();

            Assert.Same(selected, scene.Tree.CursorNode);
        }

        [Fact]
        public void EnsureCursor_NoCursorAndNoSelection_PutsTheCursorOnTheFirstRoot()
        {
            var scene = CreateScene();
            scene.Tree.SelectedNode = null;

            scene.Tree.EnsureCursor();

            Assert.Same(scene.Tree.Roots[0], scene.Tree.CursorNode);
        }

        [Fact]
        public void EnsureCursor_ACursor_KeepsTheCursor()
        {
            var scene = CreateScene();
            scene.Tree.SelectedNode = LayerNode(scene, 1);
            var cursor = LayerNode(scene, 0);
            scene.Tree.CursorNode = cursor;

            scene.Tree.EnsureCursor();

            Assert.Same(cursor, scene.Tree.CursorNode);
        }

        [Fact]
        public void Remove_TheOnlyComposition_ClearsTheSelection()
        {
            var scene = CreateScene();
            scene.Project.CompositionManager.RemoveItem(scene.A);
            scene.Tree.SelectedNode = scene.Tree.CompositionNodes()[0];

            scene.Project.CompositionManager.RemoveItem(scene.B);

            // The Compositions row is the top of the tree: it is not selected for the inspector, so the selection is empty.
            Assert.Null(scene.Tree.SelectedNode);
        }

        [Fact]
        public void MaskOwner_EachKind_IsSetForACompositionAndALayerOnly()
        {
            var scene = TreeFixtures.CreateScene([1], s => s.Layer.ModelEntityManager.AddItem(typeof(Entity)));
            var entities = Scene.Group<EntitiesGroup>(scene.LayerNode);

            Assert.Same(scene.A, scene.CompositionNode.MaskOwner);
            Assert.Same(scene.Layer, scene.LayerNode.MaskOwner);
            Assert.Null(entities.MaskOwner);
            Assert.Null(entities.Children[0].MaskOwner);
        }
    }
}
