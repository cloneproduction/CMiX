using System.Diagnostics;
using Avalonia.Headless.XUnit;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using Xunit.Abstractions;
using Composition = CMiX.Core.Compositing.Composition;
using Entity = CMiX.Core.Compositing.Entity;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // The work of the inspector for one selection stays in a budget for a large composition.
    // The bounds of time are about three times the measured values, so a slow machine passes.
    // Measured on the headless renderer (2026-11-06), in ms: entity 370 to 440 (the first build), layer 120 to 180,
    // composition 45 to 95, entity again 230 to 370. Each change of the kind of the item builds one view.
    public class PanelInspectorBudgetTests
    {
        private const int Layers = 20;
        private const int EntitiesPerLayer = 100;

        private readonly ITestOutputHelper output;

        public PanelInspectorBudgetTests(ITestOutputHelper output) => this.output = output;

        [AvaloniaFact]
        public void SelectedNode_OnALargeComposition_BuildsOneViewAndStaysInTheTimeBudget()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.ManagerData.Items[0];
            for (var i = 0; i < Layers; i++)
            {
                composition.LayerManager.AddItem(typeof(Layer));
                var layer = (Layer)composition.LayerManager.ManagerData.Items[i];
                for (var j = 0; j < EntitiesPerLayer; j++)
                    layer.ModelEntityManager.AddItem(typeof(Entity));
            }

            var tree = viewModel.ProjectTree;
            tree.SelectedNode = null;
            TestServiceProviderFactory.SelectMainTab(window, "Inspector");
            var inspector = window.GetVisualDescendants().OfType<PanelInspector>().Single();
            TestServiceProviderFactory.Pump();

            var compositionNode = tree.CompositionNodes()[0];
            TreeNode EntityOf(int layer, int index) =>
                compositionNode.Children[0].Children[layer].Children.Single(child => child.Group is EntitiesGroup).Children[index];
            var steps = new (string Name, TreeNode Node, int Views, long Bound)[]
            {
                ("entity", EntityOf(0, 0), 1, 1500),
                ("layer", compositionNode.Children[0].Children[Layers / 2], 1, 600),
                ("composition", compositionNode, 1, 400),
                ("entity again", EntityOf(Layers - 1, EntitiesPerLayer - 1), 1, 1200),
            };

            var results = new List<(string Name, long Milliseconds, int Views)>();
            foreach (var step in steps)
            {
                var before = inspector.EditorViewsBuilt;
                var watch = Stopwatch.StartNew();
                tree.SelectedNode = step.Node;
                TestServiceProviderFactory.Pump();
                window.UpdateLayout();
                TestServiceProviderFactory.Pump();
                watch.Stop();
                results.Add((step.Name, watch.ElapsedMilliseconds, inspector.EditorViewsBuilt - before));
            }

            var report = string.Join(", ", results.Select(r => $"{r.Name}: {r.Milliseconds} ms, {r.Views} views"));
            output.WriteLine(report);

            for (var i = 0; i < steps.Length; i++)
            {
                Assert.True(results[i].Views <= steps[i].Views, $"The {steps[i].Name} builds more than {steps[i].Views} views. {report}");
                Assert.True(results[i].Milliseconds < steps[i].Bound, $"The {steps[i].Name} takes more than {steps[i].Bound} ms. {report}");
            }
        }
    }
}
