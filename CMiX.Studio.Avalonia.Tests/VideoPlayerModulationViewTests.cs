using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Sources;
using CMiX.Studio.Avalonia.Views.Modulation;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Panels;
using CMiX.UI.Controls.Boolean;
using CMiX.UI.Controls.Number;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class VideoPlayerModulationViewTests
    {
        private static (VideoPlayer Player, Views.VideoPlayer View) Show()
        {
            var provider = TestServiceProviderFactory.Create();
            var player = (VideoPlayer)provider.GetRequiredService<ControlFactory>().Create(typeof(VideoPlayer));

            var view = new Views.VideoPlayer { DataContext = player };
            var window = new Window { Content = view, Width = 500, Height = 700 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (player, view);
        }

        private static ToggleButtonCMiX PlayToggle(Views.VideoPlayer view) =>
            view.GetVisualDescendants().OfType<ToggleButtonCMiX>().Single(t => (t.Content as string) == "Play");

        [AvaloniaFact]
        public void PlayToggle_HasAModulatorAssignButtonAtItsRight()
        {
            var (player, view) = Show();

            var toggle = PlayToggle(view);

            var assign = view.GetVisualDescendants().OfType<ButtonModulatorAssign>()
                .Single(a => ReferenceEquals(a.GetVisualParent(), toggle.GetVisualParent()));
            Assert.Same(player.ModulatorManager, assign.ModulatorManager);
            Assert.True(assign.TranslatePoint(new Point(0, 0), view)!.Value.X > toggle.TranslatePoint(new Point(0, 0), view)!.Value.X);
        }

        [AvaloniaFact]
        public void PlayToggle_FollowsThePlayingValue()
        {
            var (player, view) = Show();
            var toggle = PlayToggle(view);
            Assert.True(toggle.IsChecked);

            player.Playing.Value = false;
            Dispatcher.UIThread.RunJobs();

            Assert.False(toggle.IsChecked);
        }

        [AvaloniaFact]
        public void PlayToggle_IgnoresClicksWhileAModulatorIsBound()
        {
            var (player, view) = Show();
            player.ModulatorManager.AddItem(typeof(LFOModulator));
            var lfo = (LFOModulator)player.ModulatorManager.ManagerData.Items[0];
            var gate = lfo.Outputs.Single(o => o is ModulatorOutput<bool>);
            var toggle = PlayToggle(view);
            Assert.True(toggle.IsHitTestVisible);

            player.Playing.SetModulatorCommand.Execute(new ModulatorOutputSelection(lfo, gate));
            TestServiceProviderFactory.Pump();

            Assert.False(toggle.IsHitTestVisible);
        }

        [AvaloniaFact]
        public void PlayAndDoSeek_KeepTheCaptionSpaceAndTheWidthOfAValueRow()
        {
            var (_, view) = Show();
            var play = PlayToggle(view);
            var doSeek = view.GetVisualDescendants().OfType<Button>()
                .Single(b => b is not ToggleButton && (b.Content as string) == "Do Seek");
            var seekFrame = view.GetVisualDescendants().OfType<Border>()
                .First(b => b.Name == "mainBorder" && b.FindAncestorOfType<DragValue>() != null);

            double Left(Control c) => c.TranslatePoint(new Point(0, 0), view)!.Value.X;

            Assert.Equal(Left(seekFrame), Left(play), 1);
            Assert.Equal(Left(seekFrame), Left(doSeek), 1);
            Assert.Equal(play.Bounds.Width, doSeek.Bounds.Width, 1);
        }

        [AvaloniaFact]
        public void PlayAndDoSeek_AreOnePanelWithRoundedTopAndBottom()
        {
            var (_, view) = Show();
            var play = PlayToggle(view);
            var doSeek = view.GetVisualDescendants().OfType<Button>()
                .Single(b => b is not ToggleButton && (b.Content as string) == "Do Seek");

            Assert.Equal(ControlPosition.Top, PositionedControl.GetPosition(play));
            Assert.Equal(ControlPosition.Bottom, PositionedControl.GetPosition(doSeek));

            var gap = doSeek.TranslatePoint(new Point(0, 0), view)!.Value.Y
                      - (play.TranslatePoint(new Point(0, 0), view)!.Value.Y + play.Bounds.Height);
            Assert.InRange(gap, 0, 4);
        }

        [AvaloniaFact]
        public void SeekFrame_IsAModulatableDragValueFromZero()
        {
            var (player, view) = Show();

            var seek = view.GetVisualDescendants().OfType<DragValue>().Single(d => d.Caption == "Seek Frame");

            Assert.Equal(0, seek.Minimum);
            Assert.True(seek.IsInteger);
            Assert.IsType<ButtonModulatorAssign>(seek.TrailingContent);
            Assert.Same(player.ModulatorManager, ((ButtonModulatorAssign)seek.TrailingContent!).ModulatorManager);
        }

        [AvaloniaFact]
        public void DoSeek_IsAStandaloneButton()
        {
            var (_, view) = Show();

            var doSeek = view.GetVisualDescendants().OfType<Button>()
                .Single(b => b is not ToggleButton && (b.Content as string) == "Do Seek");

            Assert.IsNotType<ButtonModulatorAssign>(doSeek.GetVisualParent());
        }

        [AvaloniaFact]
        public void View_ShowsTheAddModulatorList()
        {
            var (_, view) = Show();

            Assert.Contains(view.GetVisualDescendants().OfType<Views.ModifierManager>(), m => m.Caption == "Add Modulator");
        }
    }
}
