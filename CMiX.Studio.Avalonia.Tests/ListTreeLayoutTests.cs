// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // The places of the rows, the arrangement that the tree shows and the window of the realized rows.
    public class ListTreeLayoutTests
    {
        // The nodes only give the identity of the rows. The tests build the rows by hand.
        private readonly TreeNode[] nodes = CreateScene([8]).All().ToArray();

        private ListTreeRow Item(int node, int depth) => new(KeyOf(node), nodes[node], depth, false, false, false, []);

        private ListTreeRow OpenCard(int node, int depth, params ListTreeRow[] children) => new(KeyOf(node), nodes[node], depth, true, false, true, children);

        private ListTreeRow ClosedCard(int node, int depth, params ListTreeRow[] children) => new(KeyOf(node), nodes[node], depth, true, false, false, children);

        private static ListTreeLayout LayoutOf(params ListTreeRow[] roots) => ListTreeLayout.Of(new ListTreeSnapshot(roots), ListTreeSizes.Default);

        private ListTreeRowKey KeyOf(int node) => ListTreeKeys.Of(nodes[node]);

        private ListTreePlace PlaceOf(ListTreeLayout layout, int node) => Assert.IsType<ListTreePlace>(layout.PlaceOf(KeyOf(node)));

        [Fact]
        public void Of_RowsInOrder_PutsEachRowOnePitchOfThirtyFiveBelowTheOneBefore()
        {
            var layout = LayoutOf(OpenCard(0, 0, Item(1, 1), Item(2, 1)), Item(3, 0));

            Assert.Equal(new[] { KeyOf(0), KeyOf(1), KeyOf(2), KeyOf(3) }, layout.Places.Select(place => place.Key));
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
            Assert.Null(layout.PlaceOf(KeyOf(1)));
            Assert.Null(layout.PlaceOf(KeyOf(2)));
            Assert.Null(layout.PlaceOf(KeyOf(3)));
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

        private static ListTreePlace PlaceOf(ListTreeLayout layout, TreeNode node) => Assert.IsType<ListTreePlace>(layout.PlaceOf(ListTreeKeys.Of(node)));

        [Fact]
        public void ArrangementOf_ATreeWithAClosedCard_GivesTheShownRowsInOrderWithTheirPlaces()
        {
            var scene = CreateScene([2], s => AddKinds(s.Layer));
            scene.LayerNodeOfA(0).IsExpanded = false;

            var arrangement = ListTreeArrangement.Of(scene.Tree.Roots, ListTreeSizes.Default);

            var shown = ListTreeSnapshot.Of(scene.Tree.Roots).VisibleRows();
            Assert.Equal(shown.Select(row => row.Key), arrangement.Rows.Select(row => row.Key));
            Assert.Equal(arrangement.Rows.Select(row => row.Key), arrangement.Layout.Places.Select(place => place.Key));
            Assert.DoesNotContain(arrangement.Rows, row => row.Node != null && row.Node.Parent == scene.LayerNodeOfA(0));
        }

        [Fact]
        public void ArrangementOf_TheSizes_PlacesTheRowsWithThem()
        {
            var scene = CreateScene([2], s => AddKinds(s.Layer));
            var sizes = new ListTreeSizes(20, 4, 10);

            var arrangement = ListTreeArrangement.Of(scene.Tree.Roots, sizes);

            Assert.Equal(arrangement.Rows.Count * sizes.Pitch - sizes.Gap, arrangement.Layout.TotalHeight);
            Assert.All(arrangement.Layout.Places, place => Assert.Equal(place.Depth * 10d, place.Indent));
        }

        [Fact]
        public void Empty_Arrangement_HasNoRowsAndNoHeight()
        {
            Assert.Empty(ListTreeArrangement.Empty.Rows);
            Assert.Equal(0, ListTreeArrangement.Empty.Layout.TotalHeight);
        }

        [Fact]
        public void Around_AView_AddsHalfAViewAboveAndBelow()
        {
            var window = ListTreeRowWindow.Around(1000, 600);

            Assert.Equal(700, window.Top);
            Assert.Equal(1900, window.Bottom);
        }

        [Theory]
        [InlineData(670, 32, true)]
        [InlineData(1890, 32, true)]
        [InlineData(668, 32, false)]
        [InlineData(1900, 32, false)]
        public void Overlaps_ARow_IsTrueWhenAPartOfTheRowIsInTheWindow(double top, double height, bool expected)
        {
            var window = new ListTreeRowWindow(700, 1900);

            Assert.Equal(expected, window.Overlaps(top, height));
        }

        [Theory]
        [InlineData(1000, true)]
        [InlineData(1100, true)]
        [InlineData(1200, false)]
        [InlineData(800, false)]
        public void Holds_AScrolledView_IsTrueUntilAQuarterOfAViewIsLeft(double viewTop, bool expected)
        {
            var window = ListTreeRowWindow.Around(1000, 600);

            Assert.Equal(expected, window.Holds(viewTop, 600));
        }

        [Fact]
        public void All_AnyRow_OverlapsAndHolds()
        {
            Assert.True(ListTreeRowWindow.All.Overlaps(1e9, 32));
            Assert.True(ListTreeRowWindow.All.Holds(1e9, 600));
        }
    }
}
