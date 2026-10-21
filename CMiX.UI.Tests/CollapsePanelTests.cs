using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using CMiX.UI.Panels;
using Xunit;

namespace CMiX.UI.Tests
{
    // A CollapsePanel opens and closes its content with an animation, and it does not lay the content out again for it.
    public class CollapsePanelTests
    {
        private sealed class Counted : Border
        {
            public int Measures { get; private set; }

            protected override Size MeasureOverride(Size availableSize)
            {
                Measures++;
                return base.MeasureOverride(availableSize);
            }
        }

        private static (CollapsePanel panel, Counted content) Show(bool isExpanded, TimeSpan? duration = null)
        {
            var content = new Counted { Height = 100, Child = new Button { Content = "Content" } };
            var panel = new CollapsePanel { IsExpanded = isExpanded, Child = content };
            if (duration is { } time)
                panel.Duration = time;
            new Window { Content = new StackPanel { Children = { panel } }, Width = 300, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();
            return (panel, content);
        }

        [AvaloniaFact]
        public void AnOpenPanel_HasTheHeightOfItsContent()
        {
            var (panel, _) = Show(true);

            Assert.Equal(100, panel.Bounds.Height);
        }

        [AvaloniaFact]
        public void AClosedPanel_HasNoHeight_AndItsContentIsHidden()
        {
            var (panel, content) = Show(false);

            Assert.Equal(0, panel.Bounds.Height);
            Assert.False(content.IsVisible);
        }

        [AvaloniaFact]
        public void TheHeight_IsTheHeightOfTheContentTimesTheProgress()
        {
            var (panel, _) = Show(true, TimeSpan.Zero);

            panel.Progress = 0.5;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(50, panel.Bounds.Height);
        }

        [AvaloniaFact]
        public void TheFirstState_ShowsAtOnce()
        {
            var (panel, content) = Show(true);

            Assert.Equal(1, panel.Progress);
            Assert.True(content.IsVisible);
        }

        [AvaloniaFact]
        public void Opening_ShowsTheContentAtOnce_AndTheHeightGrowsWithAnAnimation()
        {
            var (panel, content) = Show(false);

            panel.IsExpanded = true;
            Dispatcher.UIThread.RunJobs();

            Assert.True(content.IsVisible);
            Assert.InRange(panel.Progress, 0, 0.999);
        }

        [AvaloniaFact]
        public void Closing_KeepsTheContentVisibleWhileItShrinks()
        {
            var (panel, content) = Show(true);

            panel.IsExpanded = false;
            Dispatcher.UIThread.RunJobs();

            Assert.True(content.IsVisible);
            Assert.InRange(panel.Progress, 0.001, 1);
        }

        [AvaloniaFact]
        public void AfterTheClosing_TheContentIsHidden_SoItTakesNoFocus()
        {
            var (panel, content) = Show(true, TimeSpan.Zero);

            panel.IsExpanded = false;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(0, panel.Progress);
            Assert.False(content.IsVisible);
            Assert.Equal(0, panel.Bounds.Height);
        }

        [AvaloniaFact]
        public void TheContent_IsNotMeasuredAgainWhileTheProgressChanges()
        {
            var (panel, content) = Show(true, TimeSpan.Zero);
            var measures = content.Measures;

            foreach (var progress in new[] { 0.2, 0.4, 0.6, 0.8, 1.0 })
            {
                panel.Progress = progress;
                Dispatcher.UIThread.RunJobs();
            }

            Assert.Equal(measures, content.Measures);
        }

        [AvaloniaFact]
        public void WhileOpen_ThePanelFollowsTheHeightOfItsContent()
        {
            var (panel, content) = Show(true);

            content.Height = 160;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(160, panel.Bounds.Height);
        }

        [AvaloniaFact]
        public void TheContent_IsClippedOnlyWhileThePanelMoves()
        {
            var (panel, _) = Show(true, TimeSpan.Zero);
            Assert.False(panel.ClipToBounds);

            panel.Progress = 0.5;
            Dispatcher.UIThread.RunJobs();
            Assert.True(panel.ClipToBounds);

            panel.Progress = 1;
            Dispatcher.UIThread.RunJobs();
            Assert.False(panel.ClipToBounds);
        }
    }
}
