using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Views.Controls;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class PathSelectorTests
    {
        private static Views.PathSelector ShowPathSelector()
        {
            var pathSelector = new Views.PathSelector();
            var window = new Window { Content = pathSelector, Width = 400, Height = 100 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return pathSelector;
        }

        [AvaloniaFact]
        public void DefaultsToTheFilenameCaptionAndSelectsAFile()
        {
            var pathSelector = ShowPathSelector();

            Assert.Equal("Filename", pathSelector.Caption);
            Assert.False(pathSelector.SelectsFolder);
        }

        [AvaloniaFact]
        public void ShowsItsCaptionInTheLabel()
        {
            var pathSelector = ShowPathSelector();
            var label = pathSelector.GetVisualDescendants().OfType<LabeledContent>().Single();
            Assert.Equal("Filename", label.Caption);

            pathSelector.Caption = "Folder";
            TestServiceProviderFactory.Pump();

            Assert.Equal("Folder", label.Caption);
        }
    }
}
