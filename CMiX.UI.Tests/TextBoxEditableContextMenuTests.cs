using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.String;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // The name in a row of a list can be edited. The menu of the row must not open in the edit mode,
    // and the right-click that ends the edit mode must not open it either, as for the slider.
    public class TextBoxEditableContextMenuTests
    {
        private static (Window Window, Border Row, TextBoxEditable Box, TextBox Input) Show()
        {
            var box = new TextBoxEditable { Text = "Name" };
            var row = new Border
            {
                Width = 200,
                Height = 30,
                Child = box,
                ContextMenu = new ContextMenu { ItemsSource = new[] { new MenuItem { Header = "Rename" } } }
            };
            var window = TestWindow.Show(row, 300, 200);
            return (window, row, box, box.GetVisualDescendants().OfType<TextBox>().Single());
        }

        private static void RightClick(Window window, Control control) =>
            PointerInput.Click(window, control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value, MouseButton.Right);

        [AvaloniaFact]
        public void ContextRequest_OutsideTheEditMode_OpensTheMenuOfTheRow()
        {
            var (_, row, _, input) = Show();

            ContextMenuInput.Request(input);

            Assert.True(row.ContextMenu!.IsOpen);
        }

        [AvaloniaFact]
        public void ContextRequest_WhileEditing_OpensNoMenu()
        {
            var (_, row, box, input) = Show();
            box.IsEditing = true;
            Dispatcher.UIThread.RunJobs();

            ContextMenuInput.Request(input);

            Assert.False(row.ContextMenu!.IsOpen);
        }

        [AvaloniaFact]
        public void ContextRequest_AfterADoubleClickEditMode_OpensNoMenu()
        {
            var (window, row, box, input) = Show();
            var text = box.GetVisualDescendants().OfType<TextBlock>().Single(block => block.Name == "textDisplay");
            var pressed = new PointerPressedEventArgs(text, new Pointer(Pointer.GetNextFreeId(), PointerType.Mouse, true), window, new Point(5, 5), 1,
                new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.LeftButtonPressed), KeyModifiers.None);
            text.RaiseEvent(new TappedEventArgs(InputElement.DoubleTappedEvent, pressed));
            Dispatcher.UIThread.RunJobs();
            Assert.True(input.IsVisible);

            ContextMenuInput.Request(input);

            Assert.False(row.ContextMenu!.IsOpen);
        }

        [AvaloniaFact]
        public void RightClick_WhileEditing_EndsEditing_AndTheMenuStaysClosed()
        {
            var (window, row, box, _) = Show();
            box.IsEditing = true;
            Dispatcher.UIThread.RunJobs();

            RightClick(window, box);

            Assert.False(box.IsEditing);
            Assert.False(box.GetVisualDescendants().OfType<TextBox>().Single().IsVisible);
            Assert.False(row.ContextMenu!.IsOpen);
        }

        [AvaloniaFact]
        public void ContextRequest_AfterTheRightClickThatEndedEditing_OpensTheMenu()
        {
            var (window, row, box, input) = Show();
            box.IsEditing = true;
            Dispatcher.UIThread.RunJobs();
            RightClick(window, box);

            ContextMenuInput.Request(input);

            Assert.True(row.ContextMenu!.IsOpen);
        }
    }
}
