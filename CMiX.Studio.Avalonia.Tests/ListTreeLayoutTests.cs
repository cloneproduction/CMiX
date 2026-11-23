// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    public class ListTreeLayoutTests
    {
        // The nodes only give the identity of the rows. The tests build the rows by hand.
        private readonly TreeNode[] nodes = CreateScene([8]).All().ToArray();

        private ListTreeRow Item(int node, int depth) => new(nodes[node], depth, false, false, []);

        private ListTreeRow OpenCard(int node, int depth, params ListTreeRow[] children) => new(nodes[node], depth, true, true, children);

        private ListTreeRow ClosedCard(int node, int depth, params ListTreeRow[] children) => new(nodes[node], depth, true, false, children);

        private static ListTreeLayout LayoutOf(params ListTreeRow[] roots) => ListTreeLayout.Of(new ListTreeSnapshot(roots), ListTreeSizes.Default);

        private ListTreePlace PlaceOf(ListTreeLayout layout, int node) => Assert.IsType<ListTreePlace>(layout.PlaceOf(nodes[node]));

        [Fact]
        public void Of_RowsInOrder_PutsEachRowOnePitchOfThirtyFiveBelowTheOneBefore()
        {
            var layout = LayoutOf(OpenCard(0, 0, Item(1, 1), Item(2, 1)), Item(3, 0));

            Assert.Equal(new[] { nodes[0], nodes[1], nodes[2], nodes[3] }, layout.Places.Select(place => place.Node));
            Assert.Equal(new[] { 0d, 35, 70, 105 }, layout.Places.Select(place => place.Y));
            Assert.All(layout.Places, place => Assert.Equal(32, place.RowHeight));
        }

        [Fact]
        public void Of_RowsAtSeveralDepths_IndentsTheContentSixteenPerDepth()
        {
            var layout = LayoutOf(OpenCard(0, 0, OpenCard(1, 1, Item(2, 2))));

            Assert.Equal(new[] { 0, 1, 2 }, layout.Places.Select(place => place.Depth));
            Assert.Equal(new[] { 0d, 16, 32 }, layout.Places.Select(place => place.Indent));
        }

        [Fact]
        public void Of_AnOpenCard_EndsTheAreaAtTheBottomOfTheLastDescendant()
        {
            var layout = LayoutOf(OpenCard(0, 0, Item(1, 1), Item(2, 1)), Item(3, 0));

            var card = PlaceOf(layout, 0);
            Assert.Equal(2 * 35 + 32, card.AreaHeight);
            Assert.Equal(PlaceOf(layout, 2).Y + 32, card.Y + card.AreaHeight);
            Assert.True(card.CanToggle);
            Assert.Equal(32, PlaceOf(layout, 1).AreaHeight);
        }

        [Fact]
        public void Of_AnOpenCardThatEndsWithANestedOpenCard_EndsBothAreasAtTheBottomOfTheDeepestRow()
        {
            var layout = LayoutOf(OpenCard(0, 0, Item(1, 1), OpenCard(2, 1, Item(3, 2), Item(4, 2))), Item(5, 0));

            var bottom = PlaceOf(layout, 4).Y + 32;
            var outer = PlaceOf(layout, 0);
            var inner = PlaceOf(layout, 2);
            Assert.Equal(bottom, outer.Y + outer.AreaHeight);
            Assert.Equal(bottom, inner.Y + inner.AreaHeight);
            Assert.Equal(bottom + 3, PlaceOf(layout, 5).Y);
        }

        [Fact]
        public void Of_AClosedCard_GivesOneRowAndNoPlaceToItsChildren()
        {
            var layout = LayoutOf(ClosedCard(0, 0, Item(1, 1), OpenCard(2, 1, Item(3, 2))), Item(4, 0));

            var card = PlaceOf(layout, 0);
            Assert.Equal(32, card.AreaHeight);
            Assert.True(card.CanToggle);
            Assert.Null(layout.PlaceOf(nodes[1]));
            Assert.Null(layout.PlaceOf(nodes[2]));
            Assert.Null(layout.PlaceOf(nodes[3]));
            Assert.Equal(35, PlaceOf(layout, 4).Y);
        }

        [Fact]
        public void Of_AnOpenCardWithNoChild_GivesOneRowThatCanNotToggle()
        {
            var layout = LayoutOf(OpenCard(0, 0), Item(1, 0));

            var card = PlaceOf(layout, 0);
            Assert.Equal(32, card.AreaHeight);
            Assert.False(card.CanToggle);
            Assert.False(PlaceOf(layout, 1).CanToggle);
            Assert.Equal(35, PlaceOf(layout, 1).Y);
        }

        [Fact]
        public void Of_SeveralVisibleRows_GivesATotalHeightOfThePitchesLessOneGap()
        {
            var layout = LayoutOf(OpenCard(0, 0, Item(1, 1), ClosedCard(2, 1, Item(3, 2))), Item(4, 0));

            Assert.Equal(4, layout.Places.Count);
            Assert.Equal(4 * 35 - 3, layout.TotalHeight);
        }

        [Fact]
        public void Of_AnEmptySnapshot_GivesNoPlaceAndNoHeight()
        {
            var layout = LayoutOf();

            Assert.Empty(layout.Places);
            Assert.Equal(0, layout.TotalHeight);
        }

        [Fact]
        public void PlaceOf_ASnapshotOfTheLiveTree_GivesTheVisibleRowsTheirPlace()
        {
            var scene = CreateScene([2]);
            foreach (var node in TreeWalk.AllRows(scene.Tree.Roots))
                node.IsExpanded = true;

            var layout = ListTreeLayout.Of(ListTreeSnapshot.Of(scene.Tree.Roots), ListTreeSizes.Default);

            Assert.Equal(5, layout.Places.Count);
            Assert.Equal(4 * 35, PlaceOf(layout, scene.LayerNodeOfA(1)).Y);
        }

        private static ListTreePlace PlaceOf(ListTreeLayout layout, TreeNode node) => Assert.IsType<ListTreePlace>(layout.PlaceOf(node));
    }
}
