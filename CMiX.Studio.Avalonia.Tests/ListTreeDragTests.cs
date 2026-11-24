// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // The start of a drag, the nodes of a block, the note of a drag and the steps of the keyboard grab.
    public class ListTreeDragTests
    {
        private readonly Scene scene = CreateScene([2], AddEntities);

        private static void AddEntities(Scene scene)
        {
            for (var index = 0; index < 3; index++)
                scene.Layer.ModelEntityManager.AddItem(typeof(Entity));
        }

        private TreeNode EntityAt(int index) => scene.Group<EntitiesGroup>(0).Children[index];

        private static readonly ListTreePress Press = new(new ListTreePoint(120, 0), 16, 3);

        private ListTreeDrag DragOf(TreeNode node) =>
            ListTreeDrag.Start(scene.Tree, node, Press, new ListTreePick(ListTreeDragNumbers.Default), ListTreeSizes.Default);

        // Layer 1 of A has three entities. Every card is open.
        public ListTreeDragTests() => TestServiceProviderFactory.ExpandAll(scene.Tree);

        [Fact]
        public void Start_APressOnARowOfASet_TakesTheSetAndMakesTheCandidates()
        {
            scene.Tree.SelectByClick(EntityAt(0));
            scene.Tree.AddToSelection(EntityAt(2));

            var drag = DragOf(EntityAt(2));

            Assert.Equal(new[] { EntityAt(0), EntityAt(2) }, drag.Nodes.Nodes);
            Assert.Same(EntityAt(2), drag.Pressed);
            Assert.Same(Press, drag.Press);
            Assert.NotEmpty(drag.Candidates);
            Assert.Null(drag.Look);
        }

        [Fact]
        public void Start_APressOnARowOutOfTheSet_TakesOnlyThatRow()
        {
            scene.Tree.SelectByClick(EntityAt(0));
            scene.Tree.AddToSelection(EntityAt(2));

            var drag = DragOf(EntityAt(1));

            Assert.Equal(new[] { EntityAt(1) }, drag.Nodes.Nodes);
        }

        // Layer 1 of A has three entities and layer 2 of A has one. The cards keep their open state.
        private Scene? nodesScene;

        private Scene NodesScene => nodesScene ??= CreateScene([2], s =>
        {
            AddEntities(s);
            s.LayerOfA(1).ModelEntityManager.AddItem(typeof(Entity));
        });

        private TreeNode NodeEntityAt(int index) => NodesScene.Group<EntitiesGroup>(0).Children[index];

        private TreeNode EntityOfLayer2 => NodesScene.Group<EntitiesGroup>(1).Children[0];

        private ListTreeDragNodes NodesOf(TreeNode pressed, params TreeNode[] selected) =>
            ListTreeDragNodes.Of(ListTreeSnapshot.Of(NodesScene.Tree.Roots), pressed, selected);

        [Fact]
        public void DragNodesOf_SelectedRowsOfOneKind_TakesThemInTreeOrder()
        {
            var nodes = NodesOf(NodeEntityAt(0), EntityOfLayer2, NodeEntityAt(2), NodeEntityAt(0));

            Assert.Equal(new[] { NodeEntityAt(0), NodeEntityAt(2), EntityOfLayer2 }, nodes.Nodes);
            Assert.Same(NodeEntityAt(0), nodes.Pressed);
        }

        [Fact]
        public void DragNodesOf_ARowThatIsNotSelected_TakesOnlyThatRow()
        {
            var nodes = NodesOf(NodeEntityAt(1), NodeEntityAt(0), NodeEntityAt(2));

            Assert.Equal(new[] { NodeEntityAt(1) }, nodes.Nodes);
        }

        [Fact]
        public void DragNodesOf_RowsOfTwoKinds_TakesOnlyThePressedRow()
        {
            var nodes = NodesOf(NodeEntityAt(0), NodeEntityAt(0), NodesScene.LayerNodeOfA(1));

            Assert.Equal(new[] { NodeEntityAt(0) }, nodes.Nodes);
        }

        [Fact]
        public void DragNodesOf_ARowUnderAnotherSelectedRow_LeavesItWithItsCard()
        {
            var nodes = NodesOf(NodesScene.LayerNodeOfA(0), NodesScene.LayerNodeOfA(0), NodeEntityAt(1), NodesScene.LayerNodeOfA(1));

            Assert.Equal(new[] { NodesScene.LayerNodeOfA(0), NodesScene.LayerNodeOfA(1) }, nodes.Nodes);
        }

        [Fact]
        public void DragNodesOf_OneItemThatShowsTwice_TakesOnlyThePressedRow()
        {
            NodesScene.LayerOfA(1).ModelEntityManager.AddExistingItem((IPrefab)NodeEntityAt(0).Item!);
            var shared = NodesScene.Group<EntitiesGroup>(1).Children.Single(node => ReferenceEquals(node.Item, NodeEntityAt(0).Item));

            var nodes = NodesOf(NodeEntityAt(0), NodeEntityAt(0), shared);

            Assert.Equal(new[] { NodeEntityAt(0) }, nodes.Nodes);
        }

        private ListTreeDragLook LookOf(TreeNode node)
        {
            var drag = DragOf(node);
            return drag.PickCandidate(drag.Candidates[0])!;
        }

        [Fact]
        public void WithNearCard_ACardAndAKey_NamesTheKeyThatOpensTheCard()
        {
            var look = LookOf(EntityAt(1));
            var card = scene.LayerNodeOfA(1);

            var note = ListTreeDragNote.WithNearCard(look, card, "E");

            Assert.Equal(ListTreeDragNote.OfNearCard(look, card, "E"), note);
        }

        [Fact]
        public void WithNearCard_NoCardOrNoKey_IsThePlainNote()
        {
            var look = LookOf(EntityAt(1));

            var noCard = ListTreeDragNote.WithNearCard(look, null, "E");
            var noKey = ListTreeDragNote.WithNearCard(look, scene.LayerNodeOfA(1), null);

            Assert.Equal(ListTreeDragNote.Of(look), noCard);
            Assert.Equal(ListTreeDragNote.Of(look), noKey);
        }

        // A drag with the press at the left top of the row, as the keyboard grab starts it.
        private ListTreeDrag GrabOf(TreeNode node) =>
            ListTreeDrag.Start(scene.Tree, node, new ListTreePress(new ListTreePoint(0, 0), 16, 0), new ListTreePick(ListTreeDragNumbers.Default), ListTreeSizes.Default);

        [Fact]
        public void StartIndexOf_ADragWithNoPick_IsTheCandidateOfTheGrabbedRow()
        {
            var drag = GrabOf(EntityAt(1));

            var index = ListTreeGrabSteps.StartIndexOf(drag);

            Assert.Same(EntityAt(1), drag.Candidates[index].Slot.Target);
            Assert.Equal(index, ListTreeGrabSteps.ShownIndexOf(drag));
        }

        [Fact]
        public void ShownIndexOf_APickedCandidate_IsTheIndexOfThatCandidate()
        {
            var drag = GrabOf(EntityAt(1));
            drag.PickCandidate(drag.Candidates[2]);

            var index = ListTreeGrabSteps.ShownIndexOf(drag);

            Assert.Equal(2, index);
        }

        [Fact]
        public void IndexBelow_TheLastCandidate_StaysAtTheLast()
        {
            var drag = GrabOf(EntityAt(1));
            var last = drag.Candidates.Count - 1;
            drag.PickCandidate(drag.Candidates[last]);

            var index = ListTreeGrabSteps.IndexBelow(drag);

            Assert.Equal(last, index);
        }

        [Fact]
        public void IndexAbove_TheFirstCandidate_StaysAtTheFirst()
        {
            var drag = GrabOf(EntityAt(1));
            drag.PickCandidate(drag.Candidates[0]);

            var index = ListTreeGrabSteps.IndexAbove(drag);

            Assert.Equal(0, index);
        }

        [Fact]
        public void IndexAboveAndBelow_AShownCandidate_AreTheNeighbours()
        {
            var drag = GrabOf(EntityAt(1));
            drag.PickCandidate(drag.Candidates[2]);

            var above = ListTreeGrabSteps.IndexAbove(drag);
            var below = ListTreeGrabSteps.IndexBelow(drag);

            Assert.Equal(1, above);
            Assert.Equal(3, below);
        }
    }
}
