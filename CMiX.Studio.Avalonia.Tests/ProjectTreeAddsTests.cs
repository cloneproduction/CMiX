using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;
using Entity = CMiX.Core.Compositing.Entity;
using Layer = CMiX.Core.Compositing.Layer;
using Project = CMiX.Core.Compositing.Project;
using CMiX.Studio.Avalonia.Tests.Support;

namespace CMiX.Studio.Avalonia.Tests
{
    // ProjectTree.GetAddGroups feeds the Add panel of the menu of a row, of the menu of the space and of the toolbar. GetAddChoices feeds the toolbar enablement.
    public class ProjectTreeAddsTests : IDisposable
    {
        private readonly List<Window> windows = new();

        public void Dispose()
        {
            foreach (var window in windows)
            {
                foreach (var toolbar in window.GetVisualDescendants().OfType<ToolbarProjectTree>())
                    toolbar.Panel.Close();

                if (window.GetVisualDescendants().OfType<ListTreeProject>().FirstOrDefault() is { } view)
                    view.MenuAddPanel.Close();

                foreach (var control in window.GetVisualDescendants().OfType<Control>())
                    control.ContextMenu?.Close();

                window.Close();
            }
        }

        private sealed class Screen
        {
            public Window Window { get; init; } = null!;
            public ListTreeProject View { get; init; } = null!;
            public ProjectTree Tree { get; init; } = null!;
            public Project Project { get; init; } = null!;
            public Composition A { get; init; } = null!;

            public Button AddButton => View.GetVisualDescendants().OfType<Button>().First(button => button.Name == "addButton");
            public AddPanel Panel => View.GetVisualDescendants().OfType<ToolbarProjectTree>().First().Panel;
            public TreeNode LayerNode => Tree.CompositionNodes()[0].Children[0].Children[0];

            public RowListTree RowOf(TreeNode node) => View.Rows.RowOf(node)!;

            public void Click(Control control) => Window.Click(control);
        }

        // One composition with one layer, and one entity in the layer.
        private Screen Show()
        {
            var provider = TestServiceProviderFactory.Create();
            var project = provider.GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));
            var a = (Composition)project.CompositionManager.ManagerData.Items[0];
            a.LayerManager.AddItem(typeof(Layer));
            ((Layer)a.LayerManager.ManagerData.Items[0]).ModelEntityManager.AddItem(typeof(Entity));

            var tree = TreeFixtures.CreateTree(provider);
            var (window, view) = ListTreeTestScreen.Show(tree, 400, 900);
            windows.Add(window);
            return new Screen { Window = window, View = view, Tree = tree, Project = project, A = a };
        }

        private static List<string> Flat(IEnumerable<MenuItem> rows) =>
            rows.SelectMany(row => Equals(row.Header, "Add") ? row.Items.OfType<MenuItem>() : new[] { row })
                .Select(row => row.Header?.ToString() ?? string.Empty)
                .ToList();

        [AvaloniaFact]
        public void GetAddGroups_ALayerRow_ListsTheSameGroupsInTheRowMenuAndTheToolbar()
        {
            var screen = Show();
            var node = screen.LayerNode;
            var model = screen.Tree.GetAddChoices(node).Select(add => add.Header).ToList();

            // The Add row of the menu of the row opens a panel with the groups of the row.
            var row = screen.RowOf(node);
            row.RaiseEvent(new ContextRequestedEventArgs { Source = row });
            Dispatcher.UIThread.RunJobs();
            var menu = screen.View.Menu!;
            menu.Items.OfType<MenuItem>().Single(item => Equals(item.Header, "Add")).RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
            Dispatcher.UIThread.RunJobs();
            var rowGroups = screen.View.MenuAddPanel.List.Groups!.Select(group => group.Header).ToList();
            menu.Close();

            // The panel of the toolbar: the same groups, then the project.
            screen.Tree.SelectedNode = node;
            screen.Click(screen.AddButton);
            var groups = screen.Panel.List.Groups!.ToList();

            Assert.NotEmpty(model);
            Assert.Equal(new[] { "Content", "Filter", "Modifier", "Project" }, rowGroups);
            Assert.Equal(rowGroups, groups.Select(group => group.Header));
            Assert.Equal(model.Take(5), groups.Single(group => group.Header == "Content").Items.Where(item => item.IsEnabled).Select(item => item.Text));
        }

        [AvaloniaFact]
        public void GetAddChoices_NoNode_OffersAddComposition()
        {
            var screen = Show();

            Assert.Equal(new[] { "Add Composition" }, screen.Tree.GetAddChoices(null).Select(add => add.Header));

            screen.View.Scroller.RaiseEvent(new ContextRequestedEventArgs { Source = screen.View.Scroller });
            Dispatcher.UIThread.RunJobs();

            // The menu of the space has the Add row, which opens the panel of the project. Nothing is added by the menu itself.
            Assert.Equal(new[] { "Add" }, screen.View.Menu!.Items.OfType<MenuItem>().Skip(2).Take(1).Select(item => item.Header?.ToString()));
            var entries = screen.Tree.GetAddGroups(null).SelectMany(group => group.Items).Where(item => item.Enabled).Select(item => item.Header);
            Assert.Equal(new[] { "Composition" }, entries);
        }

        [AvaloniaFact]
        public void GetAddChoices_AnEntityRow_OffersAddCompositionOnlyLast()
        {
            var screen = Show();
            var entity = screen.Tree.CompositionNodes()[0].Children[0].Children[0].Children.Single(group => group.Group is EntitiesGroup).Children[0];

            var adds = screen.Tree.GetAddChoices(entity);

            Assert.NotEmpty(adds.SkipLast(1));
            Assert.DoesNotContain(adds.SkipLast(1), add => add.Header == "Add Composition");
            Assert.Equal("Add Composition", adds[^1].Header);
        }

        [AvaloniaFact]
        public void AddButton_NothingSelected_OpensThePanelAndAddsNothingAtOnce()
        {
            var screen = Show();
            screen.Tree.SelectedNode = null;

            screen.Click(screen.AddButton);

            Assert.Equal(1, screen.Project.CompositionManager.ManagerData.Items.Count);
            Assert.True(screen.Panel.IsOpen);
        }

        [Fact]
        public void TargetAdds_SelectionChanges_FollowsTheAddsOfTheSelectedNode()
        {
            var provider = TestServiceProviderFactory.Create();
            var project = provider.GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));
            var tree = TreeFixtures.CreateTree(provider);

            Assert.Equal(new[] { "Add Composition" }, tree.TargetAdds.Select(add => add.Header));

            tree.SelectedNode = tree.CompositionNodes()[0];

            Assert.Equal(tree.GetAddChoices(tree.CompositionNodes()[0]).Select(add => add.Header), tree.TargetAdds.Select(add => add.Header));
            Assert.True(tree.AddItemCommand.CanExecute(null));
        }
    }
}
