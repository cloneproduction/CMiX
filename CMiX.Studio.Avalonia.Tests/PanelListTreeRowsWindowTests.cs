// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // A big tree: only the rows near the view of the scroller have a control.
    public class PanelListTreeRowsWindowTests : IDisposable
    {
        private const int Layers = 200;

        private readonly Scene scene = CreateScene([Layers]);
        private readonly Window window;
        private readonly ListTreeProject view;

        public PanelListTreeRowsWindowTests()
        {
            TestServiceProviderFactory.ExpandAll(scene.Tree);
            view = new ListTreeProject { DataContext = scene.Tree };
            view.Rows.MoveDuration = TimeSpan.Zero;
            window = new Window { Content = view, Width = 400, Height = 600 };
            window.Show();
            Settle();
        }

        public void Dispose() => window.Close();

        private PanelListTreeRows Panel => view.Rows;

        private ScrollViewer Scroller => view.GetVisualDescendants().OfType<ScrollViewer>().First(scroller => scroller.Name == "scroller");

        private ListTreePlace LastPlace => Panel.Layout.Places[^1];

        private void Settle() => TreeTestScreen.Settle(window);

        // The window is the view and half a view above and below it.
        private int MostRowsInTheWindow => (int)Math.Ceiling(2 * Scroller.Viewport.Height / Panel.Sizes.Pitch) + 2;

        [AvaloniaFact]
        public void RealizedRows_ABigTreeInASmallView_HaveOnlyTheRowsNearTheView()
        {
            Assert.True(Panel.Layout.Places.Count > Layers);
            Assert.InRange(Panel.RealizedRows.Count(), 1, MostRowsInTheWindow);
            Assert.NotNull(Panel.RealizedRowOf(Panel.Layout.Places[0].Key));
            Assert.Null(Panel.RealizedRowOf(LastPlace.Key));
        }

        [AvaloniaFact]
        public void RowOf_ARowOutsideTheWindow_MakesItsControlAtItsPlace()
        {
            var row = Assert.IsType<RowListTree>(Panel.RowOf(LastPlace.Key));

            Assert.Same(row, Panel.RealizedRowOf(LastPlace.Key));
            Assert.Equal(LastPlace.Y, row.Bounds.Y);
            Assert.Equal(LastPlace, row.Place);
        }

        [AvaloniaFact]
        public void OnScrollChanged_AScrollToTheEnd_MovesTheControlsToTheRowsInView()
        {
            Scroller.Offset = new Vector(0, Scroller.Extent.Height);
            Settle();

            Assert.NotNull(Panel.RealizedRowOf(LastPlace.Key));
            Assert.Null(Panel.RealizedRowOf(Panel.Layout.Places[0].Key));
            Assert.InRange(Panel.RealizedRows.Count(), 1, MostRowsInTheWindow);
        }
    }
}
