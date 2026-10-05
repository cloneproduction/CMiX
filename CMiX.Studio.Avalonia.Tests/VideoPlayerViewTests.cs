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
            var pathSelector = view.GetVisualDescendants().OfType<PathSelector>().Single();

            Assert.Same(play.GetVisualParent(), doSeek.GetVisualParent());
            Assert.Equal(new[] { 0, 1 }, new[] { Grid.GetColumn(play), Grid.GetColumn(doSeek) });
            Assert.True(play.TranslatePoint(new Point(0, 0), view)!.Value.Y < pathSelector.TranslatePoint(new Point(0, 0), view)!.Value.Y);
        }
    }
}
