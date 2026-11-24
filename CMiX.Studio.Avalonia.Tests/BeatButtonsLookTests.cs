using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Panels;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The buttons of a beat row take their corners from their place in the row.
    public class BeatButtonsLookTests : IDisposable
    {
        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

        private ControlPosition PositionOfButton(Control view, string content)
        {
            windows.Add(new Window { Content = view, Width = 400, Height = 100 }).Show();
            Dispatcher.UIThread.RunJobs();
            // The buttons + and - are icon buttons: + has the icon Add, - has the icon Sub. The other buttons have a text.
            // The step buttons of a DragValue have a name. The buttons of the view have none.
            var button = content is "+" or "-"
                ? view.GetVisualDescendants().OfType<CMiX.UI.Controls.Boolean.ButtonIcon>().First(candidate => candidate.Name is null && candidate.IconKey == (content == "+" ? "Add" : "Sub"))
                : view.GetVisualDescendants().OfType<ContentControl>().First(candidate => candidate.Content as string == content);
            return PositionedControl.GetPosition(button);
        }

        [AvaloniaTheory]
        [InlineData("-", ControlPosition.None)]
        [InlineData("+", ControlPosition.Right)]
        public void PanelBeat_Buttons_TakeTheirCorners(string content, ControlPosition expected) =>
            Assert.Equal(expected, PositionOfButton(new PanelBeat(), content));

        [AvaloniaTheory]
        [InlineData("P", ControlPosition.Left)]
        [InlineData("-", ControlPosition.None)]
        [InlineData("+", ControlPosition.Right)]
        public void PanelMasterBeat_Buttons_TakeTheirCorners(string content, ControlPosition expected) =>
            Assert.Equal(expected, PositionOfButton(new PanelMasterBeat(), content));
    }
}
