using Avalonia.Headless.XUnit;
using Avalonia.Logging;
using Avalonia.Threading;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class BindingIntegrityTests
    {
        // Measured against the current, known good binding graph: constructing MainWindow and
        // pumping the dispatcher settles at exactly 17 "Could not find a matching property
        // accessor" warnings, all against CMiX.Core.Prefabs.Managers.PrefabManager (properties
        // like MaterialSettings, ModifierManagerIsExpanded, BeatSteps and so on). These are the
        // known startup accessor transients documented in the Avalonia migration memory: while
        // DataContext propagates top down, a nested view's binding evaluates once against the
        // shared PrefabManager ancestor context before its own local context attaches, then
        // never fires again once the real context is in place. The count reproduced identically
        // across repeated local runs, so the ceiling is pinned to that exact measured value
        // rather than padded. A newly introduced broken binding, such as a typo in a property
        // name, adds a distinct accessor error this graph does not produce today, pushing the
        // total past 16 and failing the test, while the known transient noise stays at exactly
        // that number.
        //
        // The measured value was 16 until the textures tab became a RepositoryTab. A RepositoryTab
        // roots its SelectionPanel and EditingPanel in the logical tree at assignment time, see
        // LogicalPanelContent, which is what makes their deferred bindings apply at all; the price
        // is that each editing panel evaluates its bindings once against the manager DataContext
        // at load. The five tabs converted earlier already contribute their panel paths that way
        // (MaterialSettings, DiffuseTexture, TransformSRTIsExpanded and so on) and the textures
        // panel adds exactly one more, TextureModifierManager. RepositoryTabTests asserts the same
        // panel scopes to the selected texture afterward, which is what makes it a transient.
        private const int BaselineMaxAccessorErrors = 17;

        private const string AccessorErrorFragment = "Could not find a matching property accessor";

        [AvaloniaFact]
        public void MainWindow_Construction_DoesNotExceedBaselineAccessorErrorCount()
        {
            var originalSink = Logger.Sink;
            var sink = new RecordingLogSink(originalSink);
            Logger.Sink = sink;

            try
            {
                var provider = TestServiceProviderFactory.Create();
                var window = TestServiceProviderFactory.CreateMainWindow(provider);
                window.Show();

                // Several passes let deferred bindings finish settling before the count is read.
                for (int i = 0; i < 5; i++)
                    Dispatcher.UIThread.RunJobs();

                var accessorErrors = sink.Entries
                    .Where(e => e.Area == LogArea.Binding && e.FlattenedText.Contains(AccessorErrorFragment))
                    .Select(e => e.FlattenedText)
                    .ToList();

                Assert.True(
                    accessorErrors.Count <= BaselineMaxAccessorErrors,
                    $"Expected at most {BaselineMaxAccessorErrors} '{AccessorErrorFragment}' binding errors " +
                    $"while constructing and showing MainWindow, found {accessorErrors.Count}:\n" +
                    string.Join("\n", accessorErrors.Distinct()));
            }
            finally
            {
                Logger.Sink = originalSink;
            }
        }
    }
}
