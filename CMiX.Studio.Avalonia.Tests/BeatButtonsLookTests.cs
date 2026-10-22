using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Panels;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The buttons of a beat row take their corners from their place in the row.
    public class BeatButtonsLookTests
    {
        private static ControlPosition PositionOfButton(Control view, string content)
        {
            new Window { Content = view, Width = 400, Height = 100 }.Show();
            Dispatcher.UIThread.RunJobs();
            var button = view.GetVisualDescendants().OfType<ContentControl>().First(candidate => candidate.Content as string == content);
            return PositionedControl.GetPosition(button);
        }

        [AvaloniaTheory]
        [InlineData("-", ControlPosition.None)]
        [InlineData("+", ControlPosition.Right)]
        public void BeatControls_Buttons_TakeTheirCorners(string content, ControlPosition expected) =>
            Assert.Equal(expected, PositionOfButton(new BeatControls(), content));

        [AvaloniaTheory]
        [InlineData("P", ControlPosition.Left)]
        [InlineData("-", ControlPosition.None)]
        [InlineData("+", ControlPosition.Right)]
        public void MasterBeatControl_Buttons_TakeTheirCorners(string content, ControlPosition expected) =>
            Assert.Equal(expected, PositionOfButton(new MasterBeatControl(), content));
    }
}
