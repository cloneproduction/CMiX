using Avalonia.Headless.XUnit;
using Avalonia.Logging;
using Avalonia.Threading;
using CMiX.Studio.Avalonia.Tests.Support;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class BindingIntegrityTests : IDisposable
    {
        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

        // Measured on 2026-11-24: MainWindow gives 4 "Could not find a matching property accessor" warnings. Each one is a binding
        // of a view (Texture, Camera, LightEntity, ModelEntity, TextEntity) that runs once against the PrefabManager of the
        // parent before the view gets its own item. A new wrong binding adds one more and fails the test. The number only goes down.
        private const int BaselineMaxAccessorErrors = 4;

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
                var window = windows.Add(TestServiceProviderFactory.CreateMainWindow(provider));
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
