using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using CMiX.Studio.Avalonia.Views;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The scroll of a drag runs while the drag is tracked. It stops at once.
    public class DragAutoScrollerTests
    {
        private static DragAutoScroller Create()
        {
            var treeView = new TreeView();
            return new DragAutoScroller(new TreeViewContext(new UserControl(), treeView, new TreeRowIndex(treeView)));
        }

        [AvaloniaFact]
        public void Track_WhenStopped_StartsTheTimer()
        {
            var scroller = Create();

            scroller.Track(10);

            Assert.True(scroller.IsRunning);
            scroller.StopNow();
        }

        [AvaloniaFact]
        public void StopNow_WhileRunning_StopsTheTimerAtOnce()
        {
            var scroller = Create();
            scroller.Track(10);

            scroller.StopNow();

            Assert.False(scroller.IsRunning);
        }

        [AvaloniaFact]
        public void Track_AfterStopNow_StartsTheTimerAgain()
        {
            var scroller = Create();
            scroller.Track(10);
            scroller.StopNow();

            scroller.Track(12);

            Assert.True(scroller.IsRunning);
            scroller.StopNow();
        }

        [AvaloniaFact]
        public void Dispose_WhileRunning_StopsTheTimer()
        {
            var scroller = Create();
            scroller.Track(10);

            scroller.Dispose();

            Assert.False(scroller.IsRunning);
        }

        [AvaloniaFact]
        public void DragAutoScroller_EdgeAndStep_EqualTheTokens()
        {
            var application = Application.Current!;

            Assert.Equal(DragAutoScroller.EdgeZone, Convert.ToDouble(application.FindResource("DragScrollEdge")));
            Assert.Equal(DragAutoScroller.MaxScrollStep, Convert.ToDouble(application.FindResource("DragScrollStep")));
        }
    }
}
