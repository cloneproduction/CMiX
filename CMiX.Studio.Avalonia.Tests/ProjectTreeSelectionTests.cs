using CMiX.Core.Compositing;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using CMiX.Studio.Avalonia.Tests.Support;

namespace CMiX.Studio.Avalonia.Tests
{
    // The tree keeps the origin of each selection: the view brings a row into view only for some origins.
    public class ProjectTreeSelectionTests
    {
        // A holds two layers. B holds one layer.
        private static ProjectTree CreateTree()
        {
            var provider = TestServiceProviderFactory.Create();
            var project = provider.GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));
            project.CompositionManager.AddItem(typeof(Composition));
            var a = (Composition)project.CompositionManager.ManagerData.Items[0];
            var b = (Composition)project.CompositionManager.ManagerData.Items[1];
            a.LayerManager.AddItem(typeof(Layer));
            a.LayerManager.AddItem(typeof(Layer));
            b.LayerManager.AddItem(typeof(Layer));
            return TreeFixtures.CreateTree(provider);
        }

        [Fact]
        public void Select_FromAPointer_SetsThePointerOrigin()
        {
            var tree = CreateTree();
            var layer = tree.CompositionNodes()[0].Children[0].Children[1];
            tree.Select(tree.CompositionNodes()[0], SelectionOrigin.Program);

            tree.SelectedNode = layer;

            Assert.Same(layer, tree.SelectedNode);
            Assert.Equal(SelectionOrigin.Pointer, tree.Origin);
        }

        [Fact]
        public void Select_FromAProgram_SetsTheProgramOrigin()
        {
            var tree = CreateTree();
            var layer = tree.CompositionNodes()[0].Children[0].Children[1];
            tree.SelectedNode = tree.CompositionNodes()[0];
            SelectionOrigin? originAtTheChange = null;
            tree.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(ProjectTree.SelectedNode))
                    originAtTheChange = tree.Origin;
            };

            tree.Select(layer, SelectionOrigin.Program);

            Assert.Same(layer, tree.SelectedNode);
            Assert.Equal(SelectionOrigin.Program, tree.Origin);
            Assert.Equal(SelectionOrigin.Program, originAtTheChange);
        }

        [Fact]
        public async Task DropAsync_AfterAMove_SelectsWithTheProgramOrigin()
        {
            var tree = CreateTree();
            var layer = tree.CompositionNodes()[0].Children[0].Children[0];
            tree.SelectedNode = layer;
            Assert.Equal(SelectionOrigin.Pointer, tree.Origin);

            await tree.DropAsync(layer, tree.CompositionNodes()[1], DropPosition.Inside);

            Assert.Same(layer.Item, tree.SelectedNode?.Item);
            Assert.Equal(SelectionOrigin.Program, tree.Origin);
        }

        [Fact]
        public void RevealsSelection_AProgramSelection_IsTrue()
        {
            var tree = CreateTree();
            var layer = tree.CompositionNodes()[0].Children[0].Children[1];

            tree.Select(layer, SelectionOrigin.Program);

            Assert.True(tree.RevealsSelection);
        }

        [Fact]
        public void RevealsSelection_APointerSelection_IsFalse()
        {
            var tree = CreateTree();
            var layer = tree.CompositionNodes()[0].Children[0].Children[1];

            tree.SelectedNode = layer;

            Assert.False(tree.RevealsSelection);
        }

        [Fact]
        public void RevealsSelection_APeerSelection_IsTrue()
        {
            var tree = CreateTree();
            var layer = tree.CompositionNodes()[0].Children[0].Children[1];

            tree.Select(layer, SelectionOrigin.Peer);

            Assert.True(tree.RevealsSelection);
        }
    }
}
