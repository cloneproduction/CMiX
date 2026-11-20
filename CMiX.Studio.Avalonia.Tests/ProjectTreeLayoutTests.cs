using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
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
using Entity = CMiX.Core.Compositing.Entity;
using Layer = CMiX.Core.Compositing.Layer;
using Project = CMiX.Core.Compositing.Project;

namespace CMiX.Studio.Avalonia.Tests
{
    // The left side of the tree: a gutter, the indent outside the fill of a row, and a space between two rows.
    public class ProjectTreeLayoutTests : IDisposable
    {
        private Window? window;

        public void Dispose() => window?.Close();

        private void Settle() => TreeTestScreen.Settle(window!);

        // A composition with a layer that has an entity.
        private (TreeViewProject View, ProjectTree Tree) Show(double scale = 1.0)
        {
            var scene = CreateScene([2], s => s.Layer.ModelEntityManager.AddItem(typeof(Entity)));
            var screen = TreeTestScreen.Show(scene.Tree, 500, 600, scale);
            window = screen.Window;
            return (screen.View, scene.Tree);
        }

        private Rect BoundsIn(Control control) => new(control.TranslatePoint(new Point(0, 0), window!)!.Value, control.Bounds.Size);

        [AvaloniaFact]
        public void Show_TheRows_KeepTheirFillsOffTheLeftBorder()
        {
            var (view, tree) = Show();
            var tops = All(tree.Roots).Select(node => BoundsIn(RowOf(view, node)).Left).ToList();

            Assert.All(tops, left => Assert.True(left >= 8, $"A fill starts at {left}, inside the gutter."));
            // The top row, the Compositions row, starts after the gutter (8) and its chevron (16).
            Assert.Equal(24, BoundsIn(RowOf(view, tree.CompositionsNode())).Left);
        }

        [AvaloniaFact]
        public void Show_TheChevron_IsLeftOfTheFillOfItsLevel()
        {
            var (view, tree) = Show();

            foreach (var node in All(tree.Roots).Where(node => node.Children.Count > 0))
            {
                var chevron = ItemOf(view, node).GetVisualDescendants().OfType<ToggleButton>().First(toggle => toggle.Name == "PART_ExpandCollapseChevron");
                var fill = BoundsIn(RowOf(view, node));
                var chevronBounds = BoundsIn(chevron);

                Assert.Equal(16, chevronBounds.Width);
                Assert.True(chevronBounds.Right <= fill.Left + 0.01, $"The chevron of {node.Header} is inside the fill.");
                Assert.Equal(fill.Left, chevronBounds.Right, 3);
                Assert.Equal(fill.Top, chevronBounds.Top, 3);
            }
        }

        [AvaloniaFact]
        public void Show_TheChevronOfAChild_IsOneIndentRightOfItsParent()
        {
            var (view, tree) = Show();
            var composition = tree.CompositionNodes()[0];
            var layers = composition.Children[0];
            var layer = layers.Children[0];

            double ChevronLeft(TreeNode node) =>
                BoundsIn(ItemOf(view, node).GetVisualDescendants().OfType<ToggleButton>().First(toggle => toggle.Name == "PART_ExpandCollapseChevron")).Left;

            Assert.Equal(16, ChevronLeft(composition) - ChevronLeft(tree.CompositionsNode()));
            Assert.Equal(16, ChevronLeft(layers) - ChevronLeft(composition));
            Assert.Equal(16, ChevronLeft(layer) - ChevronLeft(layers));
        }

        [AvaloniaFact]
        public void Show_TheFillOfAChild_StartsOneIndentRightOfItsParent()
        {
            var (view, tree) = Show();
            var composition = tree.CompositionNodes()[0];
            var layers = composition.Children[0];
            var layer = layers.Children[0];
            var entities = layer.Children.Single(child => child.Group is EntitiesGroup);

            var compositionLeft = BoundsIn(RowOf(view, composition)).Left;
            var layersLeft = BoundsIn(RowOf(view, layers)).Left;
            var layerLeft = BoundsIn(RowOf(view, layer)).Left;
            var entitiesLeft = BoundsIn(RowOf(view, entities)).Left;

            Assert.Equal(16, layersLeft - compositionLeft);
            Assert.Equal(16, layerLeft - layersLeft);
            Assert.Equal(16, entitiesLeft - layerLeft);
        }

        [AvaloniaFact]
        public void Show_AnEmptyRow_IsIndentedLikeItsSiblingsWithADisabledChevron()
        {
            var (view, tree) = Show();
            var layers = tree.CompositionNodes()[0].Children[0];
            var empty = tree.CompositionNodes()[0].Children.Single(child => child.Group is FiltersGroup);
            Assert.Empty(empty.Children);

            Assert.Equal(BoundsIn(RowOf(view, layers)).Left, BoundsIn(RowOf(view, empty)).Left, 3);
            var chevron = ItemOf(view, empty).GetVisualDescendants().OfType<ToggleButton>().First(toggle => toggle.Name == "PART_ExpandCollapseChevron");
            Assert.False(chevron.IsHitTestVisible);
        }

        [AvaloniaFact]
        public void Show_EveryFill_EndsAtTheSamePlace()
        {
            var (view, tree) = Show();

            var rights = All(tree.Roots).Select(node => BoundsIn(RowOf(view, node)).Right).Distinct().ToList();

            Assert.Single(rights);
        }

        [AvaloniaTheory]
        [InlineData(1.0)]
        [InlineData(1.25)]
        [InlineData(1.5)]
        [InlineData(2.0)]
        public void Show_AtEveryScaling_PutsWholePixelsBetweenTwoRows(double scale)
        {
            var (view, tree) = Show(scale);
            var layers = tree.CompositionNodes()[0].Children;
            var first = BoundsIn(RowOf(view, layers[0]));
            var gap = Convert.ToDouble(view.Resources["TreeRowGap"]);

            Assert.Equal(Math.Round(gap * scale), gap * scale, 3);
            Assert.True(gap * scale >= 3 * Math.Min(scale, 1) - 1e-6);
            Assert.Equal(gap, BoundsIn(RowOf(view, layers[0].Children[0])).Top - (first.Top + first.Height), 3);
        }

        [Theory]
        [InlineData(1.0, 3.0)]
        [InlineData(1.25, 3.2)]
        [InlineData(1.5, 10.0 / 3.0)]
        [InlineData(2.0, 3.0)]
        public void Show_TheGap_SnapsToWholePixels(double scale, double expected)
        {
            var (_, gap) = TreeViewProject.SnapToPixels(24, TreeViewProject.RowGap, scale);

            Assert.Equal(expected, gap, 6);
            Assert.Equal(Math.Round(gap * scale), gap * scale, 6);
        }

        [AvaloniaFact]
        public void Show_ARowAndItsToggles_HaveOnePixelBetweenThem()
        {
            var (view, tree) = Show();

            Assert.Equal(1, Convert.ToDouble(view.Resources["TreeRowSpacing"]));
            Assert.Equal(3, Convert.ToDouble(view.Resources["TreeRowGap"]));
        }

        [AvaloniaFact]
        public void DragOver_ARow_PutsTheDropLineInTheSpaceAroundTheRowAtFullWidth()
        {
            var (view, tree) = Show();
            var layer = tree.CompositionNodes()[0].Children[0].Children[0];
            var item = ItemOf(view, layer);
            var line = item.GetVisualDescendants().OfType<Border>().Last(border => border.Name == "DropLine");
            var row = RowOf(view, layer);

            ((IPseudoClasses)item.Classes).Set(":droptop", true);
            Settle();

            Assert.True(line.IsEffectivelyVisible, "line.IsEffectivelyVisible");
            var thin = Convert.ToDouble(view.Resources["TreeDropLineThickness"]);
            var topMargin = (Thickness)view.Resources["TreeDropLineTopMargin"]!;
            var gapMargin = (Thickness)view.Resources["TreeDropLineGapMargin"]!;
            Assert.Equal(BoundsIn(item).Left, BoundsIn(line).Left, 3);
            Assert.Equal(BoundsIn(item).Width, BoundsIn(line).Width, 3);
            Assert.Equal(thin, BoundsIn(line).Height, 3);
            Assert.Equal(BoundsIn(item).Top + topMargin.Top, BoundsIn(line).Top, 3);

            ((IPseudoClasses)item.Classes).Set(":droptop", false);
            ((IPseudoClasses)item.Classes).Set(":dropgap", true);
            Settle();
            var inGap = BoundsIn(line);
            var gap = Convert.ToDouble(view.Resources["TreeRowGap"]);

            Assert.Equal(BoundsIn(item).Left, inGap.Left, 3);
            Assert.Equal(BoundsIn(item).Width, inGap.Width, 3);
            Assert.Equal(BoundsIn(item).Top + gapMargin.Top, inGap.Top, 3);
            Assert.Equal(thin, inGap.Height, 3);
            Assert.Equal(BoundsIn(row).Bottom + (gap - thin) / 2, inGap.Top, 3);
        }

        [AvaloniaFact]
        public void Select_ARow_MakesItsNameWhite()
        {
            var (view, tree) = Show();
            var layer = tree.CompositionNodes()[0].Children[0].Children[0];
            var other = tree.CompositionNodes()[0].Children[0].Children[1];

            tree.SelectedNode = layer;
            Settle();

            static Color NameColor(TreeViewProject view, TreeNode node) =>
                ((ISolidColorBrush)ItemOf(view, node).GetVisualDescendants().OfType<TextBlock>().First(text => text.Name == "TextDisplay").Foreground!).Color;

            var white = ((ISolidColorBrush)Application.Current!.FindResource("ActiveForegroundBrush")!).Color;
            var light = ((ISolidColorBrush)Application.Current!.FindResource("TreeTextRestBrush")!).Color;
            Assert.Equal(white, NameColor(view, layer));
            Assert.Equal(light, NameColor(view, other));
        }
    }
}
