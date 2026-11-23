// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics;
using CMiX.Core.Compositing;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Texturing.Filters;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Xunit;
using Xunit.Abstractions;
using static CMiX.Studio.Avalonia.Tests.Support.ListTreeScenes;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    public class ListTreeBigProjectTimingTests
    {
        // Loose, so that a slow test machine does not fail. The report gives the real numbers.
        private const double BudgetMs = 200;

        private readonly ITestOutputHelper output;
        private readonly TreeDropService service = new();

        public ListTreeBigProjectTimingTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        // 20 compositions of 10 layers. Each layer has 10 entities and a filter, and its first entity has a modifier.
        private static Scene CreateBig() => TreeFixtures.CreateScene(Enumerable.Repeat(10, 20).ToArray(), Fill);

        private static Scene CreateBigAllOpen() => Opened(CreateBig());

        private static void Fill(Scene scene)
        {
            foreach (var composition in scene.Project.CompositionManager.ManagerData.Items.OfType<Composition>())
                foreach (var layer in composition.LayerManager.ManagerData.Items.OfType<Layer>())
                {
                    for (var i = 0; i < 10; i++)
                        layer.ModelEntityManager.AddItem(typeof(Entity));

                    layer.TextureModifierManager.AddItem(typeof(Blur));
                    ((Entity)layer.ModelEntityManager.ManagerData.Items[0]).ModifierManager.AddItem(typeof(TranslateModifier));
                }
        }

        private static Scene SceneOf(string name) => name == "all open" ? CreateBigAllOpen() : CreateBig();

        private static TreeNode SourceOf(Scene scene, string name)
        {
            var composition = scene.Tree.CompositionNodes()[10];
            var layer = Scene.LayersOf(composition).Children[5];
            return name switch
            {
                "composition" => composition,
                "layer" => layer,
                _ => Scene.Group<EntitiesGroup>(layer).Children[5]
            };
        }

        // The mean of five runs after one run that warms up.
        private double MillisecondsOf(Action action)
        {
            action();
            var watch = Stopwatch.StartNew();
            for (var run = 0; run < 5; run++)
                action();

            return watch.Elapsed.TotalMilliseconds / 5;
        }

        [Theory]
        [InlineData("all open", "entity")]
        [InlineData("all open", "layer")]
        [InlineData("all open", "composition")]
        [InlineData("as made", "entity")]
        public void CandidatesAndBrightRows_ARowOfABigProject_StayInTheBudget(string sceneName, string sourceName)
        {
            var scene = SceneOf(sceneName);
            var snapshot = ListTreeSnapshot.Of(scene.Tree.Roots);
            var source = snapshot.RowOf(SourceOf(scene, sourceName))!;
            var candidates = new ListTreeCandidates(new ListTreeSlots(service), service, ListTreeSizes.Default);
            IReadOnlyList<ListTreeCandidate> made = Array.Empty<ListTreeCandidate>();

            var candidatesMs = MillisecondsOf(() => made = candidates.Of(snapshot, source));
            var brightRowsMs = MillisecondsOf(() => ListTreeBrightRows.Of(ListTreeLiveRows.Of(snapshot), source, made));

            output.WriteLine($"{sceneName}, {sourceName}: {snapshot.AllRows().Count} rows, {made.Count} candidates in {candidatesMs:F2} ms, bright rows in {brightRowsMs:F2} ms");
            Assert.NotEmpty(made);
            Assert.InRange(candidatesMs, 0, BudgetMs);
            Assert.InRange(brightRowsMs, 0, BudgetMs);
        }
    }
}
