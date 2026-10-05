using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Sources;
using CMiX.Studio.Avalonia.Views;
using CMiX.Studio.Avalonia.Views.Controls;
using CMiX.Studio.Avalonia.Views.Managers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The SequencePlayer view is the VideoPlayer view with a folder picker, an FPS value and a Loop toggle.
    public class SequencePlayerViewTests
    {
        private static (CMiX.Core.Texturing.Sources.SequencePlayer Player, Views.SequencePlayer View) ShowSequencePlayerView()
        {
            var provider = TestServiceProviderFactory.Create();
            var player = (CMiX.Core.Texturing.Sources.SequencePlayer)provider.GetRequiredService<ControlFactory>()
                .Create(typeof(CMiX.Core.Texturing.Sources.SequencePlayer));

            var view = new Views.SequencePlayer { DataContext = player };
            var window = new Window { Content = view, Width = 500, Height = 600 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (player, view);
        }

        [AvaloniaFact]
        public void PathSelector_SelectsAFolderFromTheImageSequences()
        {
            var (player, view) = ShowSequencePlayerView();

            var pathSelector = view.GetVisualDescendants().OfType<PathSelector>().Single();

            Assert.True(pathSelector.SelectsFolder);
            Assert.Equal("Image Folder", pathSelector.Caption);
            Assert.Same(player.AssetSelector.AssetRepository.ImageSequences, pathSelector.ItemsSource);
        }

        [AvaloniaFact]
        public void View_ShowsTheFpsValueAndThePlayAndLoopToggles()
        {
            var (_, view) = ShowSequencePlayerView();

            Assert.Single(view.GetVisualDescendants().OfType<DragValue>(), v => v.Caption == "FPS");

            var toggleCaptions = view.GetVisualDescendants().OfType<ToggleButton>().Select(t => t.Content).ToList();
            Assert.Contains("Play", toggleCaptions);
            Assert.Contains("Loop", toggleCaptions);
        }

        private static Button DoSeekButton(Views.SequencePlayer view) =>
            view.GetVisualDescendants().OfType<Button>().Single(b => b is not ToggleButton && (b.Content as string) == "Do Seek");

        [AvaloniaFact]
        public void PlayLoopAndDoSeek_AreThreeSegmentsOfOneBarAtTheTop()
        {
            var (_, view) = ShowSequencePlayerView();

            var buttons = view.GetVisualDescendants().OfType<Button>().ToList();
            var play = buttons.Single(b => b is ToggleButton && (b.Content as string) == "Play");
            var loop = buttons.Single(b => b is ToggleButton && (b.Content as string) == "Loop");
            var doSeek = DoSeekButton(view);

            Assert.Same(play.GetVisualParent(), loop.GetVisualParent());
            Assert.Same(play.GetVisualParent(), doSeek.GetVisualParent());
            Assert.Equal(new[] { 0, 1, 2 }, new[] { Grid.GetColumn(play), Grid.GetColumn(loop), Grid.GetColumn(doSeek) });
            Assert.True(play.TranslatePoint(new Point(0, 0), view)!.Value.Y < view.GetVisualDescendants().OfType<DragValue>().First().TranslatePoint(new Point(0, 0), view)!.Value.Y);
        }

        [AvaloniaFact]
        public void DoSeek_IsGreyAndTurnsAccentOnlyWhilePressed()
        {
            var (_, view) = ShowSequencePlayerView();
            var doSeek = DoSeekButton(view);
            var border = doSeek.GetVisualDescendants().OfType<Border>().First(b => b.Name == "border");
            Color Current() => ((ISolidColorBrush)border.Background).Color;
            Color Brush(string key) => ((ISolidColorBrush)Application.Current!.FindResource(key)!).Color;
            var states = (IPseudoClasses)doSeek.Classes;

            Assert.Equal(Brush("Grey600Brush"), Current());

            states.Set(":pointerover", true);
            TestServiceProviderFactory.Pump();
            Assert.Equal(Brush("Grey700Brush"), Current());

            states.Set(":pressed", true);
            TestServiceProviderFactory.Pump();
            Assert.Equal(Brush("AccentBrush"), Current());
        }

        [AvaloniaFact]
        public void ListRow_RealizesAsPrefabListItem()
        {
            var provider = TestServiceProviderFactory.Create();
            var player = provider.GetRequiredService<ControlFactory>().Create(typeof(CMiX.Core.Texturing.Sources.SequencePlayer));

            var repositoryManager = new RepositoryManager();
            var window = new Window { Content = repositoryManager, Width = 400, Height = 600 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var listBox = repositoryManager.GetVisualDescendants().OfType<CMiXListBox>().Single();
            listBox.ItemsSource = new List<IControl> { player };
            TestServiceProviderFactory.Pump();

            var container = listBox.ContainerFromItem(player);
            Assert.NotNull(container);
            Assert.NotEmpty(container.GetVisualDescendants().OfType<PrefabListItem>());
        }
    }
}
