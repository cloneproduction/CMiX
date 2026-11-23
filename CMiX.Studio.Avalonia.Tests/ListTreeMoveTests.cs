// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core;
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
    public class ListTreeMoveTests
    {
        // Layer 1 of A has two entities with modifiers, a camera and a filter. Layer 2 of A has only a filter.
        // B has an empty layer that is closed and a layer with one entity. Every empty list is closed.
        private static Scene CreateScene()
        {
            var scene = TreeFixtures.CreateScene([2, 2], Fill);
            foreach (var node in scene.All())
                node.IsExpanded = node.Group == null || node.Children.Count > 0;

            scene.LayerNodeOfB(0).IsExpanded = false;
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

        // The second entity of layer 1 of A shows under layer 2 of A too.
        private static Scene CreateSharedScene()
        {
            var scene = TreeFixtures.CreateScene([2, 2], ShareAnEntity);
            foreach (var node in scene.All())
                node.IsExpanded = node.Group == null || node.Children.Count > 0;

            scene.LayerNodeOfA(1).IsExpanded = false;
            return scene;
        }

        private static void ShareAnEntity(Scene scene)
        {
            Fill(scene);
            scene.LayerOfA(1).ModelEntityManager.AddExistingItem((IPrefab)scene.LayerOfA(0).ModelEntityManager.ManagerData.Items[1]);
        }

        private static Layer LayerOfB(Scene scene, int index) => (Layer)scene.B.LayerManager.ManagerData.Items[index];

        private static TreeNode Entity(Scene scene, int index) => scene.Group<EntitiesGroup>(0).Children[index];

        private static TreeNode Modifier(Scene scene, int entity, int index) => Scene.Group<ModifiersGroup>(Entity(scene, entity)).Children[index];

        private static TreeNode EntityOfB(Scene scene) => Scene.Group<EntitiesGroup>(scene.LayerNodeOfB(1)).Children[0];

        private static IReadOnlyList<ListTreeSlot> SlotsOf(Scene scene, TreeNode source) =>
            new ListTreeSlots(new TreeDropService()).Of(ListTreeSnapshot.Of(scene.Tree.Roots), source);

        private static ListTreeSnapshot MoveOf(Scene scene, TreeNode source, ListTreeSlot slot)
        {
            var snapshot = ListTreeSnapshot.Of(scene.Tree.Roots);
            return ListTreeMove.Of(snapshot, RowOf(snapshot, source), slot);
        }

        private static ListTreeSlot InsideSlot(TreeNode source, TreeNode card) =>
            new(card, DropPosition.Inside, new TreeDropService().PlaceOf(source, card, DropPosition.Inside)!.Value, ListTreeKeys.Of(card));

        private static ListTreeSlot SlotAt(Scene scene, TreeNode source, ListPlace place) =>
            SlotsOf(scene, source).Single(slot => slot.ListPlace == place);

        private static ListTreeRow RowOf(ListTreeSnapshot snapshot, TreeNode node) =>
            AllRows(snapshot.Roots).Single(row => ReferenceEquals(row.Node, node));

        private static ListTreeRow RowOf(ListTreeSnapshot snapshot, ListTreeRowKey key) =>
            AllRows(snapshot.Roots).Single(row => row.Key.Equals(key));

        private static IEnumerable<ListTreeRow> AllRows(IEnumerable<ListTreeRow> rows) =>
            rows.SelectMany(row => AllRows(row.Children).Prepend(row));

        private sealed record Shown(ListTreeRowKey Key, int Depth);

        // The real tree opens the rows of a moved card. The new view restores them after the drop.
        private static Shown[] ShownRows(ListTreeSnapshot snapshot, IControl moved) =>
            snapshot.VisibleRows().Where(row => !IsUnder(row.Key.Parent, moved)).Select(row => new Shown(row.Key, row.Depth)).ToArray();

        private static bool IsUnder(ListTreeRowKey? key, IControl moved)
        {
            for (; key != null; key = key.Parent)
                if (ReferenceEquals(key.Model, moved))
                    return true;

            return false;
        }

        private static async Task<int> AssertEverySlotGivesTheRowsOfTheRealDrop(Func<Scene> create, Func<Scene, TreeNode> pickSource)
        {
            var first = create();
            var count = SlotsOf(first, pickSource(first)).Count;
            for (var index = 0; index < count; index++)
            {
                var scene = create();
                var source = pickSource(scene);
                var slot = SlotsOf(scene, source)[index];
                var moved = source.Item!;

                var expected = ShownRows(MoveOf(scene, source, slot), moved);
                await scene.Tree.DropAsync(source, slot.Target, slot.Position);

                Assert.Equal(expected, ShownRows(ListTreeSnapshot.Of(scene.Tree.Roots), moved));
            }

            return count;
        }

        public static TheoryData<string> Sources => new() { "composition", "layer", "layer of B", "entity", "entity of B", "modifier", "camera", "filter" };

        private static TreeNode SourceOf(Scene scene, string name) => name switch
        {
            "composition" => scene.NodeA,
            "layer" => scene.LayerNodeOfA(0),
            "layer of B" => scene.LayerNodeOfB(1),
            "entity" => Entity(scene, 0),
            "entity of B" => EntityOfB(scene),
            "modifier" => Modifier(scene, 1, 0),
            "camera" => scene.Group<CamerasGroup>(0).Children[0],
            _ => scene.Group<FiltersGroup>(1).Children[0]
        };

        [Theory]
        [MemberData(nameof(Sources))]
        public async Task Of_EverySlotOfTheSource_GivesTheRowsOfTheRealDrop(string name)
        {
            var count = await AssertEverySlotGivesTheRowsOfTheRealDrop(CreateScene, scene => SourceOf(scene, name));

            Assert.True(count > 1, name);
        }

        public static TheoryData<string> SharedSources => new() { "modifier of the shared entity", "modifier", "entity" };

        private static TreeNode SharedSourceOf(Scene scene, string name) => name switch
        {
            "modifier of the shared entity" => Modifier(scene, 1, 0),
            "modifier" => Modifier(scene, 0, 1),
            _ => Entity(scene, 0)
        };

        [Theory]
        [MemberData(nameof(SharedSources))]
        public async Task Of_EverySlotOfATreeWithASharedEntity_GivesTheRowsOfTheRealDrop(string name)
        {
            var count = await AssertEverySlotGivesTheRowsOfTheRealDrop(CreateSharedScene, scene => SharedSourceOf(scene, name));

            Assert.True(count > 1, name);
        }

        [Fact]
        public void Of_ADropIntoAClosedCard_OpensEveryRowOnThePath()
        {
            var scene = CreateScene();
            var card = scene.LayerNodeOfB(0);

            var moved = MoveOf(scene, Entity(scene, 0), InsideSlot(Entity(scene, 0), card));

            var entities = RowOf(moved, new ListTreeRowKey(LayerOfB(scene, 0).ModelEntityManager, ListTreeKeys.Of(card)));
            Assert.True(entities.IsExpanded);
            Assert.True(RowOf(moved, ListTreeKeys.Of(card)).IsExpanded);
            Assert.Contains(moved.VisibleRows(), row => ReferenceEquals(row.Node, Entity(scene, 0)));
        }

        [Fact]
        public void Of_ADropIntoAHiddenList_MakesTheGroupRowAppearWithTheSourceAsItsOnlyRow()
        {
            var scene = CreateScene();
            var source = Scene.Group<FiltersGroup>(scene.LayerNodeOfA(1)).Children[0];

            var moved = MoveOf(scene, source, SlotAt(scene, source, new ListPlace(scene.A.TextureModifierManager, 0)));

            var composition = RowOf(moved, scene.NodeA);
            Assert.Equal(new[] { "Layers", "Filters" }, composition.Children.Select(row => row.Node!.Header));
            var filters = composition.Children[1];
            Assert.Same(Scene.Group<FiltersGroup>(scene.NodeA), filters.Node);
            Assert.Equal(2, filters.Depth);
            Assert.Same(source, Assert.Single(filters.Children).Node);
            Assert.Equal(3, filters.Children[0].Depth);
        }

        [Fact]
        public void Of_AnInsideDrop_AppendsAtTheEndOfTheList()
        {
            var scene = CreateScene();
            var source = EntityOfB(scene);

            var moved = MoveOf(scene, source, InsideSlot(source, scene.LayerNodeOfA(0)));

            var entities = RowOf(moved, scene.Group<EntitiesGroup>(0));
            Assert.Equal(3, entities.Children.Count);
            Assert.Same(source, entities.Children[^1].Node);
        }

        [Fact]
        public void Of_TheLastRowOfAListMovesOut_LeavesTheListOut()
        {
            var scene = CreateScene();
            var source = EntityOfB(scene);

            var moved = MoveOf(scene, source, InsideSlot(source, scene.LayerNodeOfA(0)));

            Assert.Empty(RowOf(moved, scene.LayerNodeOfB(1)).Children);
            Assert.DoesNotContain(AllRows(moved.Roots), row => ReferenceEquals(row.Node, Scene.Group<EntitiesGroup>(scene.LayerNodeOfB(1))));
        }

        [Fact]
        public void Of_ACardThatMoves_KeepsItsNodeAndItsRowsAtTheirNewDepthAndShowsClosed()
        {
            var scene = CreateScene();
            var source = scene.LayerNodeOfA(0);
            var before = RowOf(ListTreeSnapshot.Of(scene.Tree.Roots), source);
            var slot = SlotAt(scene, source, new ListPlace(scene.B.LayerManager, 2));

            var moved = RowOf(MoveOf(scene, source, slot), source);

            Assert.False(moved.IsExpanded);
            Assert.Equal(new ListTreeRowKey(source.Item!, ListTreeKeys.Of(Scene.LayersOf(scene.NodeB))), moved.Key);
            Assert.Equal(AllRows(before.Children).Select(row => row.Node), AllRows(moved.Children).Select(row => row.Node));
            Assert.Equal(AllRows(before.Children).Select(row => row.IsExpanded), AllRows(moved.Children).Select(row => row.IsExpanded));
            Assert.All(moved.Children, row => Assert.Equal(4, row.Depth));
        }

        [Fact]
        public void Of_ADropIntoAListThatShowsTwice_AddsTheRowAtBothPlaces()
        {
            var scene = CreateSharedScene();
            var source = Modifier(scene, 0, 0);
            var shared = Entity(scene, 1);

            var moved = MoveOf(scene, source, SlotAt(scene, source, new ListPlace(((Entity)shared.Item!).ModifierManager, 1)));

            var rows = AllRows(moved.Roots).Where(row => ReferenceEquals(row.Key.Model, source.Item)).ToArray();
            Assert.Equal(2, rows.Length);
            Assert.Same(source, rows[0].Node);
            Assert.Null(rows[1].Node);
            Assert.Same(scene.LayerOfA(1).ModelEntityManager, rows[1].Key.Parent!.Parent!.Parent!.Model);
            Assert.True(RowOf(moved, scene.LayerNodeOfA(1)).IsExpanded);
        }
    }
}
