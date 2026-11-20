using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using CMiX.UI.Controls.String;
using Xunit;

namespace CMiX.UI.Tests
{
    // Escape and a removal from the tree cancel the edit. Enter commits it.
    public class TextBoxEditableCancelTests
    {
        private static (Window Window, TextBoxEditable Box, TextBox Input) ShowEditing(string text, string newText)
        {
            var box = new TextBoxEditable { Text = text };
            var window = new Window { Content = new StackPanel { Children = { box } }, Width = 300, Height = 200 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var input = box.FindControl<TextBox>("inputValue")!;
            box.IsEditing = true;
            Dispatcher.UIThread.RunJobs();
            input.Text = newText;
            return (window, box, input);
        }

        [AvaloniaFact]
        public void Escape_DuringAnEdit_KeepsTheOldText()
        {
            var (window, box, input) = ShowEditing("Layer.001", "Floor");

            window.KeyPress(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, null);
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("Layer.001", box.Text);
            Assert.False(input.IsVisible);
            Assert.False(box.IsEditing);
            Assert.False(box.HasTopLevelHandlers);
        }

        [AvaloniaFact]
        public void Enter_DuringAnEdit_CommitsTheText()
        {
            var (window, box, input) = ShowEditing("Layer.001", "Floor");

            window.KeyPress(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null);
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("Floor", box.Text);
            Assert.False(input.IsVisible);
        }

        [AvaloniaFact]
        public void Detach_DuringAnEdit_KeepsTheOldTextAndRemovesTheHandlers()
        {
            var (window, box, input) = ShowEditing("Layer.001", "Floor");
            Assert.True(box.HasTopLevelHandlers);

            window.Content = new Border();
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("Layer.001", box.Text);
            Assert.False(input.IsVisible);
            Assert.False(box.HasTopLevelHandlers);
        }
    }
}
