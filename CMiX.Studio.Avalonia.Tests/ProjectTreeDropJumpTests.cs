using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;
using Project = CMiX.Core.Compositing.Project;

namespace CMiX.Studio.Avalonia.Tests
{
    // A drop changes the number of rows above and under the view. The view must not jump.
    public class ProjectTreeDropJumpTests : IDisposable
    {
        private Window? window;

        public void Dispose() => window?.Close();

        private void Settle()
        {
            Dispatcher.UIThread.RunJobs();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick(60);
            window!.UpdateLayout();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            Dispatcher.UIThread.RunJobs();
        }

        // Two compositions with six open layers each: a tree that is much higher than the window.
        private (TreeViewProject View, ProjectTree Tree, ScrollViewer Scroller) Show()
        {
            var provider = TestServiceProviderFactory.Create();
            var project = provider.GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));
            project.CompositionManager.AddItem(typeof(Composition));
            foreach (var item in project.CompositionManager.ManagerData.Items.OfType<Composition>())
                for (var i = 0; i < 6; i++)
                    item.LayerManager.AddItem(typeof(Layer));

            var tree = TreeFixtures.CreateTree(provider);
            TestServiceProviderFactory.ExpandAll(tree);
            var view = new TreeViewProject { DataContext = tree };
            window = new Window { Content = view, Width = 500, Height = 500 };
            window.Show();
            Settle();
            tree.SelectedNode = null;
            Settle();
            return (view, tree, view.GetVisualDescendants().OfType<ScrollViewer>().First());
        }

        private Border RowOf(TreeViewProject view, TreeNode node) =>
            view.GetVisualDescendants().OfType<TreeViewItem>().First(item => ReferenceEquals(item.DataContext, node))
                .GetVisualDescendants().OfType<Border>().First(border => border.Name == "RowBorder");

        private double Top(TreeViewProject view, TreeNode node) => RowOf(view, node).TranslatePoint(new Point(0, 0), window!)!.Value.Y;

        private static IEnumerable<TreeNode> All(IEnumerable<TreeNode> nodes) =>
            nodes.SelectMany(node => new[] { node }.Concat(All(node.Children)));

        private Point Center(TreeViewProject view, TreeNode node)
        {
            var row = RowOf(view, node);
            return row.TranslatePoint(new Point(row.Bounds.Width / 2, row.Bounds.Height / 2), window!)!.Value;
        }

        // Drags the source row to the target row and drops it on its middle.
        private void Drop(TreeViewProject view, TreeNode source, TreeNode target)
        {
            TreeDrag.Drop(window!, Center(view, source), Center(view, target));
            Settle();
        }

        [AvaloniaFact]
        public void Drop_ALayerOnAnotherComposition_KeepsTheViewAndTheRowsAbove()
        {
            var (view, tree, scroller) = Show();
            scroller.Offset = new Vector(0, 800);
            Settle();

            var source = tree.CompositionNodes()[0].Children[0].Children.Last(layer => Top(view, layer) is > 20 and < 440);
            var target = tree.CompositionNodes()[1];
            var kept = All(tree.Roots).Where(node => !node.IsOrIsUnder(source)).First(node => Top(view, node) >= 0);
            var offset = scroller.Offset.Y;
            var top = Top(view, kept);

            Drop(view, source, target);

            Assert.Equal(7, target.Children[0].Children.Count);
            Assert.Equal(offset, scroller.Offset.Y, 1);
            Assert.Equal(top, Top(view, kept), 1);
        }

        [AvaloniaFact]
        public void Drop_ALayerInItsOwnList_KeepsTheView()
        {
            var (view, tree, scroller) = Show();
            scroller.Offset = new Vector(0, 300);
            Settle();

            var layers = tree.CompositionNodes()[0].Children[0].Children.Where(layer => Top(view, layer) is > 20 and < 440).ToList();
            var source = layers[0];
            var target = layers[^1];
            var offset = scroller.Offset.Y;

            Drop(view, source, target);

            Assert.Equal(offset, scroller.Offset.Y, 1);
        }

        [AvaloniaFact]
        public void Scroll_ToTheEnd_PutsTheLastRowNearTheTop()
        {
            var (view, tree, scroller) = Show();
            var last = All(tree.Roots).Last();

            scroller.Offset = new Vector(0, scroller.Extent.Height);
            Settle();

            Assert.True(Top(view, last) < 150, $"The last row is at {Top(view, last)}.");
        }
    }
}
