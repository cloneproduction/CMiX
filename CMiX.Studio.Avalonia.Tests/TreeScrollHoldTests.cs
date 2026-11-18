using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // The view holds an anchor row in place while rows close with their animation (TreeScrollAnchor.HoldWhileClosing). A second
    // close during the animation of the first one must not make two holds fight over the scroll, which gives an endless layout.
    public class TreeScrollHoldTests : IDisposable
    {
        private TreeTestScreen? screen;

        public void Dispose() => screen?.Dispose();

        [AvaloniaTheory]
        [InlineData(1.0)]
        [InlineData(1.25)]
        [InlineData(1.5)]
        public void Type_TwoLettersDuringTheCloseAnimation_RunsTheLayoutToItsEnd(double scale)
        {
            // Many compositions with layers, so the rows that the search opens are above and in the view. The animation stays on.
            var scene = CreateScene([6, 6, 6, 6, 6, 6]);
            foreach (var node in TreeTestScreen.All(scene.Tree.Roots))
                node.IsExpanded = false;
            screen = TreeTestScreen.Show(scene.Tree, 400, 300, scale: scale, instant: false);
            var scroller = screen.View.GetVisualDescendants().OfType<ScrollViewer>().First();

            // The search opens the rows of every layer, then the view goes to the bottom.
            scene.Tree.SearchText = "Layer";
            screen.Settle();
            scroller.Offset = scroller.Offset.WithY(scroller.Extent.Height);
            screen.Settle();
            scene.Tree.SelectedNode = scene.Tree.SearchMatches.Last();
            screen.Settle();

            // Each letter closes rows above the view, and the next letter comes before the animation ends.
            foreach (var text in new[] { "Layer 1", "Layer 2", "Layer 3", "Layer 4", "" })
            {
                scene.Tree.SearchText = text;
                Dispatcher.UIThread.RunJobs();
                screen.Window.UpdateLayout();
                AvaloniaHeadlessPlatform.ForceRenderTimerTick();
                screen.Window.UpdateLayout();
            }

            var error = Record.Exception(() => screen.Settle());

            Assert.Null(error);
        }

        // The saved projects of the user, as the compatibility tests of the core use them. Set CMIX_SAMPLE_PROJECTS to a folder with
        // .cmix files. The test does nothing when the variable is not set. A loaded project starts with its rows closed, so a search
        // opens many rows at once, and the end of the search closes them again.
        [AvaloniaTheory]
        [InlineData(1.0)]
        [InlineData(1.25)]
        [InlineData(1.5)]
        public void SearchAndClear_InASavedProject_RunsTheLayoutToItsEnd(double scale)
        {
            var folder = Environment.GetEnvironmentVariable("CMIX_SAMPLE_PROJECTS");
            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
                return;

            foreach (var file in Directory.GetFiles(folder, "*.cmix"))
            {
                var provider = TestServiceProviderFactory.Create();
                var project = provider.GetRequiredService<CMiX.Core.Compositing.Project>();
                var undo = provider.GetRequiredService<CMiX.Core.Undo.UndoManager>();
                var tree = TreeFixtures.CreateTree(provider);
                screen?.Dispose();
                screen = TreeTestScreen.Show(tree, 400, 600, scale: scale, instant: false);

                undo.SuppressUndo();
                try
                {
                    project.ApplySnapshot(CMiX.Core.Persistence.ProjectSerializer.Load(file));
                }
                finally
                {
                    undo.ResumeUndo();
                }
                screen.Settle();

                // The letters of a real name, typed one by one, then a pick and the end of the search.
                var name = TreeTestScreen.All(tree.Roots).Select(node => node.Header).FirstOrDefault(header => header.Length >= 3) ?? "Layer";
                var error = Record.Exception(() =>
                {
                    for (var length = 1; length <= name.Length; length++)
                    {
                        tree.SearchText = name[..length];
                        Dispatcher.UIThread.RunJobs();
                        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
                        screen.Window.UpdateLayout();
                    }

                    screen.Settle();
                    tree.SelectedNode = tree.SearchMatches.LastOrDefault();
                    screen.Settle();
                    tree.SearchText = string.Empty;
                    Dispatcher.UIThread.RunJobs();
                    AvaloniaHeadlessPlatform.ForceRenderTimerTick();
                    screen.Settle();
                });

                Assert.True(error == null, Path.GetFileName(file) + ": " + error);
            }
        }
    }
}
