// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Compositing;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Texturing.Filters;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    public class ListTreeSlotsTests
    {
        private readonly TreeDropService service = new();

        // Layer 1 of A has two entities with modifiers, a camera and a filter. Layer 2 of A has only a filter.
        // B has an empty layer and a layer with one entity. Every row is open.
        private static Scene CreateOpenScene()
        {
            var scene = CreateScene([2, 2], Fill);
            foreach (var node in scene.All())
                node.IsExpanded = true;

            return scene;
        }

        private static void Fill(Scene scene)
        {
            var layer1 = scene.LayerOfA(0);
            layer1.ModelEntityManager.AddItem(typeof(Entity));
            layer1.ModelEntityManager.AddItem(typeof(Entity));
            layer1.CameraManager.AddItem(typeof(Camera));
            layer1.TextureModifierManager.AddItem(typeof(Blur));
            var first = (Entity)layer1.ModelEntityManager.ManagerData.Items[0];
            first.ModifierManager.AddItem(typeof(TranslateModifier));
            first.ModifierManager.AddItem(typeof(ScaleModifier));
            var second = (Entity)layer1.ModelEntityManager.ManagerData.Items[1];
            second.ModifierManager.AddItem(typeof(TranslateModifier));
            scene.LayerOfA(1).TextureModifierManager.AddItem(typeof(Blur));
            LayerOfB(scene, 1).ModelEntityManager.AddItem(typeof(Entity));
        }

        // The second entity of layer 1 of A shows under layer 2 of A too. Every row is open.
        private static Scene CreateOpenSharedScene()
        {
            var scene = CreateScene([2, 2], ShareAnEntity);
            foreach (var node in scene.All())
                node.IsExpanded = true;

            return scene;
        }

        private static void ShareAnEntity(Scene scene)
        {
            Fill(scene);
            scene.LayerOfA(1).ModelEntityManager.AddExistingItem((IPrefab)scene.LayerOfA(0).ModelEntityManager.ManagerData.Items[1]);
        }

        // The drop into the modifiers of the shared entity gives its second row a child with no node.
        private ListTreeSnapshot MovedIntoTheSharedEntity(Scene scene, TreeNode source)
        {
            var live = ListTreeSnapshot.Of(scene.Tree.Roots);
            var place = new ListPlace(((Entity)Entity(scene, 0, 1).Item!).ModifierManager, 1);
            var slot = new ListTreeSlots(service).Of(live, source).Single(found => found.ListPlace == place);
            return ListTreeMove.Of(live, live.VisibleRows().Single(row => ReferenceEquals(row.Node, source)), slot);
        }

        private static IEnumerable<ListTreeRow> AllRows(IEnumerable<ListTreeRow> rows) =>
            rows.SelectMany(row => AllRows(row.Children).Prepend(row));

        private static Layer LayerOfB(Scene scene, int index) => (Layer)scene.B.LayerManager.ManagerData.Items[index];

        private static TreeNode Entity(Scene scene, int layer, int index) => scene.Group<EntitiesGroup>(layer).Children[index];

        private static TreeNode Modifier(Scene scene, int entity, int index) => Scene.Group<ModifiersGroup>(Entity(scene, 0, entity)).Children[index];

        private IReadOnlyList<ListTreeSlot> SlotsOf(Scene scene, TreeNode source) =>
            new ListTreeSlots(service).Of(ListTreeSnapshot.Of(scene.Tree.Roots), source);

        [Fact]
        public void Of_AnEntity_GivesASlotInEveryOpenLayerAndInAnEmptyEntitiesList()
        {
            var scene = CreateOpenScene();

            var slots = SlotsOf(scene, Entity(scene, 0, 0));

            var lists = slots.Select(slot => slot.ListPlace.List).Distinct().ToArray();
            Assert.Contains(scene.LayerOfA(0).ModelEntityManager, lists);
            Assert.Contains(scene.LayerOfA(1).ModelEntityManager, lists);
            Assert.Contains(LayerOfB(scene, 0).ModelEntityManager, lists);
            Assert.Contains(LayerOfB(scene, 1).ModelEntityManager, lists);
            Assert.Contains(new ListTreeSlot(scene.LayerNodeOfA(1), DropPosition.Inside, new ListPlace(scene.LayerOfA(1).ModelEntityManager, 0), ListTreeKeys.Of(scene.LayerNodeOfA(1))), slots);
        }

        [Fact]
        public void Of_AModifierOverAnEntityWithNoModifiers_GivesInsideItsHiddenModifiersList()
        {
            var scene = CreateOpenScene();
            var entity = Scene.Group<EntitiesGroup>(scene.LayerNodeOfB(1)).Children[0];
            var modifiers = Scene.Group<ModifiersGroup>(entity);
            var place = new ListPlace(((Entity)entity.Item!).ModifierManager, 0);

            var slots = SlotsOf(scene, Modifier(scene, 1, 0));

            Assert.Null(service.PlaceOf(Modifier(scene, 1, 0), entity, DropPosition.Inside));
            Assert.Contains(new ListTreeSlot(modifiers, DropPosition.Inside, place, ListTreeKeys.Of(entity)), slots);
        }

        [Fact]
        public void Of_AFilterOverAnOpenComposition_GivesInsideItsHiddenFiltersList()
        {
            var scene = CreateOpenScene();
            var filters = Scene.Group<FiltersGroup>(scene.NodeA);
            var place = new ListPlace(scene.A.TextureModifierManager, 0);

            var slots = SlotsOf(scene, Scene.Group<FiltersGroup>(scene.LayerNodeOfA(1)).Children[0]);

            Assert.Contains(new ListTreeSlot(filters, DropPosition.Inside, place, ListTreeKeys.Of(scene.NodeA)), slots);
        }

        [Fact]
        public void Of_AClosedCardWithRows_GivesNoSlotInsideIt()
        {
            var scene = CreateOpenScene();
            scene.LayerNodeOfA(1).IsExpanded = false;

            var slots = SlotsOf(scene, Entity(scene, 0, 0));

            Assert.DoesNotContain(slots, slot => ReferenceEquals(slot.ListPlace.List, scene.LayerOfA(1).ModelEntityManager));
            Assert.DoesNotContain(slots, slot => slot.Target.IsOrIsUnder(scene.LayerNodeOfA(1)));
        }

        [Fact]
        public void Of_ALayerNextToAClosedCardWithRows_GivesTheSlotsBeforeAndAfterIt()
        {
            var scene = CreateOpenScene();
            var closed = scene.LayerNodeOfA(1);
            closed.IsExpanded = false;

            var slots = SlotsOf(scene, scene.LayerNodeOfB(0));

            Assert.Contains(new ListTreeSlot(closed, DropPosition.Before, new ListPlace(scene.A.LayerManager, 1), ListTreeKeys.Of(closed)), slots);
            Assert.Contains(new ListTreeSlot(closed, DropPosition.After, new ListPlace(scene.A.LayerManager, 2), ListTreeKeys.Of(closed)), slots);
        }

        [Fact]
        public void Of_ACardWithRows_GivesNoSlotInsideTheSource()
        {
            var scene = CreateOpenScene();
            var source = scene.LayerNodeOfA(0);

            var slots = SlotsOf(scene, source);

            var own = Assert.Single(slots, slot => slot.Target.IsOrIsUnder(source));
            Assert.Same(source, own.Target);
            Assert.DoesNotContain(slots, slot => ReferenceEquals(slot.ListPlace.List, scene.LayerOfA(0).ModelEntityManager));
        }

        [Fact]
        public void Of_AnEntity_KeepsTheCurrentSlotOnce()
        {
            var scene = CreateOpenScene();
            var source = Entity(scene, 0, 1);
            var current = new ListPlace(scene.LayerOfA(0).ModelEntityManager, 1);

            var slots = SlotsOf(scene, source);

            Assert.Equal(new ListTreeSlot(source, DropPosition.Before, current, ListTreeKeys.Of(source)), Assert.Single(slots, slot => slot.ListPlace == current));
        }

        [Fact]
        public void Of_SlotsWithOneListPlace_KeepTheDeepestRow()
        {
            var scene = CreateOpenScene();
            var layer = scene.LayerNodeOfB(1);
            var end = new ListPlace(LayerOfB(scene, 1).ModelEntityManager, 1);

            var slots = SlotsOf(scene, Entity(scene, 0, 0));

            Assert.True(service.CanDrop(Entity(scene, 0, 0), layer, DropPosition.Inside));
            Assert.Equal(slots.Count, slots.Select(slot => slot.ListPlace).Distinct().Count());
            var last = Scene.Group<EntitiesGroup>(layer).Children[0];
            Assert.Equal(new ListTreeSlot(last, DropPosition.After, end, ListTreeKeys.Of(last)), Assert.Single(slots, slot => slot.ListPlace == end));
        }

        [Fact]
        public void Of_AfterARowAndBeforeTheNext_KeepsBefore()
        {
            var scene = CreateOpenScene();
            var translate = Modifier(scene, 0, 0);
            var scale = Modifier(scene, 0, 1);

            var slots = SlotsOf(scene, Modifier(scene, 1, 0));

            Assert.True(service.CanDrop(Modifier(scene, 1, 0), translate, DropPosition.After));
            Assert.DoesNotContain(slots, slot => ReferenceEquals(slot.Target, translate) && slot.Position == DropPosition.After);
            Assert.Contains(slots, slot => ReferenceEquals(slot.Target, scale) && slot.Position == DropPosition.Before);
        }

        [Fact]
        public void Of_AComposition_GivesNoSlotInsideAnEntity()
        {
            var scene = CreateOpenScene();
            var entity = Entity(scene, 0, 0);

            var slots = SlotsOf(scene, scene.NodeA);

            Assert.False(service.CanDrop(scene.NodeA, entity, DropPosition.Inside));
            Assert.DoesNotContain(slots, slot => ReferenceEquals(slot.Target, entity));
            Assert.All(slots, slot => Assert.Same(scene.Project.CompositionManager, slot.ListPlace.List));
        }

        [Fact]
        public void Of_ASnapshotWithARowWithNoNode_SkipsThatRow()
        {
            var scene = CreateOpenSharedScene();
            var source = Modifier(scene, 0, 0);
            var moved = MovedIntoTheSharedEntity(scene, source);
            var noNode = AllRows(moved.Roots).Single(row => row.Node == null);

            var slots = new ListTreeSlots(service).Of(moved, source);

            Assert.NotEmpty(slots);
            Assert.DoesNotContain(slots, slot => slot.ShownTarget.Equals(noNode.Key));
        }

        [Fact]
        public void Of_AnOpenTree_GivesEverySlotOfTreeDropSlots()
        {
            var scene = CreateOpenScene();
            var sources = new[]
            {
                scene.NodeA, scene.LayerNodeOfA(0), Entity(scene, 0, 0), Modifier(scene, 1, 0),
                Scene.Group<CamerasGroup>(scene.LayerNodeOfA(0)).Children[0], Scene.Group<FiltersGroup>(scene.LayerNodeOfA(1)).Children[0]
            };

            foreach (var source in sources)
            {
                var mine = SlotsOf(scene, source).Select(slot => slot.ListPlace).ToHashSet();
                var current = new ListPlace(source.Owner!, source.Owner!.ManagerData.Items.IndexOf(source.Item!));
                var theirs = new TreeDropSlots(scene.Tree, source).Slots
                    .Select(slot => slot.IsStart ? current : service.PlaceOf(source, slot.Target, slot.Position)!.Value)
                    .ToHashSet();

                Assert.True(theirs.SetEquals(mine), source.Header);
            }
        }
    }
}
