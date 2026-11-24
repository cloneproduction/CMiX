using CMiX.Studio.Avalonia.ViewModels.Tree;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;
using Entity = CMiX.Core.Compositing.Entity;
using Layer = CMiX.Core.Compositing.Layer;
using Project = CMiX.Core.Compositing.Project;
using CMiX.Studio.Avalonia.Tests.Support;

namespace CMiX.Studio.Avalonia.Tests
{
    // The work of the tree stays in a budget for a large composition. These tests count the work. They do not measure time.
    public class ProjectTreeBudgetTests
    {
        private const int Layers = 20;
        private const int EntitiesPerLayer = 100;

        private static ProjectTree CreateTree()
        {
            var provider = TestServiceProviderFactory.Create();
            var project = provider.GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)project.CompositionManager.ManagerData.Items[0];
            for (var i = 0; i < Layers; i++)
            {
                composition.LayerManager.AddItem(typeof(Layer));
                var layer = (Layer)composition.LayerManager.ManagerData.Items[i];
                for (var j = 0; j < EntitiesPerLayer; j++)
                    layer.ModelEntityManager.AddItem(typeof(Entity));
            }

            return TreeFixtures.CreateTree(provider);
        }

        private static IEnumerable<TreeNode> All(IEnumerable<TreeNode> nodes) =>
            nodes.SelectMany(node => new[] { node }.Concat(All(node.Children)));

        private static TreeNode EntitiesOf(TreeNode layer) => layer.Children.Single(child => child.Group is EntitiesGroup);

        // A drag of an entity. BeginDrag changes IsDragActive of each row and one look of the row: IsDimmed, IsDropCandidate or
        // IsDropNeighbor. EndDrag changes the same two back. That is 4 per row. A candidate that has a target below it also changes
        // HasDropTargetBelow, two more for each candidate. The bound is 4 per row plus 2 per row that is not a leaf.
        [Fact]
        public void BeginDragAndEndDrag_OnALargeTree_StayInThePropertyChangedBudget()
        {
            var tree = CreateTree();
            var nodes = All(tree.Roots).ToList();
            var parents = nodes.Count(node => node.Children.Count > 0);
            Assert.True(nodes.Count >= Layers * EntitiesPerLayer);
            var changes = 0;
            foreach (var node in nodes)
                node.PropertyChanged += (_, _) => changes++;

            tree.BeginDrag(EntitiesOf(tree.CompositionNodes()[0].Children[0].Children[0]).Children[0]);
            tree.EndDrag();

            var bound = 4 * nodes.Count + 2 * parents;
            Assert.True(changes < bound, $"{changes} changes for {nodes.Count} rows, the bound is {bound}.");
            Assert.All(nodes, node => Assert.False(node.IsDragActive));
        }
    }
}
