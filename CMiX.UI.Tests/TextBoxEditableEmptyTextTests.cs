using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using CMiX.UI.Controls.String;
using Xunit;

namespace CMiX.UI.Tests
{
    // A name is never empty. When the edit ends with an empty text, the old text comes back.
    public class TextBoxEditableEmptyTextTests
    {
        private static (Window Window, TextBoxEditable Box, TextBox Input, Button Other) Show(string text)
        {
            var box = new TextBoxEditable { Text = text };
            var other = new Button { Content = "other", Focusable = true };
            var window = new Window { Content = new StackPanel { Children = { box, other } }, Width = 300, Height = 200 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (window, box, box.FindControl<TextBox>("inputValue")!, other);
        }

        private static void Edit(TextBoxEditable box, TextBox input, string text)
        {
            box.IsEditing = true;
            Dispatcher.UIThread.RunJobs();
            input.Text = text;
        }

        [AvaloniaFact]
        public void Enter_WithAnEmptyText_KeepsTheOldText()
        {
            var (window, box, input, _) = Show("Layer.001");
            Edit(box, input, string.Empty);

            window.KeyPress(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null);

            Assert.Equal("Layer.001", box.Text);
            Assert.False(input.IsVisible);
        }

        [AvaloniaFact]
        public void LostFocus_WithAnEmptyText_KeepsTheOldText()
        {
            var (_, box, input, other) = Show("Layer.001");
            Edit(box, input, string.Empty);

            other.Focus();
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("Layer.001", box.Text);
        }

        [AvaloniaFact]
        public void Enter_WithOnlySpaces_KeepsTheOldText()
        {
            var (window, box, input, _) = Show("Layer.001");
            Edit(box, input, "   ");

            window.KeyPress(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null);

            Assert.Equal("Layer.001", box.Text);
        }

        [AvaloniaFact]
        public void Edit_AfterAnEmptyText_ShowsTheOldTextAgain()
        {
            var (window, box, input, _) = Show("Layer.001");
            Edit(box, input, string.Empty);
            window.KeyPress(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null);

            box.IsEditing = true;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("Layer.001", input.Text);
        }

        [AvaloniaFact]
        public void Enter_WithARealText_CommitsIt()
        {
            var (window, box, input, _) = Show("Layer.001");
            Edit(box, input, "Floor");

            window.KeyPress(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null);

            Assert.Equal("Floor", box.Text);
        }

        [AvaloniaFact]
        public void LostFocus_WithoutAnEdit_WritesNoStaleText()
        {
            var (_, box, input, other) = Show("Layer.001");
            Edit(box, input, "Floor");
            box.IsEditing = false;
            other.Focus();
            Dispatcher.UIThread.RunJobs();
            box.Text = "Changed outside";

            other.Focus();
            input.Focus();
            other.Focus();
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("Changed outside", box.Text);
        }
    }
}
