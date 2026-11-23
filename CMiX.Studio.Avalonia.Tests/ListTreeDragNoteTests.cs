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
    public class ListTreeDragNoteTests
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

        public ListTreeDragNoteTests() => TestServiceProviderFactory.ExpandAll(scene.Tree);

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
    }
}
