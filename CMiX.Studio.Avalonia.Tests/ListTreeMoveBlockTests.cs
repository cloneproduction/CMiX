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
using static CMiX.Studio.Avalonia.Tests.ListTreeMoveTests;
using Scene = CMiX.Studio.Avalonia.Tests.Support.TreeFixtures.Scene;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // The move of a block of rows against the real block drop, and the blocks that a list refuses.
    // The scenes are the scenes of ListTreeMoveTests.
    public class ListTreeMoveBlockTests
    {
        private static readonly string[] Blocks =
            { "two entities", "an entity of each composition", "two modifiers of one entity", "modifiers of two entities", "two layers" };

        public static TheoryData<string, string> ScenesAndBlocks
        {
            get
            {
                var data = new TheoryData<string, string>();
                foreach (var scene in new[] { "plain", "closed", "shared entity", "shared card" })
                    foreach (var block in Blocks)
                        data.Add(scene, block);

                return data;
            }
        }

        private sealed record BlockNodes(TreeNode[] Nodes, int PressedIndex);

        // The nodes are in tree order.
        private static BlockNodes BlockNodesOf(Scene scene, string name) => name switch
        {
            "two entities" => new(new[] { Entity(scene, 0), Entity(scene, 1) }, 1),
            "an entity of each composition" => new(new[] { Entity(scene, 0), EntityOfB(scene) }, 1),
            "two modifiers of one entity" => new(new[] { Modifier(scene, 0, 0), Modifier(scene, 0, 1) }, 1),
            "modifiers of two entities" => new(new[] { Modifier(scene, 0, 0), Modifier(scene, 1, 0) }, 0),
            _ => new(new[] { scene.LayerNodeOfA(0), scene.LayerNodeOfB(1) }, 0)
        };

        private static ListTreeBlock BlockOf(ListTreeSnapshot snapshot, BlockNodes nodes)
        {
            var rows = nodes.Nodes.Select(node => RowOf(snapshot, node)).ToArray();
            return new ListTreeBlock(rows, rows[nodes.PressedIndex]);
        }

        private static IReadOnlyList<ListTreeSlot> SlotsOf(ListTreeSnapshot snapshot, ListTreeBlock block)
        {
            var service = new TreeDropService();
            return new ListTreeCandidates(new ListTreeSlots(service), service, ListTreeSizes.Default).SlotsOf(snapshot, block);
        }

        private static ListTreePlace? PlaceByMoveOf(ListTreeSnapshot snapshot, ListTreeBlock block, ListTreeSlot slot)
        {
            var moved = ListTreeMove.Of(snapshot, block, slot);
            return moved.RowOf(block.Pressed.Node!) is { } row ? ListTreeLayout.Of(moved, ListTreeSizes.Default).PlaceOf(row.Key) : null;
        }

        [Theory]
        [MemberData(nameof(ScenesAndBlocks))]
        public void PlaceOf_EverySlotOfABlock_EqualsThePlaceOfThePressedRowInTheLayoutOfTheMove(string sceneName, string blockName)
        {
            var scene = SceneOf(sceneName);
            var snapshot = ListTreeSnapshot.Of(scene.Tree.Roots);
            var block = BlockOf(snapshot, BlockNodesOf(scene, blockName));
            var slots = SlotsOf(snapshot, block);

            var places = slots.Select(ListTreeCountedTree.Of(snapshot, block, ListTreeSizes.Default).PlaceOf).ToArray();

            Assert.NotEmpty(slots);
            Assert.Equal(slots.Select(slot => PlaceByMoveOf(snapshot, block, slot)), places);
        }

        public static TheoryData<string> BlockNames => new(Blocks);

        // The real tree opens the rows of a moved card. The view restores them after the drop.
        private static Shown[] ShownRowsOutsideTheBlock(ListTreeSnapshot snapshot, IReadOnlyList<IControl> block) =>
            snapshot.VisibleRows().Where(row => !block.Any(item => IsUnder(row.Key.Parent, item))).Select(row => new Shown(row.Key, row.Depth)).ToArray();

        [Theory]
        [MemberData(nameof(BlockNames))]
        public async Task Of_EverySlotOfABlock_GivesTheRowsOfTheRealBlockDrop(string name)
        {
            var first = CreateScene();
            var firstSnapshot = ListTreeSnapshot.Of(first.Tree.Roots);
            var count = SlotsOf(firstSnapshot, BlockOf(firstSnapshot, BlockNodesOf(first, name))).Count;
            for (var index = 0; index < count; index++)
            {
                var scene = CreateScene();
                var snapshot = ListTreeSnapshot.Of(scene.Tree.Roots);
                var nodes = BlockNodesOf(scene, name);
                var block = BlockOf(snapshot, nodes);
                var slot = SlotsOf(snapshot, block)[index];
                var items = nodes.Nodes.Select(node => node.Item!).ToArray();

                var expected = ShownRowsOutsideTheBlock(ListTreeMove.Of(snapshot, block, slot), items);
                await scene.Tree.DropBlockAsync(nodes.Nodes, nodes.Nodes[nodes.PressedIndex], slot.Target, slot.Position);

                Assert.Equal(expected, ShownRowsOutsideTheBlock(ListTreeSnapshot.Of(scene.Tree.Roots), items));
            }

            Assert.True(count > 1, name);
        }

        private static IReadOnlyList<IControl> EntitiesOf(Layer layer) => layer.ModelEntityManager.ManagerData.Items;

        // The second entity is shared with layer 2 of A, so it cannot go to B.
        [Fact]
        public async Task DropBlockAsync_ABlockWithASharedEntity_MovesNoRowAndShowsTheNotice()
        {
            var scene = CreateSharedScene();
            var notice = scene.Provider.GetRequiredService<FakeMoveBlockedNotice>();
            var nodes = new[] { Entity(scene, 0), Entity(scene, 1) };
            var items = nodes.Select(node => node.Item!).ToArray();
            var header = nodes[1].Header;
            var other = EntityOfB(scene).Item!;

            await scene.Tree.DropBlockAsync(nodes, nodes[0], EntityOfB(scene), DropPosition.Before);

            Assert.Equal(items, EntitiesOf(scene.LayerOfA(0)));
            Assert.Equal(new[] { other }, EntitiesOf(LayerOfB(scene, 1)));
            Assert.Contains(header, Assert.Single(notice.Messages));
        }

        [Fact]
        public void SlotsOf_ABlockWithASharedEntity_OffersNoSlotInTheOtherComposition()
        {
            var scene = CreateSharedScene();
            var snapshot = ListTreeSnapshot.Of(scene.Tree.Roots);

            var slots = SlotsOf(snapshot, BlockOf(snapshot, new BlockNodes(new[] { Entity(scene, 0), Entity(scene, 1) }, 0)));

            Assert.NotEmpty(slots);
            Assert.DoesNotContain(slots, slot => ReferenceEquals(TreeRowOrder.CompositionOf(slot.Target), scene.NodeB));
        }

        // A light takes the translate modifier and refuses the scale modifier.
        private static TreeNode ModifiersOfALightOfB(Scene scene)
        {
            LayerOfB(scene, 1).LightManager.AddItem(typeof(LightEntity));
            var light = Scene.Group<LightsGroup>(scene.LayerNodeOfB(1)).Children[0];
            light.IsExpanded = true;
            return Scene.Group<ModifiersGroup>(light);
        }

        [Fact]
        public async Task DropBlockAsync_ABlockWithARowThatTheListRefuses_MovesNoRow()
        {
            var scene = CreateScene();
            var modifiers = ModifiersOfALightOfB(scene);
            var nodes = new[] { Modifier(scene, 0, 0), Modifier(scene, 0, 1) };
            var items = nodes.Select(node => node.Item!).ToArray();
            var entity = (Entity)EntitiesOf(scene.LayerOfA(0))[0];

            await scene.Tree.DropBlockAsync(nodes, nodes[0], modifiers, DropPosition.Inside);

            Assert.Equal(items, entity.ModifierManager.ManagerData.Items);
            Assert.Empty(modifiers.ChildManager!.ManagerData.Items);
        }

        [Fact]
        public async Task DropBlockAsync_ABlockWithARowThatTheListRefuses_LeavesTheStackEmpty()
        {
            var scene = CreateScene();
            var modifiers = ModifiersOfALightOfB(scene);
            var nodes = new[] { Modifier(scene, 0, 0), Modifier(scene, 0, 1) };
            scene.ClearUndo();

            await scene.Tree.DropBlockAsync(nodes, nodes[0], modifiers, DropPosition.Inside);

            Assert.Equal(0, UndoCounts.StepsOf(scene.Undo));
            Assert.False(scene.Undo.CanRedo);
        }

        [Fact]
        public void SlotsOf_ABlockWithARowThatTheListRefuses_OffersNoSlotOnTheLight()
        {
            var scene = CreateScene();
            var modifiers = ModifiersOfALightOfB(scene);
            var snapshot = ListTreeSnapshot.Of(scene.Tree.Roots);

            var slots = SlotsOf(snapshot, BlockOf(snapshot, new BlockNodes(new[] { Modifier(scene, 0, 0), Modifier(scene, 0, 1) }, 0)));

            Assert.NotEmpty(slots);
            Assert.DoesNotContain(slots, slot => ReferenceEquals(slot.ListPlace.List, modifiers.ChildManager));
        }

        [Fact]
        public void Of_ABlockOfThreeEntitiesDroppedIntoAnotherLayer_ShowsTheRowsInTreeOrderAtTheSlot()
        {
            var scene = TreeFixtures.CreateScene([2], AddThreeEntitiesAndOne);
            foreach (var node in scene.All())
                node.IsExpanded = true;
            var snapshot = ListTreeSnapshot.Of(scene.Tree.Roots);
            var entities = scene.Group<EntitiesGroup>(0).Children.ToArray();
            var target = scene.Group<EntitiesGroup>(1).Children[0];
            var block = BlockOf(snapshot, new BlockNodes(entities, 1));
            var slot = SlotsOf(snapshot, block).Single(slot => ReferenceEquals(slot.Target, target) && slot.Position == DropPosition.Before);

            var moved = ListTreeMove.Of(snapshot, block, slot);

            var holder = ListTreeKeys.Of(scene.Group<EntitiesGroup>(1));
            var items = entities.Select(entity => (object)entity.Item!).Append(target.Item!);
            Assert.Equal(items, RowOf(moved, holder).Children.Select(row => row.Key.Model));
            Assert.DoesNotContain(RowOf(moved, ListTreeKeys.Of(scene.LayerNodeOfA(0))).Children, row => ReferenceEquals(row.Key.Model, scene.Layer.ModelEntityManager));
        }

        private static void AddThreeEntitiesAndOne(Scene scene)
        {
            for (var index = 0; index < 3; index++)
                scene.Layer.ModelEntityManager.AddItem(typeof(Entity));
            scene.LayerOfA(1).ModelEntityManager.AddItem(typeof(Entity));
        }
    }
}
