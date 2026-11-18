using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;
using Entity = CMiX.Core.Compositing.Entity;
using Layer = CMiX.Core.Compositing.Layer;
using Project = CMiX.Core.Compositing.Project;

namespace CMiX.Studio.Avalonia.Tests
{
    // The drag of a row without a starter: the tests call the session with points of the tree view. No button is down and no
    // pointer event is sent, so the session does not need them.
    public class TreeDragSessionTests : IDisposable
    {
        private TreeTestScreen? shown;

        public void Dispose() => shown?.Dispose();

        private sealed record Screen(ProjectTreeView View, TreeView TreeView, TreeNode Entity, TreeNode LayerA0, TreeNode LayerA1)
        {
            public TreeDragSession Session => View.DragSession;

            // The middle of the row of the node, in the tree view.
            public Point PointIn(TreeNode node)
            {
                var header = TreeTestScreen.RowOf(View, node);
                return header.TranslatePoint(new Point(header.Bounds.Width / 2, header.Bounds.Height / 2), TreeView)!.Value;
            }

            public int EntityCount(TreeNode layer) => ((Layer)layer.Item!).ModelEntityManager.ManagerData.Items.Count;
        }

        // One composition holds two layers. The first holds an entity, the second is collapsed.
        private Screen Show()
        {
            var provider = TestServiceProviderFactory.Create();
            var project = provider.GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)project.CompositionManager.ManagerData.Items[0];
            composition.LayerManager.AddItem(typeof(Layer));
            composition.LayerManager.AddItem(typeof(Layer));
            ((Layer)composition.LayerManager.ManagerData.Items[0]).ModelEntityManager.AddItem(typeof(Entity));

            var tree = TreeFixtures.CreateTree(provider);
            shown = TreeTestScreen.Show(tree, 400, 900);

            var layerA0 = tree.CompositionNodes()[0].Children[0].Children[0];
            var layerA1 = tree.CompositionNodes()[0].Children[0].Children[1];
            var entity = layerA0.Children.Single(group => group.Group is EntitiesGroup).Children[0];
            layerA1.IsExpanded = false;
            shown.Render();
            return new Screen(shown.View, shown.View.GetVisualDescendants().OfType<TreeView>().First(), entity, layerA0, layerA1);
        }

        [AvaloniaFact]
        public void Track_OverARowThatTakesTheNode_SetsTheSlot()
        {
            var screen = Show();

            screen.Session.Start(screen.Entity, screen.PointIn(screen.Entity));
            Assert.True(screen.Session.IsRunning);
            Assert.Same(screen.Entity, screen.Session.Source);
            Assert.Null(screen.Session.Slot);

            screen.Session.Track(screen.PointIn(screen.LayerA1));

            Assert.Equal(new DropPlace(screen.LayerA1, DropPosition.Inside), screen.Session.Slot);
            Assert.Same(screen.LayerA1, screen.View.Marker.Marked?.DataContext);

            // The session is the drag: the row drag with the button does not run.
            Assert.False(screen.View.RowDrag.IsDragging);
            screen.Session.End(TreeDragEnd.Cancel);
        }

        [AvaloniaFact]
        public void End_Drop_DropsTheNodeAtTheSlot()
        {
            var screen = Show();
            screen.Session.Start(screen.Entity, screen.PointIn(screen.Entity));
            screen.Session.Track(screen.PointIn(screen.LayerA1));

            screen.Session.End(TreeDragEnd.Drop);

            Assert.Equal(0, screen.EntityCount(screen.LayerA0));
            Assert.Equal(1, screen.EntityCount(screen.LayerA1));
            Assert.False(screen.Session.IsRunning);
            Assert.Null(screen.Session.Slot);
        }

        [AvaloniaFact]
        public void End_Cancel_ClosesTheOpenedRowsAndMovesNothing()
        {
            var screen = Show();
            screen.Session.Start(screen.Entity, screen.PointIn(screen.Entity));
            screen.Session.Track(screen.PointIn(screen.LayerA1));
            screen.Session.ToggleHovered();
            Assert.True(screen.LayerA1.IsExpanded);
            Assert.Equal(new[] { screen.LayerA1 }, screen.Session.OpenedRows);

            screen.Session.End(TreeDragEnd.Cancel);

            Assert.False(screen.LayerA1.IsExpanded);
            Assert.Empty(screen.Session.OpenedRows);
            Assert.Equal(1, screen.EntityCount(screen.LayerA0));
            Assert.Equal(0, screen.EntityCount(screen.LayerA1));
            Assert.False(screen.Session.IsRunning);
            Assert.Null(screen.Session.Slot);
        }
    }
}
