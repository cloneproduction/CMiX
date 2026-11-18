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
using Entity = CMiX.Core.Compositing.Entity;
using Layer = CMiX.Core.Compositing.Layer;
using Project = CMiX.Core.Compositing.Project;

namespace CMiX.Studio.Avalonia.Tests
{
    // A drag over a list of modifiers: the bar shows the place, the group "Modifiers" takes the row, and nothing is cut.
    public class ProjectTreeDropBarTests : IDisposable
    {
        private Window? window;

        public void Dispose() => window?.Close();

        // A composition with a layer, an entity, and three modifiers on it.
        private (ProjectTreeView View, ProjectTree Tree, Entity Entity, TreeNode Group) Show()
        {
            var scene = CreateScene([1], s =>
            {
                s.Layer.ModelEntityManager.AddItem(typeof(Entity));
                var added = (Entity)s.Layer.ModelEntityManager.ManagerData.Items[0];
                for (var i = 0; i < 3; i++)
                    added.ModifierManager.AddItem(typeof(CMiX.Core.Modulation.Modifiers.TranslateModifier));
            });
            var tree = scene.Tree;
            var entity = (Entity)scene.Layer.ModelEntityManager.ManagerData.Items[0];
            var screen = TreeTestScreen.Show(tree, 500, 900);
            window = screen.Window;
            var view = screen.View;
            var group = tree.CompositionNodes()[0].Children[0].Children[0].Children[0].Children[0].Children.Single(child => child.Group is ModifiersGroup);
            return (view, tree, entity, group);
        }

        private void Settle() => TreeTestScreen.Settle(window!);

        private Point PointIn(ProjectTreeView view, TreeNode node, double fraction)
        {
            var row = ItemOf(view, node).GetVisualDescendants().OfType<Border>().First(border => border.Name == "RowBorder");
            return row.TranslatePoint(new Point(row.Bounds.Width / 2, row.Bounds.Height * fraction), window!)!.Value;
        }

        // Drags the source row over the target and keeps the button down.
        private void DragOver(ProjectTreeView view, TreeNode source, TreeNode target, double fraction)
        {
            TreeDrag.Over(window!, PointIn(view, source, 0.5), PointIn(view, target, fraction));
            Settle();
        }

        // Ends the drag without a drop.
        private void Cancel(ProjectTreeView view, TreeNode under)
        {
            TreeDrag.Cancel(window!, PointIn(view, under, 0.5));
            Settle();
        }

        // The part of the bar that is drawn: the bar in the window, cut by everything around it.
        private static double SeenArea(Border line)
        {
            var seen = line.GetTransformedBounds();
            if (seen == null)
                return 0;

            var bounds = new Rect(seen.Value.Bounds.Size).TransformToAABB(seen.Value.Transform);
            var visible = bounds.Intersect(seen.Value.Clip);
            return visible.Width * visible.Height;
        }

        private static Border LineOf(TreeViewItem item) =>
            item.GetVisualDescendants().OfType<Border>().Last(border => border.Name == "DropLine");

        [AvaloniaFact]
        public void DragOver_BeforeARowOfAList_ShowsTheBarForEveryRow()
        {
            var (view, _, _, group) = Show();
            var source = group.Children[0];

            // Before the third row. Before the second row is where the row is: no place, no bar.
            DragOver(view, source, group.Children[2], 0.2);
            var item = ItemOf(view, group.Children[2]);
            Assert.Same(item, view.Marker.Marked);
            Assert.Equal(DropPosition.Before, view.Marker.MarkedPosition);
            Assert.Equal(DropMark.GapBelow, TreeDropMark.GetMark(ItemOf(view, TreeRowOrder.LastVisible(group.Children[1]))));
            Assert.True(SeenArea(LineOf(ItemOf(view, TreeRowOrder.LastVisible(group.Children[1])))) > 0, "The bar before the row is cut.");

            Cancel(view, group.Children[2]);
            DragOver(view, group.Children[2], group.Children[0], 0.2);
            Assert.Same(ItemOf(view, group.Children[0]), view.Marker.Marked);
            Assert.Equal(DropPosition.Before, view.Marker.MarkedPosition);
            Assert.Equal(DropMark.GapBelow, TreeDropMark.GetMark(ItemOf(view, group)));
            Assert.True(SeenArea(LineOf(ItemOf(view, group))) > 0, "The bar before the first row is cut.");
        }

        // The last row of the list has rows under it (its group of modulators): the bar after it is at the bottom of the item.
        [AvaloniaFact]
        public void DragOver_AfterTheLastRowOfAList_ShowsTheBar()
        {
            var (view, _, _, group) = Show();
            var source = group.Children[0];
            var last = group.Children[2];

            DragOver(view, source, last.Children[0], 0.9);

            var item = ItemOf(view, last);
            Assert.Same(item, view.Marker.Marked);
            Assert.Equal(DropPosition.After, view.Marker.MarkedPosition);
            Assert.Equal(DropMark.GapBelow, TreeDropMark.GetMark(ItemOf(view, TreeRowOrder.LastVisible(last))));
            Assert.True(SeenArea(LineOf(ItemOf(view, TreeRowOrder.LastVisible(last)))) > 0, "The bar after the last row is cut.");
        }

        [AvaloniaFact]
        public void Drop_AModifierOnItsGroup_PutsItAtTheEnd()
        {
            var (view, _, entity, group) = Show();
            var source = group.Children[0];
            var moved = source.Item;

            DragOver(view, source, group, 0.5);
            Assert.True(group.IsDropCandidate);
            Assert.Contains(":droptarget", ItemOf(view, group).Classes);

            TreeDrag.Release(window!, PointIn(view, group, 0.5));

            Assert.Same(moved, entity.ModifierManager.ManagerData.Items[^1]);
            Assert.Equal(3, entity.ModifierManager.ManagerData.Items.Count);
        }

        [AvaloniaFact]
        public void BeginDrag_ARowOfTheModifiers_LightsTheGroupAndNotTheRows()
        {
            var (view, tree, _, group) = Show();

            tree.BeginDrag(group.Children[0]);
            Settle();

            Assert.True(group.IsDropCandidate);
            Assert.All(group.Children, row => Assert.False(row.IsDropCandidate));
        }
    }
}
