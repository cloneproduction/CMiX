using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Panels;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;
using Entity = CMiX.Core.Compositing.Entity;
using Layer = CMiX.Core.Compositing.Layer;
using Project = CMiX.Core.Compositing.Project;
using CMiX.Studio.Avalonia.Tests.Support;

namespace CMiX.Studio.Avalonia.Tests
{
    // The view finds the item of a node through an index that the tree fills when it prepares an item.
    public class TreeRowsTests : IDisposable
    {
        private Window? window;

        public void Dispose() => window?.Close();

        // A composition with two layers. Each layer has two entities.
        private (ProjectTreeView View, ProjectTree Tree, Composition Composition) Show()
        {
            var provider = TestServiceProviderFactory.Create();
            var project = provider.GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)project.CompositionManager.ManagerData.Items[0];
            for (var i = 0; i < 2; i++)
            {
                composition.LayerManager.AddItem(typeof(Layer));
                var layer = (Layer)composition.LayerManager.ManagerData.Items[i];
                layer.ModelEntityManager.AddItem(typeof(Entity));
                layer.ModelEntityManager.AddItem(typeof(Entity));
            }

            var tree = TreeFixtures.CreateTree(provider);
            var view = new ProjectTreeView { DataContext = tree };
            window = new Window { Content = view, Width = 500, Height = 800 };
            window.Show();
            foreach (var panel in view.GetVisualDescendants().OfType<PanelCollapse>())
                panel.Duration = TimeSpan.Zero;
            Settle();
            return (view, tree, composition);
        }

        private void Settle()
        {
            Dispatcher.UIThread.RunJobs();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            window!.UpdateLayout();
            Dispatcher.UIThread.RunJobs();
        }

        private static TreeNode LastEntity(ProjectTree tree) =>
            tree.CompositionNodes()[0].Children[0].Children[1].Children.Single(child => child.Group is EntitiesGroup).Children[1];

        [AvaloniaFact]
        public void ContainerPrepared_ForNestedItems_FillsTheIndexForEveryRow()
        {
            var (view, _, _) = Show();

            var realized = view.GetVisualDescendants().OfType<TreeViewItem>().Count();

            Assert.True(realized > 4);
            Assert.Equal(realized, view.RowIndex.Count);
        }

        [AvaloniaFact]
        public void FindItem_AfterTheContainerIsPrepared_UsesTheIndex()
        {
            var (view, tree, _) = Show();
            var node = LastEntity(tree);

            var item = view.RowIndex.FindItem(node);

            Assert.NotNull(item);
            Assert.Same(node, item!.DataContext);
            Assert.Equal(0, view.RowIndex.FallbackWalks);
        }

        [AvaloniaFact]
        public void FindItem_AfterTheContainerIsCleared_FallsBackAndRefills()
        {
            var (view, tree, _) = Show();
            var node = LastEntity(tree);
            var item = view.RowIndex.FindItem(node)!;

            view.RowIndex.Forget(item);
            var found = view.RowIndex.FindItem(node);
            Assert.Same(item, found);
            Assert.Equal(1, view.RowIndex.FallbackWalks);

            Assert.Same(item, view.RowIndex.FindItem(node));
            Assert.Equal(1, view.RowIndex.FallbackWalks);
        }

        [AvaloniaFact]
        public void ContainerClearing_WhenALayerIsRemoved_RemovesItsRowsFromTheIndex()
        {
            var (view, _, composition) = Show();
            var before = view.RowIndex.Count;

            composition.LayerManager.RemoveItem(composition.LayerManager.ManagerData.Items[1]);
            Settle();

            Assert.Equal(view.GetVisualDescendants().OfType<TreeViewItem>().Count(), view.RowIndex.Count);
            Assert.True(view.RowIndex.Count < before);
        }
    }
}
