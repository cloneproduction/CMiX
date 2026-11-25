// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using CompositionModel = CMiX.Core.Compositing.Composition;
using LayerModel = CMiX.Core.Compositing.Layer;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // A search that changes the rows in the row window gives the row controls of the rows that leave to the rows that come in.
    public class ListTreeRowControlsTests : IDisposable
    {
        private readonly Scene scene = CreateScene([4, 4, 4], s =>
        {
            foreach (var composition in s.Project.CompositionManager.ManagerData.Items.Cast<CompositionModel>())
                foreach (var layer in composition.LayerManager.ManagerData.Items.Cast<LayerModel>())
                    AddKinds(layer);
        });

        private readonly ListTreeTestScreen screen;

        public ListTreeRowControlsTests()
        {
            foreach (var node in ListTreeTestScreen.All(scene.Tree.Roots).Where(node => node.Children.Count > 0))
                node.IsExpanded = false;
            screen = ListTreeTestScreen.Show(scene.Tree, 400, 400);
        }

        public void Dispose() => screen.Window.Close();

        private PanelListTreeRows Rows => screen.View.Rows;

        private List<RowListTree> RowControls() => Rows.Children.OfType<RowListTree>().ToList();

        private void Search(string text)
        {
            scene.Tree.SearchText = text;
            screen.Settle();
        }

        // A row that moves out of the window keeps its control for its slide, so only such a row can need a new control.
        [AvaloniaFact]
        public void ShowRows_MatchesThatChange_ReuseTheRowControls()
        {
            Search("Translate");
            var before = RowControls().ToDictionary(row => row, row => row.DataContext);

            Search("Blur");
            var after = RowControls();
            var added = after.Count(row => !before.ContainsKey(row));
            var slidingOut = after.Count(row => !Rows.RowWindow.Overlaps(row.Place!.Y, row.Place.RowHeight));

            Assert.InRange(added, 0, slidingOut);
            Assert.Contains(after, row => before.TryGetValue(row, out var node) && !ReferenceEquals(node, row.DataContext));
        }

        [AvaloniaFact]
        public void ShowRows_AReusedRow_ShowsItsNewNode()
        {
            Search("Translate");
            Search("Blur");

            Assert.All(Rows.RealizedRows, row =>
            {
                var node = Assert.IsType<TreeNode>(row.DataContext);
                Assert.Same(row, Rows.RealizedRowOf(row.Place!.Key));
                Assert.Equal(Rows.Layout.PlaceOf(row.Place.Key), row.Place);
                Assert.Same(row, Rows.RowOf(node));
                Assert.Equal(row.Place.Y, PanelListTreeRows.GetTop(row));
                Assert.Equal(0, ListTreeRowSlide.OffsetOf(row));
            });
        }

        [AvaloniaFact]
        public void ShowRows_AFocusedRow_IsNotReused()
        {
            Search("Blur");
            var focused = RowControls().Last(row => row.DataContext is TreeNode { Item: not null });
            var node = focused.DataContext;
            focused.Focus();
            screen.Settle();

            Search("Translate");

            var place = Rows.ShownPlaceOf(Assert.IsType<TreeNode>(node))!;
            Assert.False(Rows.RowWindow.Overlaps(place.Y, place.RowHeight));
            Assert.True(focused.IsKeyboardFocusWithin);
            Assert.Same(node, focused.DataContext);
            Assert.Contains(focused, RowControls());
        }
    }
}
