// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    public class ListTreeArrangementTests
    {
        private readonly Scene scene = CreateScene([2], s => AddKinds(s.Layer));

        [Fact]
        public void Of_ATreeWithAClosedCard_GivesTheShownRowsInOrderWithTheirPlaces()
        {
            scene.LayerNodeOfA(0).IsExpanded = false;

            var arrangement = ListTreeArrangement.Of(scene.Tree.Roots, ListTreeSizes.Default);

            var shown = ListTreeSnapshot.Of(scene.Tree.Roots).VisibleRows();
            Assert.Equal(shown.Select(row => row.Key), arrangement.Rows.Select(row => row.Key));
            Assert.Equal(arrangement.Rows.Select(row => row.Key), arrangement.Layout.Places.Select(place => place.Key));
            Assert.DoesNotContain(arrangement.Rows, row => row.Node != null && row.Node.Parent == scene.LayerNodeOfA(0));
        }

        [Fact]
        public void Of_TheSizes_PlacesTheRowsWithThem()
        {
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
    }
}
