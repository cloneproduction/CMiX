using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using static CMiX.Studio.Avalonia.Tests.Support.TreeTestScreen;
using Entity = CMiX.Core.Compositing.Entity;

namespace CMiX.Studio.Avalonia.Tests
{
    // The look of the search: the rows that do not match are covered, as a drag covers the rows that can not take the node.
    // During a drag only the drag covers rows, and the search look comes back when the drag ends.
    public class ProjectTreeSearchLookTests : IDisposable
    {
        private Window? window;

        public void Dispose() => window?.Close();

        // A composition with two layers. The first has one entity, renamed Echo.
        private (TreeViewProject View, ProjectTree Tree, TreeNode Echo) Show()
        {
            var scene = CreateScene([2], s => s.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity)));
            var screen = TreeTestScreen.Show(scene.Tree, 500, 700);
            window = screen.Window;
            var echo = All(scene.Tree.Roots).Single(node => node.Item is Entity);
            ((Entity)echo.Item!).PrefabService.Name.Value = "Echo";
            screen.Settle();
            return (screen.View, scene.Tree, echo);
        }

        private void Settle() => TreeTestScreen.Settle(window!);

        private static bool IsCovered(TreeViewProject view, TreeNode node) => Part(view, node, "DimLayer").Classes.Contains("dimmed");

        [AvaloniaFact]
        public void Search_AName_CoversTheRowsThatDoNotMatchAndTheParents()
        {
            var (view, tree, echo) = Show();

            tree.SearchText = "Echo";
            Settle();

            Assert.False(IsCovered(view, echo));
            Assert.True(IsCovered(view, echo.Parent!));
            Assert.True(IsCovered(view, tree.CompositionNodes()[0]));
            foreach (var node in All(tree.Roots))
                Assert.Equal(!node.IsSearchMatch, IsCovered(view, node));
        }

        [AvaloniaFact]
        public void Search_AnEmptyText_CoversNoRow()
        {
            var (view, tree, _) = Show();
            tree.SearchText = "Echo";
            Settle();

            tree.SearchText = "";
            Settle();

            Assert.All(All(tree.Roots), node => Assert.False(IsCovered(view, node)));
        }

        [AvaloniaFact]
        public void Drag_DuringASearch_ShowsOnlyTheCoverOfTheDragAndTheSearchComesBackAfter()
        {
            var (view, tree, echo) = Show();
            tree.SearchText = "Echo";
            Settle();
            var layer = tree.CompositionNodes()[0].Children[0].Children[1];

            tree.BeginDrag(layer);
            Settle();

            foreach (var node in All(tree.Roots))
                Assert.Equal(node.IsDimmed, IsCovered(view, node));
            // A row that the drag does not cover, and that the search would cover, is clear.
            Assert.Contains(All(tree.Roots), node => !node.IsDimmed && node.IsSearchDimmed);

            tree.EndDrag();
            Settle();

            Assert.False(IsCovered(view, echo));
            foreach (var node in All(tree.Roots))
                Assert.Equal(!node.IsSearchMatch, IsCovered(view, node));
        }
    }
}
