using CMiX.Core.Compositing;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // The nodes of the scene of ProjectTreeDropTargetsTests.
    file static class DropTargetsNodes
    {
        extension(Scene scene)
        {
            public TreeNode CompositionA => scene.NodeA;
            public TreeNode CompositionB => scene.NodeB;
            public TreeNode Layer1 => scene.LayerNodeOfA(0);
            public TreeNode Layer2 => scene.LayerNodeOfA(1);
            public TreeNode LayerOfB => scene.LayerNodeOfB(0);
            public TreeNode Entity1 => Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(0)).Children[0];
            public TreeNode Entity2 => Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(0)).Children[1];
            public TreeNode EntityOfLayer2 => Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(1)).Children[0];
        }
    }

    // During a drag, the rows that take the node inside are lit (candidates), the rows that it can go next to keep their look (neighbors,
    // the bar shows the place), and the other rows are dimmed. The rules are the ones of the drop itself.
    public class ProjectTreeDropTargetsTests
    {
        // Composition A: layer 1 has two entities, a camera, modifiers on the entities and two filters. Layer 2 has one entity and one
        // filter. B has one layer.
        private static Scene CreateScene() => TreeFixtures.CreateScene(fill: Fill);

        private static void Fill(Scene scene)
        {
            var layer1 = scene.LayerOfA(0);
            var layer2 = scene.LayerOfA(1);
            layer1.ModelEntityManager.AddItem(typeof(Entity));
            layer1.ModelEntityManager.AddItem(typeof(Entity));
            layer1.CameraManager.AddItem(typeof(CMiX.Core.Rendering.Cameras.Camera));
            layer2.ModelEntityManager.AddItem(typeof(Entity));
            var first = (Entity)layer1.ModelEntityManager.ManagerData.Items[0];
            first.ModifierManager.AddItem(typeof(TranslateModifier));
            first.ModifierManager.AddItem(typeof(ScaleModifier));
            var second = (Entity)layer1.ModelEntityManager.ManagerData.Items[1];
            second.ModifierManager.AddItem(typeof(TranslateModifier));
            layer1.TextureModifierManager.AddItem(typeof(CMiX.Core.Texturing.Filters.Blur));
            layer1.TextureModifierManager.AddItem(typeof(CMiX.Core.Texturing.Filters.Blur));
            layer2.TextureModifierManager.AddItem(typeof(CMiX.Core.Texturing.Filters.Blur));
        }

        [Fact]
        public void Build_BeforeADrag_GivesNoRowADragState()
        {
            var scene = CreateScene();

            Assert.False(scene.Tree.IsDragging);
            Assert.All(scene.All(), node =>
            {
                Assert.False(node.IsDragActive);
                Assert.False(node.IsDropCandidate);
                Assert.False(node.IsDropNeighbor);
                Assert.False(node.IsDimmed);
            });
        }

        [Fact]
        public void BeginDrag_AnEntity_LightsTheEntityGroupsAndTheLayersOnly()
        {
            var scene = CreateScene();

            scene.Tree.BeginDrag(scene.Entity1);

            Assert.Equal(
                new[] { scene.Layer1, Scene.Group<EntitiesGroup>(scene.Layer1), scene.Layer2, Scene.Group<EntitiesGroup>(scene.Layer2), scene.LayerOfB, Scene.Group<EntitiesGroup>(scene.LayerOfB) },
                scene.Candidates());
            Assert.True(scene.Tree.IsDragging);
        }

        [Fact]
        public void BeginDrag_AnEntity_MarksTheOtherEntitiesAsNeighbors()
        {
            var scene = CreateScene();

            scene.Tree.BeginDrag(scene.Entity1);

            Assert.Equal(new[] { scene.Entity2, scene.EntityOfLayer2 }, scene.Neighbors());
            Assert.All(scene.Neighbors(), node =>
            {
                Assert.False(node.IsDropCandidate);
                Assert.False(node.IsDimmed);
            });
        }

        // The case of a filter: only the groups of filters and the layers light up. The filters themselves keep their look, and the bar shows the place.
        [Fact]
        public void BeginDrag_AFilter_LightsTheFilterGroupsAndTheLayersOnly()
        {
            var scene = CreateScene();
            var filters = Scene.Group<FiltersGroup>(scene.Layer1);
            var filter = filters.Children[0];

            scene.Tree.BeginDrag(filter);

            Assert.Equal(
                new[] { scene.Layer1, filters, scene.Layer2, Scene.Group<FiltersGroup>(scene.Layer2), Scene.Group<FiltersGroup>(scene.CompositionA), scene.LayerOfB, Scene.Group<FiltersGroup>(scene.LayerOfB), Scene.Group<FiltersGroup>(scene.CompositionB) },
                scene.Candidates());
            Assert.All(scene.Candidates(), node => Assert.True(node.Item == null || node.Item is Layer));

            // The other filter of the list and the filter of the other layer: next to them, not inside.
            Assert.Equal(new[] { filters.Children[1], Scene.Group<FiltersGroup>(scene.Layer2).Children[0] }, scene.Neighbors());
        }

        [Fact]
        public void BeginDrag_ARow_MarksItsGroupButNeverTheRowAsCandidate()
        {
            var scene = CreateScene();

            scene.Tree.BeginDrag(scene.Entity1);

            Assert.True(scene.Entity1.IsDragSource);
            Assert.False(scene.Entity1.IsDropCandidate);
            Assert.False(scene.Entity1.IsDropNeighbor);
            Assert.True(Scene.Group<EntitiesGroup>(scene.Layer1).IsDropCandidate);
            Assert.False(Scene.Group<CamerasGroup>(scene.Layer1).IsDropCandidate);

            // The last entity of the list is at the end already: its own group has nothing to do for it.
            scene.Tree.EndDrag();
            scene.Tree.BeginDrag(scene.Entity2);

            Assert.False(Scene.Group<EntitiesGroup>(scene.Layer1).IsDropCandidate);
            Assert.True(Scene.Group<EntitiesGroup>(scene.Layer2).IsDropCandidate);
        }

        // The case of a row of modifiers: the group "Modifiers" of the entity takes it, and it goes to the end of the list.
        [Fact]
        public void Drop_ARowOnItsOwnGroup_PutsItAtTheEndOfItsList()
        {
            var scene = CreateScene();
            var service = new TreeDropService();
            var group = Scene.Group<ModifiersGroup>(scene.Entity1);
            var first = group.Children[0];
            var second = group.Children[1];
            var firstItem = first.Item;

            Assert.True(service.CanDrop(first, group, DropPosition.Inside));
            Assert.False(service.CanDrop(second, group, DropPosition.Inside));

            var outcome = service.Drop(first, group, DropPosition.Inside);

            Assert.True(outcome.Moved);
            Assert.Same(firstItem, ((Entity)scene.Entity1.Item!).ModifierManager.ManagerData.Items[^1]);
            Assert.Equal(2, ((Entity)scene.Entity1.Item!).ModifierManager.ManagerData.Items.Count);
        }

        [Fact]
        public void BeginDrag_ALayer_LightsTheCompositionsAndTheirLayersRowsAndMarksTheLayersAsNeighbors()
        {
            var scene = CreateScene();

            scene.Tree.BeginDrag(scene.Layer1);

            // A composition row takes a layer through its Layers row, so both rows light.
            Assert.Equal(new[] { scene.CompositionA, Scene.Group<LayersGroup>(scene.CompositionA), scene.CompositionB, Scene.Group<LayersGroup>(scene.CompositionB) }, scene.Candidates());
            Assert.Equal(new[] { scene.Layer2, scene.LayerOfB }, scene.Neighbors());
        }

        [Fact]
        public void BeginDrag_AModifier_LightsTheModifierGroupsOnly()
        {
            var scene = CreateScene();
            var firstModifier = Scene.Group<ModifiersGroup>(scene.Entity1).Children[0];
            var secondEntityGroup = Scene.Group<ModifiersGroup>(scene.Entity2);

            scene.Tree.BeginDrag(firstModifier);

            Assert.Equal(
                new[] { Scene.Group<ModifiersGroup>(scene.Entity1), secondEntityGroup, Scene.Group<ModifiersGroup>(scene.EntityOfLayer2) },
                scene.Candidates());
            Assert.Contains(Scene.Group<ModifiersGroup>(scene.Entity1).Children[1], scene.Neighbors());
            Assert.Contains(secondEntityGroup.Children[0], scene.Neighbors());
            Assert.DoesNotContain(Scene.Group<FiltersGroup>(scene.Layer1), scene.Candidates());
            Assert.All(scene.Candidates().Concat(scene.Neighbors()), node => Assert.Equal(TreeCategory.Modifier, node.Category));
        }

        [Fact]
        public void BeginDrag_ANode_CoversEveryRowThatIsNotATargetOrAHint()
        {
            var scene = CreateScene();

            scene.Tree.BeginDrag(scene.Entity1);

            foreach (var node in scene.All())
            {
                if (node.IsDropCandidate || node.IsDropNeighbor || node.IsDropHint)
                    Assert.False(node.IsDimmed);
                else
                    Assert.True(node.IsDimmed, $"{node.Header} is not dimmed.");
            }
        }

        [Fact]
        public void BeginDrag_ACollapsedRowWithATargetUnderIt_GivesAHint()
        {
            var scene = CreateScene();
            scene.CompositionB.IsExpanded = false;

            scene.Tree.BeginDrag(scene.Entity1);

            Assert.True(scene.CompositionB.HasDropTargetBelow);
            Assert.True(scene.CompositionB.IsDropHint);
            Assert.False(scene.CompositionB.IsDimmed);
            Assert.False(scene.CompositionA.IsDropHint);

            scene.CompositionB.IsExpanded = true;
            Assert.False(scene.CompositionB.IsDropHint);
            Assert.True(scene.CompositionB.IsDimmed);
        }

        [Fact]
        public void BeginDrag_AnEntity_MarksTheLayerRowAsCandidate()
        {
            var scene = CreateScene();

            scene.Tree.BeginDrag(scene.Entity1);

            Assert.True(scene.LayerOfB.IsDropCandidate);
            Assert.True(scene.Layer2.IsDropCandidate);
            Assert.False(scene.LayerOfB.IsDimmed);
            Assert.True(scene.Tree.CanDrop(scene.Entity1, scene.LayerOfB, DropPosition.Inside));
        }

        [Fact]
        public void BeginDrag_ASharedEntity_MarksTheOtherLayerBlockedAndCovered()
        {
            var scene = CreateScene();
            var layer2 = (Layer)scene.Layer2.Item!;
            layer2.ModelEntityManager.AddExistingItem((CMiX.Core.Prefabs.IPrefab)scene.Entity1.Item!);

            scene.Tree.BeginDrag(scene.Entity1);

            foreach (var node in new[] { scene.LayerOfB, Scene.Group<EntitiesGroup>(scene.LayerOfB) })
            {
                Assert.True(node.IsDropBlocked, $"{node.Header} is not blocked.");
                Assert.False(node.IsDropCandidate);
                Assert.True(node.IsDimmed);
                Assert.Equal($"Layer \"{scene.Layer2.Header}\" also holds it", node.DropBlockReason);
                Assert.False(scene.Tree.CanDrop(scene.Entity1, node, DropPosition.Inside));
            }

            // The layers of the same composition take the entity: it is in one composition only.
            Assert.False(scene.Layer2.IsDropBlocked);
            Assert.False(scene.Layer1.IsDropBlocked);

            scene.Tree.EndDrag();
            Assert.All(scene.All(), node => Assert.False(node.IsDropBlocked));
            Assert.All(scene.All(), node => Assert.Null(node.DropBlockReason));
        }

        [Fact]
        public void BeginDrag_AModifierThatALayerCannotHold_MarksTheLayersBlockedAndCovered()
        {
            var scene = CreateScene();
            var modifier = Scene.Group<ModifiersGroup>(scene.Entity1).Children[0];

            scene.Tree.BeginDrag(modifier);

            Assert.True(scene.Layer2.IsDropBlocked);
            Assert.True(Scene.Group<ModifiersGroup>(scene.Layer2).IsDropBlocked);
            Assert.Equal("A Translate cannot go to a layer", scene.Layer2.DropBlockReason);
            Assert.True(scene.Layer2.IsDimmed);
        }

        [Fact]
        public void BeginDrag_ARowWithNoTargetUnderIt_GivesNoHint()
        {
            var scene = CreateScene();
            scene.Tree.BeginDrag(scene.Entity1);

            var cameras = Scene.Group<CamerasGroup>(scene.Layer1);
            cameras.IsExpanded = false;

            Assert.False(cameras.HasDropTargetBelow);
            Assert.False(cameras.IsDropHint);
        }

        [Fact]
        public void EndDrag_AfterADrag_ClearsEveryStateAndTellsTheChanges()
        {
            var scene = CreateScene();
            var changes = new List<string?>();
            var dimmedChanges = new List<string?>();
            scene.Tree.BeginDrag(scene.Entity1);
            Scene.Group<EntitiesGroup>(scene.Layer2).PropertyChanged += (_, e) => changes.Add(e.PropertyName);
            var dimmed = scene.All().First(node => node.IsDimmed);
            dimmed.PropertyChanged += (_, e) => dimmedChanges.Add(e.PropertyName);

            scene.Tree.EndDrag();

            Assert.False(scene.Tree.IsDragging);
            Assert.All(scene.All(), node =>
            {
                Assert.False(node.IsDragActive);
                Assert.False(node.IsDragSource);
                Assert.False(node.IsDropCandidate);
                Assert.False(node.IsDropNeighbor);
                Assert.False(node.IsDimmed);
            });
            Assert.Contains(nameof(TreeNode.IsDropCandidate), changes);
            Assert.Contains(nameof(TreeNode.IsDimmed), dimmedChanges);
        }

        [Fact]
        public void BeginDrag_AfterAnotherDrag_StartsFromScratch()
        {
            var scene = CreateScene();
            scene.Tree.BeginDrag(scene.Entity1);
            scene.Tree.EndDrag();

            scene.Tree.BeginDrag(scene.Layer1);

            Assert.DoesNotContain(scene.Entity2, scene.Neighbors());
            Assert.Contains(scene.Layer2, scene.Neighbors());
        }

        [Fact]
        public void DragOver_ALitRowOrANeighbor_TakesTheDropInsideOrBeside()
        {
            var scene = CreateScene();
            var service = new TreeDropService();
            var filter = Scene.Group<FiltersGroup>(scene.Layer1).Children[0];
            scene.Tree.BeginDrag(filter);

            foreach (var node in scene.All().Where(node => !ReferenceEquals(node, filter)))
            {
                if (node.IsDropCandidate)
                    Assert.True(service.CanDrop(filter, node, DropPosition.Inside), $"{node.Header} is lit but takes no drop inside.");

                if (node.IsDropNeighbor)
                    Assert.True(service.CanDrop(filter, node, DropPosition.Before) || service.CanDrop(filter, node, DropPosition.After), $"{node.Header} is a neighbor but takes no drop.");
            }
        }

        [Fact]
        public void ShowDragLook_WithTheSameLook_RaisesNothing()
        {
            var scene = CreateScene();
            var node = scene.Entity2;
            node.ShowDragLook(new DragLook(false, false, true, false, null));
            var changes = new List<string?>();
            node.PropertyChanged += (_, e) => changes.Add(e.PropertyName);

            node.ShowDragLook(new DragLook(false, false, true, false, null));

            Assert.Empty(changes);
        }

        [Fact]
        public void ShowDragLook_WithANewValue_RaisesOnlyTheChangedProperties()
        {
            var scene = CreateScene();
            var node = scene.Entity2;
            var changes = new List<string?>();
            node.PropertyChanged += (_, e) => changes.Add(e.PropertyName);

            node.ShowDragLook(DragLook.None);

            Assert.Equal(new[] { nameof(TreeNode.IsDragActive), nameof(TreeNode.IsDimmed) }, changes);
        }
    }
}
