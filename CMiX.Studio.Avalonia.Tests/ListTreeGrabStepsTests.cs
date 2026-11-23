// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Compositing;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    public class ListTreeGrabStepsTests
    {
        private readonly Scene scene = CreateScene([2], AddEntities);

        // Layer 1 of A has three entities. Every card is open.
        public ListTreeGrabStepsTests() => TestServiceProviderFactory.ExpandAll(scene.Tree);

        private static void AddEntities(Scene scene)
        {
            for (var index = 0; index < 3; index++)
                scene.Layer.ModelEntityManager.AddItem(typeof(Entity));
        }

        private TreeNode EntityAt(int index) => scene.Group<EntitiesGroup>(0).Children[index];

        private ListTreeDrag DragOf(TreeNode node) =>
            ListTreeDrag.Start(scene.Tree, node, new ListTreePress(new ListTreePoint(0, 0), 16, 0), new ListTreePick(ListTreeDragNumbers.Default), ListTreeSizes.Default);

        [Fact]
        public void StartIndexOf_ADragWithNoPick_IsTheCandidateOfTheGrabbedRow()
        {
            var drag = DragOf(EntityAt(1));

            var index = ListTreeGrabSteps.StartIndexOf(drag);

            Assert.Same(EntityAt(1), drag.Candidates[index].Slot.Target);
            Assert.Equal(index, ListTreeGrabSteps.ShownIndexOf(drag));
        }

        [Fact]
        public void ShownIndexOf_APickedCandidate_IsTheIndexOfThatCandidate()
        {
            var drag = DragOf(EntityAt(1));
            drag.PickCandidate(drag.Candidates[2]);

            var index = ListTreeGrabSteps.ShownIndexOf(drag);

            Assert.Equal(2, index);
        }

        [Fact]
        public void IndexBelow_TheLastCandidate_StaysAtTheLast()
        {
            var drag = DragOf(EntityAt(1));
            var last = drag.Candidates.Count - 1;
            drag.PickCandidate(drag.Candidates[last]);

            var index = ListTreeGrabSteps.IndexBelow(drag);

            Assert.Equal(last, index);
        }

        [Fact]
        public void IndexAbove_TheFirstCandidate_StaysAtTheFirst()
        {
            var drag = DragOf(EntityAt(1));
            drag.PickCandidate(drag.Candidates[0]);

            var index = ListTreeGrabSteps.IndexAbove(drag);

            Assert.Equal(0, index);
        }

        [Fact]
        public void IndexAboveAndBelow_AShownCandidate_AreTheNeighbours()
        {
            var drag = DragOf(EntityAt(1));
            drag.PickCandidate(drag.Candidates[2]);

            var above = ListTreeGrabSteps.IndexAbove(drag);
            var below = ListTreeGrabSteps.IndexBelow(drag);

            Assert.Equal(1, above);
            Assert.Equal(3, below);
        }
    }
}
