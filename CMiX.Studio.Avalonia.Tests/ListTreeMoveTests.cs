// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering.Lights;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.ListTreeScenes;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // The move of one row against the real drop. ListTreeMoveBlockTests moves a block.
    public class ListTreeMoveTests
    {
        // Layer 1 of A has two entities with modifiers, a camera and a filter. Layer 2 of A has only a filter.
        // B has an empty layer that is closed and a layer with one entity. Every empty list is closed.
        internal static Scene CreateScene()
        {
            var scene = Opened(TreeFixtures.CreateScene([2, 2], Fill));
            scene.LayerNodeOfB(0).IsExpanded = false;
            return scene;
        }

        // The second entity of layer 1 of A shows under layer 2 of A too.
        internal static Scene CreateSharedScene()
        {
            var scene = Opened(TreeFixtures.CreateScene([2, 2], ShareAnEntity));
            scene.LayerNodeOfA(1).IsExpanded = false;
            return scene;
        }

        // The entity of B, which has no modifiers, shows under both layers of B.
        private static Scene CreateSharedCardScene() => Opened(TreeFixtures.CreateScene([2, 2], ShareTheEntityOfB));

        private static void ShareTheEntityOfB(Scene scene)
        {
            Fill(scene);
            LayerOfB(scene, 0).ModelEntityManager.AddExistingItem((IPrefab)LayerOfB(scene, 1).ModelEntityManager.ManagerData.Items[0]);
        }

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

        internal static ListTreeRow RowOf(ListTreeSnapshot snapshot, TreeNode node) =>
            AllRows(snapshot.Roots).Single(row => ReferenceEquals(row.Node, node));

        internal static ListTreeRow RowOf(ListTreeSnapshot snapshot, ListTreeRowKey key) =>
            AllRows(snapshot.Roots).Single(row => row.Key.Equals(key));

        internal sealed record Shown(ListTreeRowKey Key, int Depth);

        // The real tree opens the rows of a moved card. The view restores them after the drop.
        private static Shown[] ShownRows(ListTreeSnapshot snapshot, IControl moved) =>
            snapshot.VisibleRows().Where(row => !IsUnder(row.Key.Parent, moved)).Select(row => new Shown(row.Key, row.Depth)).ToArray();

        internal static bool IsUnder(ListTreeRowKey? key, IControl moved)
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

        private static readonly string[] AllSources =
            { "composition", "layer", "layer of B", "entity", "entity of B", "modifier", "first modifier", "second modifier", "camera", "filter" };

        public static TheoryData<string, string> ScenesAndSources
        {
            get
            {
                var data = new TheoryData<string, string>();
                foreach (var scene in new[] { "plain", "closed", "shared entity", "shared card" })
                    foreach (var source in AllSources)
                        data.Add(scene, source);

                return data;
            }
        }

        // The entity with two modifiers, layer 2 of A and composition B are closed.
        internal static Scene CreateClosedScene()
        {
            var scene = CreateScene();
            Entity(scene, 0).IsExpanded = false;
            scene.LayerNodeOfA(1).IsExpanded = false;
            scene.NodeB.IsExpanded = false;
            return scene;
        }

        internal static Scene SceneOf(string name) => name switch
        {
            "closed" => CreateClosedScene(),
            "shared entity" => CreateSharedScene(),
            "shared card" => CreateSharedCardScene(),
            _ => CreateScene()
        };

        private static TreeNode AnySourceOf(Scene scene, string name) => name switch
        {
            "first modifier" => Modifier(scene, 0, 0),
            "second modifier" => Modifier(scene, 0, 1),
            _ => SourceOf(scene, name)
        };

        private static ListTreePlace? PlaceByMoveOf(ListTreeSnapshot snapshot, ListTreeRow source, ListTreeSlot slot)
        {
            var moved = ListTreeMove.Of(snapshot, source, slot);
            return moved.RowOf(source.Node!) is { } row ? ListTreeLayout.Of(moved, ListTreeSizes.Default).PlaceOf(row.Key) : null;
        }

        [Theory]
        [MemberData(nameof(ScenesAndSources))]
        public void PlaceOf_EverySlotOfTheSource_EqualsThePlaceInTheLayoutOfTheMove(string sceneName, string sourceName)
        {
            var scene = SceneOf(sceneName);
            var source = AnySourceOf(scene, sourceName);
            var snapshot = ListTreeSnapshot.Of(scene.Tree.Roots);
            var row = RowOf(snapshot, source);
            var slots = SlotsOf(scene, source);

            var places = slots.Select(ListTreeCountedTree.Of(snapshot, row, ListTreeSizes.Default).PlaceOf).ToArray();

            Assert.NotEmpty(slots);
            Assert.Equal(slots.Select(slot => PlaceByMoveOf(snapshot, row, slot)), places);
        }

        [Fact]
        public void Of_AMoveInsideOneComposition_KeepsTheRowsOfTheOtherCompositionAsTheSameObjects()
        {
            var scene = CreateScene();
            var source = Entity(scene, 0);
            var snapshot = ListTreeSnapshot.Of(scene.Tree.Roots);

            var moved = ListTreeMove.Of(snapshot, RowOf(snapshot, source), InsideSlot(source, scene.LayerNodeOfA(1)));

            Assert.Same(RowOf(snapshot, scene.NodeB), RowOf(moved, ListTreeKeys.Of(scene.NodeB)));
            Assert.NotSame(RowOf(snapshot, scene.NodeA), RowOf(moved, ListTreeKeys.Of(scene.NodeA)));
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
        public async Task Of_ADropIntoTheHiddenListOfASharedCard_ShowsTheListUnderBothCardsAsTheRealDrop()
        {
            var scene = CreateSharedCardScene();
            var source = Modifier(scene, 0, 0);
            var modifiers = ((Entity)LayerOfB(scene, 1).ModelEntityManager.ManagerData.Items[0]).ModifierManager;
            var slot = SlotAt(scene, source, new ListPlace(modifiers, 0));
            var item = source.Item!;

            var moved = MoveOf(scene, source, slot);
            await scene.Tree.DropAsync(source, slot.Target, slot.Position);

            var lists = AllRows(moved.Roots).Where(row => ReferenceEquals(row.Key.Model, modifiers)).ToArray();
            Assert.Equal(2, lists.Length);
            Assert.All(lists, list => Assert.Same(item, Assert.Single(list.Children).Key.Model));
            Assert.Equal(ShownRows(ListTreeSnapshot.Of(scene.Tree.Roots), item), ShownRows(moved, item));
        }

        [Fact]
        public void Of_ADropIntoAListThatShowsTwice_AddsTheRowUnderBothCards()
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
