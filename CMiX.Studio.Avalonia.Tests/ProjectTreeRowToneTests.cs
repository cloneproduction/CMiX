using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Themes;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using static CMiX.Studio.Avalonia.Tests.Support.TreeTestScreen;
using AvaloniaColor = Avalonia.Media.Color;
using Composition = CMiX.Core.Compositing.Composition;
using Entity = CMiX.Core.Compositing.Entity;
using Layer = CMiX.Core.Compositing.Layer;
using Project = CMiX.Core.Compositing.Project;

namespace CMiX.Studio.Avalonia.Tests
{
    // The fill behind a row is the tone of its kind: one step brighter under the pointer, and one more when the row is selected.
    public class ProjectTreeRowToneTests : IDisposable
    {
        private readonly List<Window> windows = new();
        private readonly CategoryPalette palette = CategoryPalette.FromApplication();

        public void Dispose()
        {
            foreach (var window in windows)
                window.Close();
        }

        private (Window Window, TreeViewProject View, ProjectTree Tree) Show()
        {
            var tree = CreateScene([2], s => s.Layer.ModelEntityManager.AddItem(typeof(Entity))).Tree;
            var screen = TreeTestScreen.Show(tree, 400, 600, instant: false);
            windows.Add(screen.Window);
            tree.SelectedNode = null;
            Settle(screen.Window);
            return (screen.Window, screen.View, tree);
        }

        private CategoryTone ToneOf(TreeNode node) => palette.For(node.Category!.Value, node.ComposableDepth, node.KindDepth);

        private static Point CenterOf(Window window, Border row) =>
            row.TranslatePoint(new Point(row.Bounds.Width / 2, row.Bounds.Height / 2), window)!.Value;

        private static AvaloniaColor Grey(string key) => ((ISolidColorBrush)Application.Current!.FindResource(key)!).Color;

        [AvaloniaFact]
        public void Show_ARowAtRest_HasTheFillOfItsKindOrGreyForAnEmptyGroup()
        {
            var (_, view, tree) = Show();

            foreach (var node in All(tree.Roots))
                Assert.Equal(node.Item == null && node.Children.Count == 0 ? Grey("Grey300Brush") : ToneOf(node).Fill, FillOf(RowOf(view, node)));
        }

        [AvaloniaFact]
        public void AddAndRemove_TheOnlyChild_ChangesTheFillBetweenGreyAndTheKind()
        {
            var (window, view, tree) = Show();
            var layerNode = tree.CompositionNodes()[0].Children[0].Children[1];
            var entities = layerNode.Children.Single(child => child.Group is EntitiesGroup);
            var layer = (Layer)layerNode.Item!;

            Assert.Empty(entities.Children);
            Assert.Equal(Grey("Grey300Brush"), FillOf(RowOf(view, entities)));

            layer.ModelEntityManager.AddItem(typeof(Entity));
            Settle(window);

            tree.SelectedNode = null;
            Settle(window);
            Assert.Equal(ToneOf(entities).Fill, FillOf(RowOf(view, entities)));

            layer.ModelEntityManager.RemoveItem(layer.ModelEntityManager.ManagerData.Items[0]);
            tree.SelectedNode = null;
            Settle(window);

            Assert.Equal(Grey("Grey300Brush"), FillOf(RowOf(view, entities)));
        }

        [AvaloniaFact]
        public void Select_ARowWithNoChildren_GivesItABrighterGrey()
        {
            var (window, view, tree) = Show();
            var entities = tree.CompositionNodes()[0].Children[0].Children[1].Children.Single(child => child.Group is EntitiesGroup);
            Assert.Empty(entities.Children);

            tree.SelectedNode = entities;
            Settle(window);

            Assert.Equal(Grey("Grey500Brush"), FillOf(RowOf(view, entities)));
        }

        [AvaloniaFact]
        public void Show_RowsOfTwoKinds_HaveAFillPerKind()
        {
            var (_, view, tree) = Show();
            var composition = tree.CompositionNodes()[0];
            var layer = composition.Children[0].Children[0];
            var entities = layer.Children.Single(child => child.Group is EntitiesGroup);
            var entity = entities.Children[0];

            Assert.NotEqual(FillOf(RowOf(view, layer)), FillOf(RowOf(view, entities)));
            Assert.Equal(FillOf(RowOf(view, entities)), FillOf(RowOf(view, entity)));
        }

        [AvaloniaFact]
        public void Select_ARow_GivesOnlyThatRowTheSelectedFill()
        {
            var (window, view, tree) = Show();
            var layer = tree.CompositionNodes()[0].Children[0].Children[0];
            var other = tree.CompositionNodes()[0].Children[0].Children[1];

            tree.SelectedNode = layer;
            Settle(window);

            Assert.Equal(ToneOf(layer).FillSelected, FillOf(RowOf(view, layer)));
            Assert.Equal(ToneOf(other).Fill, FillOf(RowOf(view, other)));
        }

        [AvaloniaFact]
        public void Hover_ARow_GivesItTheHoverFillAndKeepsTheText()
        {
            var (window, view, tree) = Show();
            var first = tree.CompositionNodes()[0].Children[0].Children[0];
            var second = tree.CompositionNodes()[0].Children[0].Children[1];
            var text = ((ISolidColorBrush)Application.Current!.FindResource("TreeTextRestBrush")!).Color;

            window.MouseMove(CenterOf(window, RowOf(view, second)));
            Settle(window);
            Assert.Equal(ToneOf(second).FillHover, FillOf(RowOf(view, second)));
            Assert.NotEqual(ToneOf(second).FillSelected, FillOf(RowOf(view, second)));
            Assert.Equal(text, ((ISolidColorBrush)RowOf(view, second).GetValue(global::Avalonia.Controls.Documents.TextElement.ForegroundProperty)!).Color);
            Assert.Equal(ToneOf(first).Fill, FillOf(RowOf(view, first)));

            // A selected row keeps the selected fill under the pointer.
            tree.SelectedNode = second;
            Settle(window);
            Assert.Equal(ToneOf(second).FillSelected, FillOf(RowOf(view, second)));
        }

        [AvaloniaFact]
        public void Show_EachState_IsBrighterThanTheStateBefore()
        {
            var (_, _, tree) = Show();
            var tone = ToneOf(tree.CompositionNodes()[0].Children[0].Children[0]);

            static int Sum(AvaloniaColor color) => color.R + color.G + color.B;
            Assert.True(Sum(tone.Fill) < Sum(tone.FillHover));
            Assert.True(Sum(tone.FillHover) < Sum(tone.FillSelected));
        }

        // The first background that is not clear, from a control up the tree of controls.
        private static AvaloniaColor? BackgroundUnder(Visual start)
        {
            foreach (var ancestor in start.GetVisualAncestors())
            {
                var brush = ancestor switch
                {
                    Border border => border.Background,
                    Panel panel => panel.Background,
                    global::Avalonia.Controls.Presenters.ContentPresenter presenter => presenter.Background,
                    _ => null
                };

                if (brush is ISolidColorBrush solid && solid.Color.A > 0)
                    return solid.Color;
            }

            return null;
        }

        [AvaloniaFact]
        public void TreeBackground_Show_EqualsTheBackgroundUnderTheRows()
        {
            var (_, window, _) = TestServiceProviderFactory.ShowMainWindow();
            var view = window.GetVisualDescendants().OfType<TreeViewProject>().First();
            var token = ((ISolidColorBrush)Application.Current!.FindResource("TreeBackgroundBrush")!).Color;
            var rows = view.GetVisualDescendants().OfType<TreeView>().First();

            Assert.Equal(token, BackgroundUnder(rows));
            Assert.Equal(token, BackgroundUnder(view));
        }
    }
}
