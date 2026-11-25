// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.VisualTree;
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

        private ScrollViewer Scroller => view.GetVisualDescendants().OfType<ScrollViewer>().First(scroller => scroller.Name == "scroller");

        // A scroll up brings frames that come before the kept frames in tree order.
        [AvaloniaFact]
        public void Show_ANewCardFrame_TakesNoOtherFrameOutOfThePanel()
        {
            Scroller.Offset = new Vector(0, Scroller.Extent.Height);
            window.Settle();
            var before = Panel.Surfaces.Areas.ToList();
            var detached = new List<FrameListTreeCard>();
            foreach (var area in before)
                area.DetachedFromVisualTree += (_, _) => detached.Add(area);

            Scroller.Offset = new Vector(0, Scroller.Offset.Y - Scroller.Viewport.Height);
            window.Settle();

            var kept = Panel.Surfaces.Areas.Where(before.Contains).ToList();
            Assert.Contains(Panel.Surfaces.Areas, area => !before.Contains(area));
            Assert.NotEmpty(kept);
            Assert.Empty(detached.Where(kept.Contains));
        }
    }
}
