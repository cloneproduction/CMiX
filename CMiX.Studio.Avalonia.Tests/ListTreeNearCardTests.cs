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
    public class ListTreeNearCardTests
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

        // Layer 2 of A is a closed card.
        public ListTreeNearCardTests()
        {
            TestServiceProviderFactory.ExpandAll(scene.Tree);
            scene.LayerNodeOfA(1).IsExpanded = false;
        }

        [Fact]
        public void OfKey_EveryCandidate_IsTheNearCardOfAPointerAtThePlaceOfTheCandidate()
        {
            var drag = DragOf(EntityAt(1));

            foreach (var candidate in drag.Candidates)
            {
                var look = drag.PickCandidate(candidate)!;
                var atPlace = new ListTreePoint(Press.Pointer.X, candidate.Place.Y + Press.GrabOffsetY);

                Assert.Same(ListTreeNearCard.Of(look, Press, atPlace), ListTreeNearCard.OfKey(look, Press));
            }
        }
    }
}
