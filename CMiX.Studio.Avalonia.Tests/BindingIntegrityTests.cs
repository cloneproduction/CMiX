using Avalonia.Headless.XUnit;
using Avalonia.Logging;
using Avalonia.Threading;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class BindingIntegrityTests
    {
        // Measured against the current, known good binding graph: constructing MainWindow and
        // pumping the dispatcher settles at exactly 14 "Could not find a matching property
        // accessor" warnings, all against CMiX.Core.Prefabs.Managers.PrefabManager (properties
        // like MaterialSettings, ModifierManagerIsExpanded, TextureModifierManager and so on).
        // These are the known startup accessor transients documented in the Avalonia migration
        // memory: while DataContext propagates top down, a nested view's binding evaluates once
        // against the shared PrefabManager ancestor context before its own local context
        // attaches, then never fires again once the real context is in place. The count
        // reproduced identically across repeated local runs, so the ceiling is pinned to that
        // exact measured value rather than padded. A newly introduced broken binding, such as a
        // typo in a property name, adds a distinct accessor error this graph does not produce
        // today, pushing the total past the ceiling and failing the test, while the known
        // transient noise stays at exactly that number.
        private const int BaselineMaxAccessorErrors = 14;

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
