// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Prefabs;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Entity = CMiX.Core.Compositing.Entity;

namespace CMiX.Studio.Avalonia.Tests
{
    public class ListTreeDragNodesTests
    {
        private readonly Scene scene = CreateScene([2], AddEntities);

        private static void AddEntities(Scene scene)
        {
            for (var index = 0; index < 3; index++)
                scene.Layer.ModelEntityManager.AddItem(typeof(Entity));
            scene.LayerOfA(1).ModelEntityManager.AddItem(typeof(Entity));
        }

        private TreeNode EntityAt(int index) => scene.Group<EntitiesGroup>(0).Children[index];

        private TreeNode EntityOfLayer2 => scene.Group<EntitiesGroup>(1).Children[0];

        private ListTreeDragNodes Of(TreeNode pressed, params TreeNode[] selected) =>
            ListTreeDragNodes.Of(ListTreeSnapshot.Of(scene.Tree.Roots), pressed, selected);

        [Fact]
        public void Of_SelectedRowsOfOneKind_TakesThemInTreeOrder()
        {
            var nodes = Of(EntityAt(0), EntityOfLayer2, EntityAt(2), EntityAt(0));

            Assert.Equal(new[] { EntityAt(0), EntityAt(2), EntityOfLayer2 }, nodes.Nodes);
            Assert.Same(EntityAt(0), nodes.Pressed);
        }

        [Fact]
        public void Of_ARowThatIsNotSelected_TakesOnlyThatRow()
        {
            var nodes = Of(EntityAt(1), EntityAt(0), EntityAt(2));

            Assert.Equal(new[] { EntityAt(1) }, nodes.Nodes);
        }

        [Fact]
        public void Of_RowsOfTwoKinds_TakesOnlyThePressedRow()
        {
            var nodes = Of(EntityAt(0), EntityAt(0), scene.LayerNodeOfA(1));

            Assert.Equal(new[] { EntityAt(0) }, nodes.Nodes);
        }

        [Fact]
        public void Of_ARowUnderAnotherSelectedRow_LeavesItWithItsCard()
        {
            var nodes = Of(scene.LayerNodeOfA(0), scene.LayerNodeOfA(0), EntityAt(1), scene.LayerNodeOfA(1));

            Assert.Equal(new[] { scene.LayerNodeOfA(0), scene.LayerNodeOfA(1) }, nodes.Nodes);
        }

        [Fact]
        public void Of_OneItemThatShowsTwice_TakesOnlyThePressedRow()
        {
            scene.LayerOfA(1).ModelEntityManager.AddExistingItem((IPrefab)EntityAt(0).Item!);
            var shared = scene.Group<EntitiesGroup>(1).Children.Single(node => ReferenceEquals(node.Item, EntityAt(0).Item));

            var nodes = Of(EntityAt(0), EntityAt(0), shared);

            Assert.Equal(new[] { EntityAt(0) }, nodes.Nodes);
        }
    }
}
