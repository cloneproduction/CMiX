using CMiX.Core.Compositing;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Texturing.Filters;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // The drop slots of a source: the places that the mouse drop accepts, in the order of the rows that show, each place once.
    public class TreeDropSlotsTests
    {
        // Composition A: layer 1 has two entities with modifiers, a camera and two filters. Layer 2 has one entity and one filter.
        // B has one layer with one entity.
        private static Scene CreateScene(bool expand = true)
        {
            var scene = TreeFixtures.CreateScene(fill: Fill);
            if (expand)
                foreach (var node in scene.All())
                    node.IsExpanded = true;

            return scene;
        }

        private static void Fill(Scene scene)
        {
            var layer1 = scene.LayerOfA(0);
            var layer2 = scene.LayerOfA(1);
            var layerOfB = (Layer)scene.B.LayerManager.ManagerData.Items[0];
            layer1.ModelEntityManager.AddItem(typeof(Entity));
            layer1.ModelEntityManager.AddItem(typeof(Entity));
            layer1.CameraManager.AddItem(typeof(Camera));
            layer2.ModelEntityManager.AddItem(typeof(Entity));
            layerOfB.ModelEntityManager.AddItem(typeof(Entity));
            var first = (Entity)layer1.ModelEntityManager.ManagerData.Items[0];
            first.ModifierManager.AddItem(typeof(TranslateModifier));
            first.ModifierManager.AddItem(typeof(ScaleModifier));
            var second = (Entity)layer1.ModelEntityManager.ManagerData.Items[1];
            second.ModifierManager.AddItem(typeof(TranslateModifier));
            layer1.TextureModifierManager.AddItem(typeof(Blur));
            layer1.TextureModifierManager.AddItem(typeof(Blur));
            layer2.TextureModifierManager.AddItem(typeof(Blur));
        }

        private static TreeNode Entity(Scene scene, int layer, int index) => scene.Group<EntitiesGroup>(layer).Children[index];

        private static TreeNode Modifier(Scene scene, int entity, int index) => Scene.Group<ModifiersGroup>(Entity(scene, 0, entity)).Children[index];

        // The rows that show, from top to bottom.
        private static List<TreeNode> ShownRows(IEnumerable<TreeNode> nodes) =>
            nodes.SelectMany(node => new[] { node }.Concat(node.IsExpanded ? ShownRows(node.Children) : [])).ToList();

        // The height of a slot among the rows: Before a row is above it, Inside is on it, After a block is under its last shown row.
        private static int HeightOf(DropSlot slot, List<TreeNode> rows) => slot.Position switch
        {
            DropPosition.Before => 3 * rows.IndexOf(slot.Target),
            DropPosition.Inside => 3 * rows.IndexOf(slot.Target) + 1,
            _ => 3 * rows.IndexOf(TreeRowOrder.LastVisible(slot.Target)) + 2
        };

        // The checks that every list must pass: one start slot at the source, every other slot accepted by CanDrop, no slot in the
        // block of the source, the order of the rows, each slot once, and every accepted place found by IndexOf.
        private static TreeDropSlots CheckSlots(Scene scene, TreeNode source)
        {
            scene.Tree.BeginDrag(source);
            var slots = new TreeDropSlots(scene.Tree, source);
            var rows = ShownRows(scene.Tree.Roots);

            Assert.Single(slots.Slots, slot => slot.IsStart);
            Assert.Equal(new DropSlot(source, DropPosition.Before, true), slots.Slots[slots.StartIndex]);
            Assert.All(slots.Slots.Where(slot => !slot.IsStart), slot =>
            {
                Assert.True(scene.Tree.CanDrop(source, slot.Target, slot.Position), $"{slot.Target.Header} {slot.Position}");
                Assert.False(slot.Target.IsOrIsUnder(source));
                Assert.Contains(slot.Target, rows);
            });

            var heights = slots.Slots.Select(slot => HeightOf(slot, rows)).ToList();
            Assert.Equal(heights.OrderBy(height => height), heights);
            Assert.Equal(slots.Slots.Count, slots.Slots.Distinct().Count());

            foreach (var row in rows)
                foreach (var position in new[] { DropPosition.Before, DropPosition.Inside, DropPosition.After })
                    if (scene.Tree.CanDrop(source, row, position))
                        Assert.True(slots.IndexOf(row, position) >= 0, $"{row.Header} {position}");

            return slots;
        }

        [Fact]
        public void New_AComposition_ListsOnlyTheAcceptedPlacesInRowOrder()
        {
            var scene = CreateScene();

            var slots = CheckSlots(scene, scene.NodeA);

            // After B is the only move: Before B is the place of A.
            Assert.Equal(new[] { new DropSlot(scene.NodeA, DropPosition.Before, true), new DropSlot(scene.NodeB, DropPosition.After) }, slots.Slots);
        }

        [Fact]
        public void New_ALayer_ListsOnlyTheAcceptedPlacesInRowOrder()
        {
            var scene = CreateScene();
            var layer1 = scene.LayerNodeOfA(0);

            var slots = CheckSlots(scene, layer1);

            Assert.Contains(new DropSlot(scene.LayerNodeOfA(1), DropPosition.After), slots.Slots);
            Assert.Contains(new DropSlot(scene.LayerNodeOfB(0), DropPosition.Before), slots.Slots);
        }

        [Fact]
        public void New_AnEntity_ListsOnlyTheAcceptedPlacesInRowOrder()
        {
            var scene = CreateScene();

            var slots = CheckSlots(scene, Entity(scene, 0, 0));

            Assert.Contains(new DropSlot(Entity(scene, 0, 1), DropPosition.After), slots.Slots);
            Assert.Contains(new DropSlot(Entity(scene, 1, 0), DropPosition.Before), slots.Slots);
        }

        [Fact]
        public void New_AFilter_ListsOnlyTheAcceptedPlacesInRowOrder()
        {
            var scene = CreateScene();

            var slots = CheckSlots(scene, Scene.Group<FiltersGroup>(scene.LayerNodeOfA(0)).Children[0]);

            Assert.Contains(new DropSlot(Scene.Group<FiltersGroup>(scene.LayerNodeOfA(1)).Children[0], DropPosition.Before), slots.Slots);
        }

        [Fact]
        public void New_AModifier_ListsOnlyTheAcceptedPlacesInRowOrder()
        {
            var scene = CreateScene();

            var slots = CheckSlots(scene, Modifier(scene, 1, 0));

            Assert.Contains(new DropSlot(Modifier(scene, 0, 0), DropPosition.Before), slots.Slots);
        }

        [Fact]
        public void New_ACamera_ListsOnlyTheAcceptedPlacesInRowOrder()
        {
            var scene = CreateScene();

            CheckSlots(scene, Scene.Group<CamerasGroup>(scene.LayerNodeOfA(0)).Children[0]);
        }

        [Fact]
        public void New_AGroup_ListsNoSlot()
        {
            var scene = CreateScene();

            var slots = new TreeDropSlots(scene.Tree, scene.Group<EntitiesGroup>(0));

            Assert.Empty(slots.Slots);
            Assert.Equal(-1, slots.StartIndex);
            Assert.Equal(-1, slots.Next(-1));
            Assert.Equal(-1, slots.Previous(-1));
        }

        // After a row and Before the next row are one place: the list keeps Before.
        [Fact]
        public void New_AfterARowAndBeforeTheNext_KeepsBefore()
        {
            var scene = CreateScene();
            var source = Modifier(scene, 1, 0);
            var translate = Modifier(scene, 0, 0);
            var scale = Modifier(scene, 0, 1);

            var slots = CheckSlots(scene, source);

            Assert.True(scene.Tree.CanDrop(source, translate, DropPosition.After));
            Assert.DoesNotContain(new DropSlot(translate, DropPosition.After), slots.Slots);
            Assert.Contains(new DropSlot(scale, DropPosition.Before), slots.Slots);
            Assert.Equal(slots.Slots.ToList().IndexOf(new DropSlot(scale, DropPosition.Before)), slots.IndexOf(translate, DropPosition.After));
        }

        // Inside an open list with rows and After its last row are one place: the list keeps the bar After the last row.
        [Fact]
        public void New_InsideAnOpenRow_KeepsTheBarAfterItsLastRow()
        {
            var scene = CreateScene();
            var source = Entity(scene, 0, 0);
            var layer2 = scene.LayerNodeOfA(1);
            var entities2 = scene.Group<EntitiesGroup>(1);
            var last = Entity(scene, 1, 0);

            var slots = CheckSlots(scene, source);

            Assert.True(scene.Tree.CanDrop(source, layer2, DropPosition.Inside));
            Assert.True(scene.Tree.CanDrop(source, entities2, DropPosition.Inside));
            Assert.DoesNotContain(slots.Slots, slot => slot.Position == DropPosition.Inside && (slot.Target == layer2 || slot.Target == entities2));
            var after = slots.Slots.ToList().IndexOf(new DropSlot(last, DropPosition.After));
            Assert.True(after >= 0);
            Assert.Equal(after, slots.IndexOf(layer2, DropPosition.Inside));
            Assert.Equal(after, slots.IndexOf(entities2, DropPosition.Inside));
        }

        // A closed row shows no rows under it, so Inside is its only place. The drop opens the row.
        [Fact]
        public void New_AClosedRow_KeepsItsInsideSlot()
        {
            var scene = CreateScene();
            var source = Entity(scene, 0, 0);
            var layer2 = scene.LayerNodeOfA(1);
            layer2.IsExpanded = false;

            var slots = CheckSlots(scene, source);

            Assert.Contains(new DropSlot(layer2, DropPosition.Inside), slots.Slots);
            Assert.DoesNotContain(slots.Slots, slot => slot.Target.IsOrIsUnder(layer2) && slot.Target != layer2);
        }

        // The rows under the source are no target: the composition goes nowhere inside itself.
        [Fact]
        public void New_ASourceWithRows_HasNoSlotInItsBlock()
        {
            var scene = CreateScene();

            var slots = CheckSlots(scene, scene.LayerNodeOfA(0));

            Assert.DoesNotContain(slots.Slots, slot => !slot.IsStart && slot.Target.IsOrIsUnder(scene.LayerNodeOfA(0)));
        }

        // The places just before and just after the source move nothing: they give the start slot.
        [Fact]
        public void IndexOf_APlaceThatMovesNothing_GivesTheStartSlot()
        {
            var scene = CreateScene();
            var source = scene.LayerNodeOfA(0);

            var slots = CheckSlots(scene, source);

            Assert.Equal(slots.StartIndex, slots.IndexOf(source, DropPosition.After));
            Assert.Equal(slots.StartIndex, slots.IndexOf(scene.LayerNodeOfA(1), DropPosition.Before));
            Assert.Equal(-1, slots.IndexOf(scene.Group<EntitiesGroup>(0), DropPosition.Inside));
        }

        [Fact]
        public void NextAndPrevious_AtTheEnds_StopAtTheEdges()
        {
            var scene = CreateScene();
            var slots = CheckSlots(scene, Entity(scene, 0, 0));
            var last = slots.Slots.Count - 1;

            Assert.True(slots.Slots.Count > 2);
            Assert.Equal(slots.StartIndex + 1, slots.Next(slots.StartIndex));
            Assert.Equal(0, slots.Previous(0));
            Assert.Equal(last, slots.Next(last));
            Assert.Equal(last - 1, slots.Previous(last));
            Assert.Equal(1, slots.Next(0));
            Assert.Equal(0, slots.Next(-1));
            Assert.Equal(last, slots.Previous(-1));
        }
    }
}
