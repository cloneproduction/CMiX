// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // The card frames of a big tree while it scrolls.
    public class ListTreeSurfacesTests : IDisposable
    {
        private const int Compositions = 40;

        private readonly Scene scene = CreateScene(Enumerable.Repeat(2, Compositions).ToArray());
        private readonly Window window;
        private readonly ListTreeProject view;

        public ListTreeSurfacesTests()
        {
            TestServiceProviderFactory.ExpandAll(scene.Tree);
            var screen = ListTreeTestScreen.Show(scene.Tree, 400, 600);
            view = screen.View;
            window = screen.Window;
        }

        public void Dispose() => window.Close();

        private PanelListTreeRows Panel => view.Rows;

        private ScrollViewer Scroller => view.Scroller;

        // A scroll up brings frames that come before the kept frames in tree order.
        [AvaloniaFact]
        public void Show_ANewCardFrame_TakesNoOtherFrameOutOfThePanel()
        {
            Scroller.Offset = new Vector(0, Scroller.Extent.Height);
            window.Settle();
            var cardsBefore = Panel.Surfaces.Areas.ToDictionary(area => area, area => area.Place!.Key);
            var detached = new List<FrameListTreeCard>();
            foreach (var area in cardsBefore.Keys)
                area.DetachedFromVisualTree += (_, _) => detached.Add(area);

            Scroller.Offset = new Vector(0, Scroller.Offset.Y - Scroller.Viewport.Height);
            window.Settle();

            var kept = Panel.Surfaces.Areas.Where(area => cardsBefore.TryGetValue(area, out var key) && key == area.Place!.Key).ToList();
            Assert.Contains(Panel.Surfaces.Areas, area => !cardsBefore.ContainsValue(area.Place!.Key));
            Assert.NotEmpty(kept);
            Assert.Empty(detached.Where(kept.Contains));
        }

        // The number of cards in a view changes with the scroll, so the test compares with the most cards that one view shows.
        [AvaloniaFact]
        public void Show_ScrollDown_ReusesTheCardFrames()
        {
            var seen = new HashSet<FrameListTreeCard>();
            var most = 0;

            for (var view = 0; view < 4; view++)
            {
                ScrollDownOneView();
                seen.UnionWith(AllFrames());
                most = Math.Max(most, Panel.Surfaces.Areas.Count);
            }

            Assert.Equal(most, seen.Count);
        }

        [AvaloniaFact]
        public void Show_ASpareFrame_IsHiddenAndHasNoNode()
        {
            ScrollDownOneView();
            Scroller.Offset = new Vector(0, 0);
            window.Settle();

            var spares = AllFrames().Except(Panel.Surfaces.Areas).ToList();
            Assert.NotEmpty(spares);
            Assert.All(spares, area =>
            {
                Assert.False(area.IsVisible);
                Assert.Null(area.DataContext);
            });
            Assert.All(Panel.Surfaces.Areas, area => Assert.True(area.IsVisible));
        }

        private void ScrollDownOneView()
        {
            Scroller.Offset = new Vector(0, Scroller.Offset.Y + Scroller.Viewport.Height);
            window.Settle();
        }

        private List<FrameListTreeCard> AllFrames() => Panel.Children.OfType<FrameListTreeCard>().ToList();
    }
}
