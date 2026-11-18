using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Panels;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;
using Project = CMiX.Core.Compositing.Project;
using CMiX.Studio.Avalonia.Tests.Support;

namespace CMiX.Studio.Avalonia.Tests
{
    // The two bands behind the rows of the tree. They are a pattern behind everything: every row sits on a band, a row and
    // the next one are on different bands, and opening or closing a row does not change the pattern.
    public class ProjectTreeStripesTests : IDisposable
    {
        private readonly List<Window> windows = new();

        public void Dispose()
        {
            foreach (var window in windows)
                window.Close();
        }

        private sealed class Screen
        {
            public Window Window { get; init; } = null!;
            public ProjectTreeView View { get; init; } = null!;
            public ProjectTree Tree { get; init; } = null!;
            public Composition A { get; init; } = null!;

            public RowStripes Stripes => View.GetVisualDescendants().OfType<RowStripes>().Single();
            public TreeView TreeView => View.GetVisualDescendants().OfType<TreeView>().First();

            public Border HeaderOf(TreeNode node) =>
                View.GetVisualDescendants().OfType<TreeViewItem>().First(item => ReferenceEquals(item.DataContext, node))
                    .GetVisualDescendants().OfType<Border>().First(border => border.Name == "RowBorder");
        }

        // A holds two layers, B holds one.
        private Screen Show(int moreLayers = 0)
        {
            var provider = TestServiceProviderFactory.Create();
            var project = provider.GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));
            project.CompositionManager.AddItem(typeof(Composition));
            var a = (Composition)project.CompositionManager.ManagerData.Items[0];
            var b = (Composition)project.CompositionManager.ManagerData.Items[1];
            for (var i = 0; i < 2 + moreLayers; i++)
                a.LayerManager.AddItem(typeof(Layer));
            b.LayerManager.AddItem(typeof(Layer));

            var tree = TreeFixtures.CreateTree(provider);
            var view = new ProjectTreeView { DataContext = tree };
            var window = new Window { Content = view, Width = 400, Height = 900 };
            windows.Add(window);
            window.Show();
            Settle(window);
            var screen = new Screen { Window = window, View = view, Tree = tree, A = a };
            WithoutAnimation(screen);
            return screen;
        }

        // The panels of the rows open and close at once, so a row is never measured in the middle of the animation.
        private static void WithoutAnimation(Screen screen)
        {
            foreach (var panel in screen.View.GetVisualDescendants().OfType<CollapsePanel>())
                panel.Duration = TimeSpan.Zero;
        }

        // Lets the animation of the rows end and the layout run.
        private static void Settle(Window window)
        {
            Dispatcher.UIThread.RunJobs();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick(60);
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            Dispatcher.UIThread.RunJobs();
        }

        // The nodes that are shown, from the top.
        private static IEnumerable<TreeNode> Visible(IEnumerable<TreeNode> nodes) =>
            nodes.SelectMany(node => new[] { node }.Concat(node.IsExpanded ? Visible(node.Children) : Enumerable.Empty<TreeNode>()));

        private static double WindowTop(Screen screen, Control control) => control.TranslatePoint(new Point(0, 0), screen.Window)!.Value.Y;

        // The band under a row: its brush, and the distance between the top of the row and the top of the band.
        private static (double Opacity, double Gap) BandUnder(Screen screen, TreeNode node)
        {
            var top = WindowTop(screen, screen.HeaderOf(node));
            var stripesTop = WindowTop(screen, screen.Stripes);
            var band = screen.Stripes.Bands().OrderBy(b => Math.Abs(stripesTop + b.Band.Top - top)).First();
            return (band.Opacity, stripesTop + band.Band.Top - top);
        }

        private static void AssertEveryRowIsOnABand_AndNextRowsOnOtherBands(Screen screen)
        {
            var rows = Visible(screen.Tree.Roots).ToList();
            var bands = rows.Select(node => BandUnder(screen, node)).ToList();

            Assert.All(bands, band => Assert.InRange(band.Gap, -0.5, 0.5));
            for (var i = 1; i < bands.Count; i++)
                Assert.NotEqual(bands[i - 1].Opacity, bands[i].Opacity);
        }

        [AvaloniaFact]
        public void Show_EveryRow_IsOnABandAndTheNextOnTheOther()
        {
            var screen = Show();

            AssertEveryRowIsOnABand_AndNextRowsOnOtherBands(screen);
        }

        [AvaloniaFact]
        public void Close_ARow_LeavesEveryRowOnABand()
        {
            var screen = Show();

            screen.Tree.CompositionNodes()[0].Children[0].Children[0].IsExpanded = false;
            Settle(screen.Window);
            AssertEveryRowIsOnABand_AndNextRowsOnOtherBands(screen);

            screen.Tree.CompositionNodes()[0].IsExpanded = false;
            Settle(screen.Window);
            AssertEveryRowIsOnABand_AndNextRowsOnOtherBands(screen);

            screen.Tree.CompositionNodes()[0].IsExpanded = true;
            Settle(screen.Window);
            AssertEveryRowIsOnABand_AndNextRowsOnOtherBands(screen);
        }

        [AvaloniaFact]
        public void Toggle_ARow_DoesNotChangeTheBands()
        {
            var screen = Show();
            var before = screen.Stripes.Bands().Select(band => (band.Band, band.Opacity)).ToList();

            screen.Tree.CompositionNodes()[0].IsExpanded = false;
            Settle(screen.Window);
            var closed = screen.Stripes.Bands().Select(band => (band.Band, band.Opacity)).ToList();

            screen.Tree.CompositionNodes()[0].IsExpanded = true;
            Settle(screen.Window);
            var open = screen.Stripes.Bands().Select(band => (band.Band, band.Opacity)).ToList();

            Assert.Equal(before, closed);
            Assert.Equal(before, open);
        }

        [AvaloniaFact]
        public void Scroll_TheTree_KeepsTheRowsOnTheBands()
        {
            var screen = Show(moreLayers: 40);
            var viewer = screen.TreeView.GetVisualDescendants().OfType<ScrollViewer>().First();
            var bandsBefore = screen.Stripes.Bands().Select(band => band.Band.Top).ToList();

            viewer.Offset = new Vector(0, 7);
            Settle(screen.Window);

            Assert.True(viewer.Offset.Y > 0, "The tree did not scroll.");
            Assert.NotEqual(bandsBefore, screen.Stripes.Bands().Select(band => band.Band.Top).ToList());
            var stripesTop = WindowTop(screen, screen.Stripes);
            var rows = Visible(screen.Tree.Roots)
                .Where(node => screen.TreeView.GetVisualDescendants().OfType<TreeViewItem>().Any(item => ReferenceEquals(item.DataContext, node)))
                .Where(node => WindowTop(screen, screen.HeaderOf(node)) >= stripesTop && WindowTop(screen, screen.HeaderOf(node)) + 24 <= stripesTop + screen.Stripes.Bounds.Height)
                .ToList();
            var bands = rows.Select(node => BandUnder(screen, node)).ToList();
            Assert.NotEmpty(bands);
            Assert.All(bands, band => Assert.InRange(band.Gap, -0.5, 0.5));
        }

        [AvaloniaFact]
        public void TheStripes_AreBehindTheTree_AndFillTheSpaceAboveTheToolbar()
        {
            var screen = Show();
            var cell = (Grid)screen.Stripes.GetVisualParent()!;

            Assert.True(cell.Children.IndexOf(screen.Stripes) < cell.Children.IndexOf(screen.TreeView));
            Assert.Equal(cell.Bounds.Size, screen.Stripes.Bounds.Size);
            Assert.False(screen.Stripes.IsHitTestVisible);
        }

        [AvaloniaFact]
        public void TheStripes_UseAColorThatTheThemeHasAlready_AndEightyPercentOfItForTheOtherBand()
        {
            var screen = Show();
            var theme = (ISolidColorBrush)Application.Current!.FindResource("PanelBackgroundLevel1Brush")!;

            var brush = Assert.IsAssignableFrom<ISolidColorBrush>(screen.Stripes.Brush);

            Assert.Equal(theme.Color, brush.Color);
            Assert.Equal(0.8, screen.Stripes.AlternateOpacity);
            // No theme color is darker than this one except the window itself, so the stripes are darker by their own opacity.
            Assert.Equal(0.6, screen.Stripes.Opacity);
        }

        [AvaloniaTheory]
        [InlineData("RowStripeA")]
        [InlineData("RowStripeB")]
        [InlineData("RowStripeABrush")]
        [InlineData("RowStripeBBrush")]
        public void Check_TheTheme_HasNoColorOnlyForTheStripes(string key)
        {
            Assert.False(Application.Current!.TryFindResource(key, out _));
        }

        [AvaloniaFact]
        public void TheRows_AreOneRowHeightPlusOneGapFromEachOther_WhateverIsOpen()
        {
            var screen = Show();
            var pitch = Convert.ToDouble(Application.Current!.FindResource("ControlHeightMD")) + Convert.ToDouble(screen.View.Resources["TreeRowGap"]);

            foreach (var open in new[] { true, false })
            {
                screen.Tree.CompositionNodes()[0].Children[0].Children[0].IsExpanded = open;
                screen.Tree.CompositionNodes()[0].Children[0].Children[1].IsExpanded = open;
                Settle(screen.Window);

                var tops = Visible(screen.Tree.Roots).Select(node => WindowTop(screen, screen.HeaderOf(node))).ToList();
                for (var i = 1; i < tops.Count; i++)
                    Assert.InRange(tops[i] - tops[i - 1], pitch - 0.5, pitch + 0.5);
            }
        }
    }
}
