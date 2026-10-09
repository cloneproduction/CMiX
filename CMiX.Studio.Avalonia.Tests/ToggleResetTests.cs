using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources;
using CMiX.Studio.Avalonia.Views.Controls;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // A toggle shows a Reset menu only when it has a reset command, and the menu restores the default.
    public class ToggleResetTests
    {
        private static void Host(Control content)
        {
            new Window { Content = content, Width = 400, Height = 300 }.Show();
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void Toggle_WithoutResetCommand_HasNoMenu()
        {
            var toggle = new CaptionedToggleButton();
            Host(toggle);

            Assert.Null(toggle.ContextMenu);
        }

        [AvaloniaFact]
        public void Toggle_WithResetCommand_HasAResetMenuThatRunsIt()
        {
            var ran = false;
            var toggle = new CaptionedToggleButton { ResetCommand = new RelayCommand(() => ran = true) };
            Host(toggle);

            toggle.ContextMenu!.Items.OfType<MenuItem>().Single().Command!.Execute(null);

            Assert.True(ran);
        }

        [AvaloniaFact]
        public void Toggle_LosesTheMenu_WhenTheCommandIsRemoved()
        {
            var toggle = new CaptionedToggleButton { ResetCommand = new RelayCommand(() => { }) };
            Host(toggle);
            Assert.NotNull(toggle.ContextMenu);

            toggle.ResetCommand = null!;

            Assert.Null(toggle.ContextMenu);
        }

        [AvaloniaFact]
        public void ThresholdAntialiasingToggle_ResetRestoresTheModelDefault()
        {
            var threshold = (Threshold)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(Threshold));
            var view = new Views.Threshold { DataContext = threshold };
            Host(view);
            Assert.True(threshold.Antialiasing.Value);
            threshold.Antialiasing.Value = false;

            var toggle = view.GetVisualDescendants().OfType<CaptionedToggleButton>().Single(t => t.Caption == "Antialiasing");
            toggle.ContextMenu!.Items.OfType<MenuItem>().Single().Command!.Execute(null);

            Assert.True(threshold.Antialiasing.Value);
        }

        [AvaloniaFact]
        public void PlainToggle_WithoutResetCommand_HasNoMenu()
        {
            var toggle = new CMiXToggleButton();
            Host(toggle);

            Assert.Null(toggle.ContextMenu);
        }

        [AvaloniaFact]
        public void PlainToggle_WithResetCommand_HasAResetMenuThatRunsIt()
        {
            var ran = false;
            var toggle = new CMiXToggleButton { ResetCommand = new RelayCommand(() => ran = true) };
            Host(toggle);

            toggle.ContextMenu!.Items.OfType<MenuItem>().Single().Command!.Execute(null);

            Assert.True(ran);
        }

        [AvaloniaFact]
        public void PlainToggle_LosesTheMenu_WhenTheCommandIsRemoved()
        {
            var toggle = new CMiXToggleButton { ResetCommand = new RelayCommand(() => { }) };
            Host(toggle);
            Assert.NotNull(toggle.ContextMenu);

            toggle.ResetCommand = null;

            Assert.Null(toggle.ContextMenu);
        }

        [AvaloniaFact]
        public void SequencePlayerToggles_ResetRestoresTheModelDefault()
        {
            var player = (SequencePlayer)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(SequencePlayer));
            var view = new Views.SequencePlayer { DataContext = player };
            Host(view);
            Assert.True(player.Play.Value);
            Assert.True(player.Loop.Value);
            player.Play.Value = false;
            player.Loop.Value = false;

            foreach (var toggle in view.GetVisualDescendants().OfType<CMiXToggleButton>())
                toggle.ContextMenu!.Items.OfType<MenuItem>().Single().Command!.Execute(null);

            Assert.True(player.Play.Value);
            Assert.True(player.Loop.Value);
        }
    }
}
