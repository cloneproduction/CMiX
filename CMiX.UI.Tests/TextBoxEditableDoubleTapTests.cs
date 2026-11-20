using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.String;
using Xunit;

namespace CMiX.UI.Tests
{
    // A double tap on the text of a name starts the edit mode. A double tap on the rest of the row does not,
    // so the row can use it (a row of a tree expands). The tap that the name takes is handled, so the row does not see it.
    public class TextBoxEditableDoubleTapTests
    {
        private static (Window Window, TextBoxEditable Box, TextBlock Text, TextBox Input) Show(string text = "Layer 1")
        {
            var box = new TextBoxEditable { Text = text };
            var window = new Window { Content = box, Width = 400, Height = 100 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (window, box,
                box.GetVisualDescendants().OfType<TextBlock>().Single(block => block.Name == "textDisplay"),
                box.GetVisualDescendants().OfType<TextBox>().Single());
        }

        // Raises a double tap on a control, as the pointer would. Tells if a handler took it.
        private static bool DoubleTap(Window window, Control source)
        {
            var pressed = new PointerPressedEventArgs(source, new Pointer(Pointer.GetNextFreeId(), PointerType.Mouse, true), window, new Point(5, 5), 2,
                new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.LeftButtonPressed), KeyModifiers.None);
            var tapped = new TappedEventArgs(InputElement.DoubleTappedEvent, pressed);
            source.RaiseEvent(tapped);
            Dispatcher.UIThread.RunJobs();
            return tapped.Handled;
        }

        [AvaloniaFact]
        public void DoubleTap_OnTheText_StartsTheEditModeAndHandlesTheTap()
        {
            var (window, _, text, input) = Show();

            var handled = DoubleTap(window, text);

            Assert.True(input.IsVisible);
            Assert.True(handled);
        }

        [AvaloniaFact]
        public void DoubleTap_AwayFromTheText_LeavesTheTapToTheRow()
        {
            var (window, box, _, input) = Show();
            var rest = (Grid)box.Content!;

            var handled = DoubleTap(window, rest);

            Assert.False(input.IsVisible);
            Assert.False(handled);
        }

        [AvaloniaFact]
        public void DoubleTap_InTheInputWhileEditing_KeepsTheEditModeAndHandlesTheTap()
        {
            var (window, box, _, input) = Show();
            box.IsEditing = true;
            Dispatcher.UIThread.RunJobs();
            Assert.True(input.IsVisible);

            var handled = DoubleTap(window, input);

            Assert.True(input.IsVisible);
            Assert.True(handled);
        }

        [AvaloniaFact]
        public void Show_TheText_IsAsWideAsItsTextAndNotAsTheBox()
        {
            var (_, box, text, _) = Show("Layer 1");

            Assert.True(text.Bounds.Width < box.Bounds.Width / 2, $"The text is {text.Bounds.Width} px wide in a box of {box.Bounds.Width} px.");
        }

        [AvaloniaFact]
        public void DoubleTap_AnEmptyName_StillFindsAText()
        {
            var (_, _, text, _) = Show("");
            var height = System.Convert.ToDouble(Application.Current!.FindResource("ControlHeightMD"));

            Assert.True(text.Bounds.Width >= height);
        }

        [AvaloniaFact]
        public void DoubleClick_OnTheTextOrTheRow_StartsTheEditModeOnTheTextOnly()
        {
            var (window, box, text, input) = Show();
            var onText = text.TranslatePoint(new Point(text.Bounds.Width / 2, text.Bounds.Height / 2), window)!.Value;
            var away = box.TranslatePoint(new Point(box.Bounds.Width - 20, box.Bounds.Height / 2), window)!.Value;

            DoubleClick(window, away);
            Assert.False(input.IsVisible);

            DoubleClick(window, onText);
            Assert.True(input.IsVisible);
        }

        private static void DoubleClick(Window window, Point point)
        {
            window.UpdateLayout();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            Dispatcher.UIThread.RunJobs();
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
        }
    }
}
