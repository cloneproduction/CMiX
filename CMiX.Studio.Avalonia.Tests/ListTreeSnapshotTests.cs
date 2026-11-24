// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Entity = CMiX.Core.Compositing.Entity;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // The snapshot of the tree, the keys of its rows, the hidden marks and the watch of the nodes.
    public class ListTreeSnapshotTests
    {
        // A composition with two layers. Every row is open.
        private static Scene CreateOpenScene()
        {
            var scene = CreateScene([2]);
            OpenAll(scene);
            return scene;
        }

        private static void OpenAll(Scene scene)
        {
            foreach (var node in TreeWalk.AllRows(scene.Tree.Roots))
                node.IsExpanded = true;
        }

        private static ListTreeRow RowOf(ListTreeSnapshot snapshot, TreeNode node) =>
            AllRows(snapshot.Roots).Single(row => ReferenceEquals(row.Node, node));

        private static IEnumerable<ListTreeRow> AllRows(IEnumerable<ListTreeRow> rows) =>
            rows.SelectMany(row => AllRows(row.Children).Prepend(row));

        [Fact]
        public void VisibleRows_AnOpenTree_GivesTheRowsInOrderWithTheirDepth()
        {
            var scene = CreateOpenScene();

            var rows = ListTreeSnapshot.Of(scene.Tree.Roots).VisibleRows();

            var expected = new[] { scene.Tree.Roots[0], scene.NodeA, Scene.LayersOf(scene.NodeA), scene.LayerNodeOfA(0), scene.LayerNodeOfA(1) };
            Assert.Equal(expected, rows.Select(row => row.Node));
            Assert.Equal(new[] { 0, 1, 2, 3, 3 }, rows.Select(row => row.Depth));
            Assert.All(rows, row => Assert.Equal(TreeWalk.DepthOf(row.Node!), row.Depth));
        }

        [Fact]
        public void VisibleRows_AClosedCard_KeepsItsChildrenInTheSnapshotAndHidesThem()
        {
            var scene = CreateOpenScene();
            scene.NodeA.IsExpanded = false;

            var snapshot = ListTreeSnapshot.Of(scene.Tree.Roots);

            var card = RowOf(snapshot, scene.NodeA);
            Assert.True(card.IsCard);
            Assert.False(card.IsExpanded);
            Assert.Same(Scene.LayersOf(scene.NodeA), Assert.Single(card.Children).Node);
            Assert.Equal(2, card.Children[0].Children.Count);
            Assert.Equal(new[] { scene.Tree.Roots[0], scene.NodeA }, snapshot.VisibleRows().Select(row => row.Node));
        }

        [Fact]
        public void AllRows_AClosedCard_GivesTheRowsUnderItInTreeOrder()
        {
            var scene = CreateOpenScene();
            scene.NodeA.IsExpanded = false;

            var rows = ListTreeSnapshot.Of(scene.Tree.Roots).AllRows();

            var expected = new[] { scene.Tree.Roots[0], scene.NodeA, Scene.LayersOf(scene.NodeA), scene.LayerNodeOfA(0), scene.LayerNodeOfA(1) };
            Assert.Equal(expected, rows.Select(row => row.Node));
        }

        [Fact]
        public void RowOf_ANodeUnderAClosedCard_GivesItsRow()
        {
            var scene = CreateOpenScene();
            scene.NodeA.IsExpanded = false;

            var row = ListTreeSnapshot.Of(scene.Tree.Roots).RowOf(scene.LayerNodeOfA(1));

            Assert.Same(scene.LayerNodeOfA(1), row?.Node);
            Assert.Equal(ListTreeKeys.Of(scene.LayerNodeOfA(1)), row?.Key);
        }

        [Fact]
        public void RowOf_AGroupWithNoItem_GivesNull()
        {
            var scene = CreateOpenScene();

            var row = ListTreeSnapshot.Of(scene.Tree.Roots).RowOf(scene.Group<EntitiesGroup>(0));

            Assert.Null(row);
        }

        [Fact]
        public void Of_AGroupRowWithNoItem_LeavesItOut()
        {
            var scene = CreateOpenScene();

            var snapshot = ListTreeSnapshot.Of(scene.Tree.Roots);

            Assert.Equal(new[] { "Layers" }, RowOf(snapshot, scene.NodeA).Children.Select(row => row.Node!.Header));
            var layer = RowOf(snapshot, scene.LayerNodeOfA(0));
            Assert.Empty(layer.Children);
            Assert.True(layer.IsCard);
        }

        [Fact]
        public void Of_AGroupRowWithItems_KeepsItAsTheParentOfItsItems()
        {
            var scene = CreateOpenScene();
            scene.Composition.TextureModifierManager.AddItem(typeof(Blur));
            var filters = Scene.Group<FiltersGroup>(scene.NodeA);

            var snapshot = ListTreeSnapshot.Of(scene.Tree.Roots);

            Assert.Equal(new[] { "Layers", "Filters" }, RowOf(snapshot, scene.NodeA).Children.Select(row => row.Node!.Header));
            var group = RowOf(snapshot, filters);
            Assert.Equal(2, group.Depth);
            var filter = Assert.Single(group.Children);
            Assert.Same(filters.Children[0], filter.Node);
            Assert.Equal(3, filter.Depth);
        }

        [Fact]
        public void Of_AGroupRowAndAnItemRow_MarksOnlyTheGroupRowAsAGroup()
        {
            var scene = CreateOpenScene();

            var snapshot = ListTreeSnapshot.Of(scene.Tree.Roots);

            Assert.True(RowOf(snapshot, Scene.LayersOf(scene.NodeA)).IsGroup);
            Assert.False(RowOf(snapshot, scene.NodeA).IsGroup);
            Assert.False(RowOf(snapshot, scene.LayerNodeOfA(0)).IsGroup);
        }

        [Fact]
        public void Of_OneEntityUnderTwoLayers_GivesTwoRows()
        {
            var scene = CreateOpenScene();
            scene.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            var entity = (IPrefab)scene.LayerOfA(0).ModelEntityManager.ManagerData.Items[0];
            scene.LayerOfA(1).ModelEntityManager.AddExistingItem(entity);

            var rows = AllRows(ListTreeSnapshot.Of(scene.Tree.Roots).Roots).Where(row => ReferenceEquals(row.Node!.Item, entity)).ToArray();

            Assert.Equal(2, rows.Length);
            Assert.NotSame(rows[0].Node, rows[1].Node);
            Assert.Same(Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(0)), rows[0].Node.Parent);
            Assert.Same(Scene.Group<EntitiesGroup>(scene.LayerNodeOfA(1)), rows[1].Node!.Parent);
        }

        [Fact]
        public void Of_OneEntityUnderTwoLayers_GivesTwoKeysOfOneModel()
        {
            var scene = CreateOpenScene();
            scene.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            var entity = (IPrefab)scene.LayerOfA(0).ModelEntityManager.ManagerData.Items[0];
            scene.LayerOfA(1).ModelEntityManager.AddExistingItem(entity);

            var rows = AllRows(ListTreeSnapshot.Of(scene.Tree.Roots).Roots).Where(row => ReferenceEquals(row.Key.Model, entity)).ToArray();

            Assert.Equal(2, rows.Length);
            Assert.NotEqual(rows[0].Key, rows[1].Key);
            Assert.Equal(ListTreeKeys.Of(rows[0].Node!), rows[0].Key);
            Assert.Same(scene.LayerOfA(1).ModelEntityManager, rows[1].Key.Parent!.Model);
        }

        [Fact]
        public void Of_ARowAfterTheTreeIsMadeAgain_GivesAnEqualKey()
        {
            var scene = CreateOpenScene();
            var before = RowOf(ListTreeSnapshot.Of(scene.Tree.Roots), scene.LayerNodeOfA(1)).Key;

            var again = CreateTree(scene.Provider);

            var layer = AllRows(ListTreeSnapshot.Of(again.Roots).Roots).Single(row => ReferenceEquals(row.Key.Model, scene.LayerOfA(1)));
            Assert.NotSame(scene.LayerNodeOfA(1), layer.Node);
            Assert.Equal(before, layer.Key);
        }

        [Fact]
        public void IsHidden_ARowAtRest_IsFalse()
        {
            var scene = CreateScene([2], s => AddKinds(s.Layer));
            var marks = new ListTreeMarks(null, null, null);

            Assert.False(marks.IsHidden(scene.LayerNodeOfA(0)));
            Assert.False(marks.IsHidden(scene.Group<EntitiesGroup>(0).Children[0]));
            Assert.False(marks.IsHidden(null));
        }

        [Fact]
        public void IsHidden_AHiddenLayer_IsTrueForTheLayerAndEveryRowUnderIt()
        {
            var scene = CreateScene([2], s => AddKinds(s.Layer));
            var marks = new ListTreeMarks(null, null, null);

            scene.Layer.PrefabService.Visibility.Value = true;

            Assert.True(marks.IsHidden(scene.LayerNodeOfA(0)));
            Assert.True(marks.IsHidden(scene.Group<EntitiesGroup>(0).Children[0]));
            Assert.False(marks.IsHidden(scene.LayerNodeOfA(1)));
        }

        // A watch of the default scene. Changes gives the number of its Changed events.
        private static (Scene Scene, ListTreeNodeWatch Watch, Func<int> Changes) Watched()
        {
            var scene = CreateScene();
            var watch = new ListTreeNodeWatch();
            var changes = 0;
            watch.Changed += (_, _) => changes++;
            watch.Watch(scene.Tree.Roots);
            return (scene, watch, () => changes);
        }

        [Fact]
        public void Watch_ADeepCardCloses_RaisesChanged()
        {
            var (scene, _, changes) = Watched();

            scene.LayerNodeOfA(0).IsExpanded = !scene.LayerNodeOfA(0).IsExpanded;

            Assert.Equal(1, changes());
        }

        [Fact]
        public void Watch_ARowComes_RaisesChanged()
        {
            var (scene, _, changes) = Watched();

            scene.A.LayerManager.AddItem(typeof(Layer));

            Assert.True(changes() > 0);
        }

        [Fact]
        public void Watch_ALayerIsHidden_RaisesHiddenChangedOnceAndNotChanged()
        {
            var (scene, watch, changes) = Watched();
            var hiddenChanges = 0;
            watch.HiddenChanged += (_, _) => hiddenChanges++;

            scene.Layer.PrefabService.Visibility.Value = true;

            Assert.Equal(1, hiddenChanges);
            Assert.Equal(0, changes());
        }

        [Fact]
        public void Unwatch_ACardCloses_RaisesNothing()
        {
            var (scene, watch, changes) = Watched();

            watch.Unwatch();

            scene.NodeA.IsExpanded = false;
            scene.A.LayerManager.AddItem(typeof(Layer));

            Assert.Equal(0, changes());
        }
    }
}
