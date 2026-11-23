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
            Assert.Same(EntityAt(2), drag.Node);
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
    }
}
