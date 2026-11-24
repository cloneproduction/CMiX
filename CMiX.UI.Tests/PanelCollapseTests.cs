using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Panels;
using CMiX.UI.Tests.Support;
using CMiX.UI.Themes;
using Xunit;

namespace CMiX.UI.Tests
{
    // A PanelCollapse opens and closes its content with an animation, and it does not lay the content out again for it.
    // Its time comes from Motion, as the time of the chevron.
    public class PanelCollapseTests
    {
        // A PanelCollapse opens and closes its content with an animation, and it does not lay the content out again for it.

        private sealed class Counted : Border
        {
            public int Measures { get; private set; }

            protected override Size MeasureOverride(Size availableSize)
            {
                Measures++;
                return base.MeasureOverride(availableSize);
            }
        }

        private static (PanelCollapse panel, Counted content) Show(bool isExpanded, TimeSpan? duration = null)
        {
            var content = new Counted { Height = 100, Child = new Button { Content = "Content" } };
            var panel = new PanelCollapse { IsExpanded = isExpanded, Child = content };
            if (duration is { } time)
                panel.Duration = time;
            TestWindow.Show(new StackPanel { Children = { panel } }, 300, 400);
            return (panel, content);
        }

        [AvaloniaFact]
        public void Open_APanel_HasTheHeightOfItsContent()
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
        public void Progress_AnyValue_GivesTheHeightOfTheContentTimesTheProgress()
        {
            var (panel, _) = Show(true, TimeSpan.Zero);

            panel.Progress = 0.5;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(50, panel.Bounds.Height);
        }

        [AvaloniaFact]
        public void Show_TheFirstState_AppearsAtOnce()
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
        public void Close_APanel_KeepsTheContentVisibleWhileItShrinks()
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
        public void Progress_AChange_DoesNotMeasureTheContentAgain()
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
        public void Open_ANewContentHeight_ResizesThePanel()
        {
            var (panel, content) = Show(true);

            content.Height = 160;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(160, panel.Bounds.Height);
        }

        [AvaloniaFact]
        public void Clip_OfTheContent_IsSetOnlyWhileThePanelMoves()
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

        // Every expander moves the same way: the panel and the chevron take their time from Motion.

        [AvaloniaFact]
        public void Duration_APanelAndAChevron_AreTheDurationOfMotion()
        {
            Assert.Equal(Motion.Duration, new PanelCollapse().Duration);
            Assert.Equal(Motion.Duration, new Chevron().Duration);
        }
    }
}
