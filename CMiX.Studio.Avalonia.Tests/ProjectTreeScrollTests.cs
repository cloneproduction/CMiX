using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Platform.Storage;
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
    // The tree does not scroll when the user clicks a row, so a double click works at the edge of the view. A row that a program
    // selects is brought into view.
    public class ProjectTreeScrollTests : IDisposable
    {
        private Window? window;

        public void Dispose() => window?.Close();

        // A composition with twelve layers: a tree that is much higher than the window.
        private (TreeViewProject View, ProjectTree Tree, ScrollViewer Scroller) Show()
        {
            var tree = CreateScene([12]).Tree;
            var screen = TreeTestScreen.Show(tree, 500, 500);
            window = screen.Window;
            var view = screen.View;
            tree.SelectedNode = null;
            Settle();
            return (view, tree, view.GetVisualDescendants().OfType<ScrollViewer>().First());
        }

        private void Settle() => TreeTestScreen.Settle(window!);

        private double RowTop(TreeViewProject view, TreeNode node) =>
            ItemOf(view, node).GetVisualDescendants().OfType<Border>().First(border => border.Name == "RowBorder").TranslatePoint(new Point(0, 0), window!)!.Value.Y;

        private Point RowPoint(TreeViewProject view, TreeNode node)
        {
            var row = ItemOf(view, node).GetVisualDescendants().OfType<Border>().First(border => border.Name == "RowBorder");
            // Left of the toggles, which end the row.
            return row.TranslatePoint(new Point(row.Bounds.Width - 80, row.Bounds.Height / 2), window!)!.Value;
        }

        // Scrolls until a layer row is near the bottom edge of the view, where its whole block is not in view. Returns that layer.
        private TreeNode ScrollToARowAtTheBottomEdge(TreeViewProject view, ProjectTree tree, ScrollViewer scroller)
        {
            for (var offset = 0; offset < 900; offset += 10)
            {
                scroller.Offset = new Vector(0, offset);
                Settle();
                var found = tree.CompositionNodes()[0].Children[0].Children.FirstOrDefault(layer => RowTop(view, layer) is > 400 and < 440);
                if (found != null)
                    return found;
            }

            throw new InvalidOperationException("No row near the bottom edge.");
        }

        [AvaloniaFact]
        public void Click_ARowAtTheEdgeOfTheView_DoesNotScroll()
        {
            var (view, tree, scroller) = Show();
            var layer = ScrollToARowAtTheBottomEdge(view, tree, scroller);
            var before = scroller.Offset;

            var point = RowPoint(view, layer);
            window!.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            Settle();

            Assert.Same(layer, tree.SelectedNode);
            Assert.Equal(before, scroller.Offset);
        }

        [AvaloniaFact]
        public void DoubleClick_ARowAtTheEdgeOfTheView_CollapsesItInPlace()
        {
            var (view, tree, scroller) = Show();
            var layer = ScrollToARowAtTheBottomEdge(view, tree, scroller);
            var top = RowTop(view, layer);
            var offset = scroller.Offset;

            var point = RowPoint(view, layer);
            window!.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            Settle();

            Assert.False(layer.IsExpanded);
            Assert.Equal(offset, scroller.Offset);
            Assert.Equal(top, RowTop(view, layer));
        }

        [AvaloniaFact]
        public void Select_FromAProgram_ScrollsTheRowIntoView()
        {
            var (view, tree, scroller) = Show();
            var far = tree.CompositionNodes()[0].Children[0].Children[10];
            Assert.True(RowTop(view, far) > 500, "The row is not out of the view.");

            tree.Select(far, SelectionOrigin.Program);
            Settle();
            Dispatcher.UIThread.RunJobs();
            Settle();

            Assert.True(scroller.Offset.Y > 0);
            Assert.InRange(RowTop(view, far), 0, 500);
        }

        [AvaloniaFact]
        public void Select_FromAPointer_DoesNotScroll()
        {
            var (view, tree, scroller) = Show();
            var far = tree.CompositionNodes()[0].Children[0].Children[10];
            Assert.True(RowTop(view, far) > 500, "The row is not out of the view.");

            tree.Select(far, SelectionOrigin.Pointer);
            Settle();
            Dispatcher.UIThread.RunJobs();
            Settle();

            Assert.Equal(0, scroller.Offset.Y);
        }

        // A point of the scroll viewer, in the window.
        private Point PointAt(ScrollViewer scroller, double y) => scroller.TranslatePoint(new Point(40, y), window!)!.Value;

        // Starts the drag of the first layer with the pointer and moves it to the height y of the scroll viewer.
        private void DragTo(TreeViewProject view, ProjectTree tree, ScrollViewer scroller, double y)
        {
            TreeDrag.Over(window!, RowPoint(view, tree.CompositionNodes()[0].Children[0].Children[0]), PointAt(scroller, y));
            Settle();
        }

        // Moves the pointer of the drag to the height y of the scroll viewer.
        private void MoveTo(ScrollViewer scroller, double y)
        {
            TreeDrag.MoveTo(window!, PointAt(scroller, y));
            Settle();
        }

        [AvaloniaFact]
        public void DragOver_NearAnEdge_ScrollsTowardThatEdge()
        {
            var (view, tree, scroller) = Show();
            DragTo(view, tree, scroller, scroller.Bounds.Height / 2);
            // The timer of the drag also scrolls while the test runs the jobs. Read the offset after the move.
            MoveTo(scroller, scroller.Bounds.Height - 4);
            var before = scroller.Offset.Y;
            Assert.True(view.AutoScrollStep() > 0);
            Assert.True(scroller.Offset.Y > before);

            // Some more steps down. A tick up during the next move must not bring the view to the top.
            while (scroller.Offset.Y < 4 * DragAutoScroller.MaxScrollStep)
                Assert.True(view.AutoScrollStep() > 0);

            MoveTo(scroller, 4);
            before = scroller.Offset.Y;
            Assert.True(before > 0);
            Assert.True(view.AutoScrollStep() < 0);
            Assert.True(scroller.Offset.Y < before);

            TreeDrag.Cancel(window!, PointAt(scroller, 4));
            Assert.False(tree.IsDragging);
        }

        [AvaloniaFact]
        public void Wheel_DuringADragOnly_ScrollsTheTree()
        {
            var (view, tree, scroller) = Show();
            view.ScrollByWheel(-2);
            Assert.Equal(0, scroller.Offset.Y);

            DragTo(view, tree, scroller, scroller.Bounds.Height / 2);
            view.ScrollByWheel(-2);
            var down = scroller.Offset.Y;
            Assert.True(down > 0);

            view.ScrollByWheel(1);
            Assert.True(scroller.Offset.Y < down);

            TreeDrag.Cancel(window!, PointAt(scroller, scroller.Bounds.Height / 2));
        }

        // The pointer is captured, so the drag goes on beyond the edge of the tree. Escape stops it.
        [AvaloniaFact]
        public void DragOver_AwayFromTheEdgesOrCancelled_StopsTheScroll()
        {
            var (view, tree, scroller) = Show();
            DragTo(view, tree, scroller, scroller.Bounds.Height / 2);
            Assert.Equal(0, view.AutoScrollStep());

            MoveTo(scroller, scroller.Bounds.Height - 4);
            Assert.True(view.AutoScrollStep() > 0);

            TreeDrag.Cancel(window!, PointAt(scroller, scroller.Bounds.Height - 4));
            Assert.Equal(0, view.AutoScrollStep());
            Assert.False(view.IsAutoScrolling);
        }

        // A file drag comes from the system (see TreeViewProject.OnDragOver). The edge zone serves it.
        private void FilesAt(ScrollViewer scroller, double y, RawDragEventType type)
        {
            var data = new DataObject();
            data.Set(DataFormats.Files, Array.Empty<IStorageItem>());
            window!.DragDrop(PointAt(scroller, y), type, data, DragDropEffects.Copy);
            Settle();
        }

        [AvaloniaFact]
        public void DragLeave_AfterAFileDragOverNearTheEdge_StopsTheScrollAtOnce()
        {
            var (view, _, scroller) = Show();
            FilesAt(scroller, scroller.Bounds.Height / 2, RawDragEventType.DragEnter);
            FilesAt(scroller, scroller.Bounds.Height - 4, RawDragEventType.DragOver);
            Assert.True(view.IsAutoScrolling);

            // A file drag that is cancelled ends with a leave inside the tree, and with no drop. The headless DragDrop runs the
            // dispatcher jobs, so the leave is raised here on a row.
            var data = new DataObject();
            data.Set(DataFormats.Files, Array.Empty<IStorageItem>());
            var row = view.GetVisualDescendants().OfType<TreeViewItem>().First();
            row.RaiseEvent(new DragEventArgs(DragDrop.DragLeaveEvent, data, row, new Point(40, 4), KeyModifiers.None));

            Assert.False(view.IsAutoScrolling);
        }
    }
}
