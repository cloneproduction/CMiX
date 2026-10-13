using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Prefabs;
using CMiX.Studio.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class VideoPlayerViewTests
    {
        [AvaloniaFact]
        public void PlayAndDoSeek_AreTwoSegmentsOfOneBarAtTheTop()
        {
            var provider = TestServiceProviderFactory.Create();
            var player = provider.GetRequiredService<ControlFactory>().Create(typeof(CMiX.Core.Texturing.Sources.VideoPlayer));
            var view = new Views.VideoPlayer { DataContext = player };
            var window = new Window { Content = view, Width = 500, Height = 600 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var buttons = view.GetVisualDescendants().OfType<Button>().ToList();
            var play = buttons.Single(b => b is ToggleButton && (b.Content as string) == "Play");
            var doSeek = buttons.Single(b => b is not ToggleButton && (b.Content as string) == "Do Seek");
            var pathSelector = view.GetVisualDescendants().OfType<AssetPathSelector>().Single();

            Assert.Same(play.GetVisualParent(), doSeek.GetVisualParent());
            Assert.Equal(new[] { 0, 1 }, new[] { Grid.GetColumn(play), Grid.GetColumn(doSeek) });
            Assert.True(play.TranslatePoint(new Point(0, 0), view)!.Value.Y < pathSelector.TranslatePoint(new Point(0, 0), view)!.Value.Y);
        }

        // The user picks a video in the file dialog. The video must appear in the list and be selected.
        [AvaloniaFact]
        public void ChoosingAVideoFile_AddsItToTheListAndSelectsIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var player = (CMiX.Core.Texturing.Sources.VideoPlayer)provider.GetRequiredService<ControlFactory>()
                .Create(typeof(CMiX.Core.Texturing.Sources.VideoPlayer));
            var view = new Views.VideoPlayer { DataContext = player };
            var window = new Window { Content = view, Width = 500, Height = 600 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var file = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "cmix-test-" + Guid.NewGuid().ToString("N") + ".mp4");
            File.WriteAllText(file, "x");
            try
            {
                var pathSelector = view.GetVisualDescendants().OfType<AssetPathSelector>().Single();
                var combo = pathSelector.GetVisualDescendants().OfType<ComboBox>().Single();

                pathSelector.ChoosePath(file);
                Dispatcher.UIThread.RunJobs();

                Assert.Equal(file, player.AssetSelector.Asset?.FilePath);
                Assert.Contains(player.AssetSelector.AssetRepository.Videos, v => v.FilePath == file);
                Assert.Equal(file, (combo.SelectedItem as CMiX.Core.Assets.IAsset)?.FilePath);
            }
            finally
            {
                File.Delete(file);
            }
        }
    }
}
