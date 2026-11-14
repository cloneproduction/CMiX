using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Themes;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using static CMiX.Studio.Avalonia.Tests.Support.TreeTestScreen;
using Entity = CMiX.Core.Compositing.Entity;

namespace CMiX.Studio.Avalonia.Tests
{
    // The fill shows the selection: the selected row has the selected fill and the active text, and no frame.
    public class ProjectTreeSelectionLookTests : IDisposable
    {
        private Window? window;

        public void Dispose() => window?.Close();

        private void Settle() => TreeTestScreen.Settle(window!);

        private (ProjectTreeView View, ProjectTree Tree) Show()
        {
            var tree = CreateScene([2], s => s.Layer.ModelEntityManager.AddItem(typeof(Entity))).Tree;
            var screen = TreeTestScreen.Show(tree, 500, 700);
            window = screen.Window;
            tree.SelectedNode = null;
            Settle();
            return (screen.View, tree);
        }

        private static Color Brush(string key) => ((ISolidColorBrush)Application.Current!.FindResource(key)!).Color;

        [AvaloniaFact]
        public void Select_ARow_GivesItTheSelectedFillAndTheActiveText()
        {
            var (view, tree) = Show();
            var layer = tree.CompositionNodes()[0].Children[0].Children[0];
            var other = tree.CompositionNodes()[0].Children[0].Children[1];

            tree.SelectedNode = layer;
            Settle();

            var row = RowOf(view, layer);
            Assert.Equal(ExpectedFill(layer, ToneRole.FillSelected), FillOf(row));
            Assert.Equal(Brush("ActiveForegroundBrush"), ((ISolidColorBrush)row.GetValue(global::Avalonia.Controls.Documents.TextElement.ForegroundProperty)!).Color);
            Assert.Equal(ExpectedFill(other, ToneRole.Fill), FillOf(view, other));
            Assert.Equal(Brush("TreeTextRestBrush"), ((ISolidColorBrush)RowOf(view, other).GetValue(global::Avalonia.Controls.Documents.TextElement.ForegroundProperty)!).Color);
        }

        // The cursor follows the selection, so CursorFrame is not counted here: it is the frame of the cursor.
        [AvaloniaFact]
        public void Select_ARow_DrawsNoFrame()
        {
            var (view, tree) = Show();
            var layer = tree.CompositionNodes()[0].Children[0].Children[0];

            tree.SelectedNode = layer;
            Settle();

            var row = RowOf(view, layer);
            var borders = new[] { row }.Concat(row.GetVisualDescendants().OfType<Border>()).Where(border => border.Name != "CursorFrame");
            Assert.All(borders, border => Assert.Equal(new Thickness(0), border.BorderThickness));
            Assert.DoesNotContain(row.GetVisualDescendants().OfType<Border>(), border => border.Name is "SelectionFrame" or "FocusFrame");
        }

        [AvaloniaFact]
        public void Select_ARow_KeepsItInPlace()
        {
            var (view, tree) = Show();
            var layer = tree.CompositionNodes()[0].Children[0].Children[0];
            var row = RowOf(view, layer);
            var before = (row.Bounds, row.TranslatePoint(new Point(0, 0), window!));

            tree.SelectedNode = layer;
            Settle();

            Assert.Equal(before, (row.Bounds, row.TranslatePoint(new Point(0, 0), window!)));
        }
    }
}
