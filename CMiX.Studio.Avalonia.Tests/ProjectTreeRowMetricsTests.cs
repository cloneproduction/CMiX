using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Panels;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using static CMiX.Studio.Avalonia.Tests.Support.TreeTestScreen;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;
using Project = CMiX.Core.Compositing.Project;

namespace CMiX.Studio.Avalonia.Tests
{
    // On a screen with a scaling like 125%, every panel rounds its size to whole pixels, and the rows of a long tree would not be
    // exactly one pitch apart. The tree gives its rows sizes in whole pixels, so every row is one same pitch from the next.
    public class ProjectTreeRowMetricsTests : IDisposable
    {
        private readonly List<Window> windows = new();

        public void Dispose()
        {
            foreach (var window in windows)
                window.Close();
        }

        // The window of the test gets a scaling of the screen.
        private (Window Window, TreeViewProject View, ProjectTree Tree) Show(double scale, int layers)
        {
            var tree = CreateScene([layers]).Tree;
            var screen = TreeTestScreen.Show(tree, 400, 900, scale);
            windows.Add(screen.Window);
            return (screen.Window, screen.View, tree);
        }

        private static List<Border> RowsOf(TreeViewProject view) =>
            view.GetVisualDescendants().OfType<TreeViewItem>()
                .Select(item => item.GetVisualDescendants().OfType<Border>().First(border => border.Name == "RowBorder"))
                .Where(row => row.IsEffectivelyVisible)
                .ToList();

        private static double TopIn(Visual control, Visual root) => control.TranslatePoint(new Point(0, 0), root)!.Value.Y;

        // Every row is a whole number of pixels from the next one, so there is nothing to round.
        [AvaloniaTheory]
        [InlineData(1.0)]
        [InlineData(1.1)]
        [InlineData(1.25)]
        [InlineData(1.5)]
        [InlineData(1.75)]
        [InlineData(2.0)]
        public void TheRows_AreAWholeNumberOfPixelsApart_AndAllTheSamePitch(double scale)
        {
            var (window, view, _) = Show(scale, layers: 30);
            var tops = RowsOf(view).Select(row => TopIn(row, window)).ToList();
            var pitches = tops.Zip(tops.Skip(1), (a, b) => (b - a) * scale).ToList();

            Assert.All(pitches, pitch => Assert.Equal(Math.Round(pitch), pitch, 3));
            Assert.Single(pitches.Select(pitch => Math.Round(pitch)).Distinct());
        }

        [AvaloniaTheory]
        [InlineData(1.0)]
        [InlineData(1.25)]
        [InlineData(1.5)]
        public void TheRows_AreOneRowHeightPlusOneSpaceFromEachOther_WhateverIsOpen(double scale)
        {
            var (window, view, tree) = Show(scale, layers: 4);
            var layers = tree.CompositionNodes()[0].Children;

            foreach (var open in new[] { true, false })
            {
                foreach (var layer in layers)
                    layer.IsExpanded = open;
                Settle(window);

                var tops = RowsOf(view).Select(row => TopIn(row, window)).ToList();
                var pitches = tops.Zip(tops.Skip(1), (a, b) => (b - a) * scale).Select(pitch => Math.Round(pitch)).Distinct().ToList();
                Assert.Single(pitches);
            }
        }

        [Theory]
        [InlineData(1.0, 24.0, 1.0)]
        [InlineData(1.25, 24.0, 0.8)]
        [InlineData(1.5, 24.0, 1.0 + 1.0 / 3.0)]
        [InlineData(2.0, 24.0, 1.0)]
        public void TheSnappedSizes_AreWholePixels(double scale, double expectedHeight, double expectedSpacing)
        {
            var (height, spacing) = TreeViewProject.SnapToPixels(24, 1, scale);

            Assert.Equal(expectedHeight, height, 6);
            Assert.Equal(expectedSpacing, spacing, 6);
            Assert.Equal(Math.Round(height * scale), height * scale, 6);
            Assert.Equal(Math.Round(spacing * scale), spacing * scale, 6);
        }

        [Fact]
        public void ASizeThatIsNotAWholeNumberOfPixels_GoesUpToTheNextPixel()
        {
            var (height, spacing) = TreeViewProject.SnapToPixels(24, 1, 1.1);

            Assert.Equal(27 / 1.1, height, 6);
            Assert.Equal(1 / 1.1, spacing, 6);
        }

        [AvaloniaFact]
        public void TreeRowMetrics_RowGap_EqualsTheToken()
        {
            Assert.Equal(TreeRowMetrics.RowGap, Convert.ToDouble(Application.Current!.FindResource("TreeRowGap")));
        }
    }
}
