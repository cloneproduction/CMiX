using Avalonia;
using Avalonia.Controls;
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
using Layer = CMiX.Core.Compositing.Layer;
using Project = CMiX.Core.Compositing.Project;

namespace CMiX.Studio.Avalonia.Tests
{
    // The drop mark of a row sets the pseudo classes of the row. The marker puts the bar on the right row.
    public class TreeDropMarkTests : IDisposable
    {
        private Window? window;

        public void Dispose() => window?.Close();

        // Two compositions. The first has a layer, so the row above the second composition is a row of that layer.
        private (ProjectTreeView View, ProjectTree Tree) Show()
        {
            var tree = CreateScene([1, 0]).Tree;
            TestServiceProviderFactory.ExpandAll(tree);
            var screen = TreeTestScreen.Show(tree, 500, 700, settle: false);
            window = screen.Window;
            var view = screen.View;
            Dispatcher.UIThread.RunJobs();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick(60);
            window.UpdateLayout();
            Dispatcher.UIThread.RunJobs();
            return (view, tree);
        }


        // The own mark line of an item: the last one in the tree order, because the rows under the item come before it.
        private static Border DropLineOf(TreeViewItem item) =>
            item.GetVisualDescendants().OfType<Border>().Last(border => border.Name == "DropLine");

        private static bool HasClass(TreeViewItem item, string name) => item.Classes.Contains(name);

        [AvaloniaFact]
        public void SetMark_Target_SetsTheDropTargetClass()
        {
            var item = new TreeViewItem();

            TreeDropMark.SetMark(item, DropMark.Target);

            Assert.True(HasClass(item, ":droptarget"));
            Assert.False(HasClass(item, ":dropgap"));
            Assert.False(HasClass(item, ":droptop"));
        }

        [AvaloniaFact]
        public void SetMark_GapBelow_SetsTheDropGapClass()
        {
            var item = new TreeViewItem();

            TreeDropMark.SetMark(item, DropMark.GapBelow);

            Assert.True(HasClass(item, ":dropgap"));
            Assert.False(HasClass(item, ":droptarget"));
            Assert.False(HasClass(item, ":droptop"));
        }

        [AvaloniaFact]
        public void SetMark_None_ClearsEveryClass()
        {
            var item = new TreeViewItem();
            TreeDropMark.SetMark(item, DropMark.GapTop);
            Assert.True(HasClass(item, ":droptop"));

            TreeDropMark.SetMark(item, DropMark.None);

            Assert.False(HasClass(item, ":droptarget"));
            Assert.False(HasClass(item, ":dropgap"));
            Assert.False(HasClass(item, ":droptop"));
        }

        [AvaloniaFact]
        public void Mark_BeforeARow_PutsTheGapBelowOnTheRowAbove()
        {
            var (view, tree) = Show();
            var node = tree.CompositionNodes()[1];
            var above = TreeRowOrder.RowAbove(node)!;
            Assert.NotSame(tree.CompositionNodes()[0], above);

            view.Marker.Mark(ItemOf(view, node), DropPosition.Before);

            Assert.Equal(DropMark.GapBelow, TreeDropMark.GetMark(ItemOf(view, above)));
            Assert.Equal(DropMark.None, TreeDropMark.GetMark(ItemOf(view, node)));
            Assert.Same(ItemOf(view, node), view.Marker.Marked);
            Assert.Equal(DropPosition.Before, view.Marker.MarkedPosition);
        }

        [AvaloniaFact]
        public void Mark_BeforeTheFirstRow_PutsTheGapTopOnTheRow()
        {
            var (view, tree) = Show();
            var item = ItemOf(view, tree.CompositionsNode());

            view.Marker.Mark(item, DropPosition.Before);

            Assert.Equal(DropMark.GapTop, TreeDropMark.GetMark(item));
        }

        [AvaloniaFact]
        public void Mark_None_ClearsTheMarksOfTheRowAndTheBar()
        {
            var (view, tree) = Show();
            var node = tree.CompositionNodes()[1];
            var above = TreeRowOrder.RowAbove(node)!;
            view.Marker.Mark(ItemOf(view, node), DropPosition.Before);

            view.Marker.Mark(null, null);

            Assert.Equal(DropMark.None, TreeDropMark.GetMark(ItemOf(view, above)));
            Assert.Equal(DropMark.None, TreeDropMark.GetMark(ItemOf(view, node)));
            Assert.Null(view.Marker.Marked);
            Assert.Null(view.Marker.MarkedPosition);
        }

        [AvaloniaFact]
        public void Mark_BeforeARow_ShowsTheDropLineOfTheRowAbove()
        {
            var (view, tree) = Show();
            var node = tree.CompositionNodes()[1];
            var above = TreeRowOrder.RowAbove(node)!;

            view.Marker.Mark(ItemOf(view, node), DropPosition.Before);
            window!.UpdateLayout();

            var line = DropLineOf(ItemOf(view, above));
            var lineColor = ((ISolidColorBrush)Application.Current!.FindResource("CursorFrameBrush")!).Color;
            Assert.True(line.IsVisible);
            Assert.Equal(lineColor, ((ISolidColorBrush)line.Background!).Color);
            Assert.False(DropLineOf(ItemOf(view, node)).IsVisible);
        }
    }
}
