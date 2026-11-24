// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Entity = CMiX.Core.Compositing.Entity;

namespace CMiX.Studio.Avalonia.Tests
{
    // The set of selected rows of the list tree, read and changed through the model.
    public class ProjectTreeSelectionSetTests
    {
        private readonly Scene scene = CreateScene([2], AddEntities);

        public ProjectTreeSelectionSetTests()
        {
            TestServiceProviderFactory.ExpandAll(scene.Tree);
            scene.Tree.SelectedNode = null;
        }

        private static void AddEntities(Scene scene)
        {
            for (var index = 0; index < 3; index++)
                scene.Layer.ModelEntityManager.AddItem(typeof(Entity));
        }

        private ProjectTree Tree => scene.Tree;

        private TreeNode EntityAt(int index) => scene.Group<EntitiesGroup>(0).Children[index];

        private List<int> CountsOfSelectedNodes()
        {
            var counts = new List<int>();
            Tree.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(ProjectTree.SelectedNodes))
                    counts.Add(Tree.SelectedNodes.Count);
            };
            return counts;
        }

        [Fact]
        public void AddToSelection_ARow_ShowsTheNewSetOnce()
        {
            Tree.SelectByClick(EntityAt(0));
            var counts = CountsOfSelectedNodes();

            Tree.AddToSelection(EntityAt(2));

            Assert.Equal(new[] { 2 }, counts);
            Assert.Same(EntityAt(2), Tree.SelectedNode);
        }

        [Fact]
        public void SelectByClick_ARowOfASet_LeavesOnlyThatRow()
        {
            Tree.SelectByClick(EntityAt(0));
            Tree.AddToSelection(EntityAt(2));

            Tree.SelectByClick(EntityAt(1));

            Assert.Equal(new[] { EntityAt(1) }, Tree.SelectedNodes);
        }

        [Fact]
        public void AddOrRemoveFromSelection_ARowTwice_AddsItAndTakesItOut()
        {
            Tree.SelectByClick(EntityAt(0));

            Tree.AddOrRemoveFromSelection(EntityAt(1));
            var added = Tree.SelectedNodes.ToArray();
            Tree.AddOrRemoveFromSelection(EntityAt(1));

            Assert.Equal(new[] { EntityAt(0), EntityAt(1) }, added);
            Assert.Equal(new[] { EntityAt(0) }, Tree.SelectedNodes);
        }

        [Fact]
        public void IsOneOfSeveralSelected_ARowOfASetAndTheOnlySelectedRow_IsTrueOnlyInTheSet()
        {
            Tree.SelectByClick(EntityAt(0));
            var alone = Tree.IsOneOfSeveralSelected(EntityAt(0));

            Tree.AddToSelection(EntityAt(1));

            Assert.False(alone);
            Assert.True(Tree.IsOneOfSeveralSelected(EntityAt(0)));
            Assert.False(Tree.IsOneOfSeveralSelected(EntityAt(2)));
        }

        [Fact]
        public void SelectedNodesOf_TheItemsOfTheSelectedNode_GivesTheSelectedNodesOfTheItems()
        {
            Tree.SelectByClick(EntityAt(0));
            Tree.AddToSelection(EntityAt(1));
            Tree.AddToSelection(EntityAt(2));

            var nodes = Tree.SelectedNodesOf(new object?[] { EntityAt(2).Item, EntityAt(0).Item });

            Assert.Equal(new[] { EntityAt(0), EntityAt(2) }, nodes);
        }

        [Fact]
        public void SelectedNodesOf_ItemsWithoutTheSelectedNode_IsEmpty()
        {
            Tree.SelectByClick(EntityAt(0));
            Tree.AddToSelection(EntityAt(1));

            var nodes = Tree.SelectedNodesOf(new object?[] { EntityAt(0).Item });

            Assert.Empty(nodes);
        }

        [Fact]
        public void VisibleNodes_ATreeWithAClosedCard_GivesTheShownNodesInTreeOrder()
        {
            scene.LayerNodeOfA(0).IsExpanded = false;
            var snapshot = ListTreeSnapshot.Of(Tree.Roots);

            var nodes = snapshot.VisibleNodes();

            Assert.Equal(snapshot.VisibleRows().Select(row => row.Node), nodes);
            Assert.DoesNotContain(EntityAt(0), nodes);
            Assert.Contains(scene.LayerNodeOfA(1), nodes);
        }
    }
}
