using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.Parts;
using Xunit;

namespace CMiX.UI.Tests
{
    // The clear button of a search field (TextBoxSearch.ShowsClear): inside the field at its right, with no background, only while the field
    // holds text. A click empties the field and keeps the focus in it. Without ShowsClear the field has no clear button.
    public class TextBoxSearchTests
    {
        private static (Window Window, TextBoxSearch Box) Show(bool showsClear, string text = "")
        {
            var box = new TextBoxSearch { Width = 200, ShowsClear = showsClear, Text = text, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
            var window = new Window { Content = box, Width = 400, Height = 200 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            return (window, box);
        }

        private static TextBox InputOf(TextBoxSearch box) => box.GetVisualDescendants().OfType<TextBox>().First();

        private static Rect BoundsIn(Window window, Control control) =>
            new(control.TranslatePoint(default, window)!.Value, control.Bounds.Size);

        [AvaloniaFact]
        public void Show_WithShowsClearAndAText_PutsTheButtonInsideTheFieldAtItsRight()
        {
            var (window, box) = Show(true, "Echo");

            var field = BoundsIn(window, box);
            var clear = BoundsIn(window, box.ClearButton!);

            Assert.True(box.ClearButton!.IsVisible);
            Assert.InRange(clear.Left, field.Center.X, field.Right);
            Assert.InRange(clear.Right, field.Right - 1, field.Right + 0.5);
            Assert.Equal(field.Center.Y, clear.Center.Y, 0.51);
        }

        // The button shows when the text comes. It must not make the field higher, or what is under the field moves down.
        [AvaloniaFact]
        public void Type_WithShowsClear_KeepsTheHeightOfTheField()
        {
            var (window, box) = Show(true);
            var height = Convert.ToDouble(Application.Current!.FindResource("ControlHeightMD"));
            Assert.Equal(height, box.Bounds.Height, 0.01);

            box.Text = "Echo";
            window.UpdateLayout();
            Dispatcher.UIThread.RunJobs();

            var clear = box.GetVisualDescendants().OfType<Button>().First(button => button.Name == TextBoxSearch.ClearPartName);
            Assert.True(clear.IsVisible);
            Assert.Equal(height, box.Bounds.Height, 0.01);
            Assert.True(clear.Bounds.Height <= height + 0.01, "clear: " + clear.Bounds.Height);
        }

        [AvaloniaFact]
        public void Show_WithShowsClear_GivesTheButtonNoBackground()
        {
            var (_, box) = Show(true, "Echo");

            var border = box.ClearButton!.GetVisualDescendants().OfType<Border>().First(part => part.Name == "border");

            Assert.True(border.Background is null || border.Background is ISolidColorBrush { Color.A: 0 }, "the clear button has a background");
        }

        [AvaloniaFact]
        public void Type_WithShowsClear_ShowsTheButtonOnlyWhileTheFieldHoldsText()
        {
            var (_, box) = Show(true);
            Assert.False(box.ClearButton!.IsVisible);

            box.Text = "Echo";
            Dispatcher.UIThread.RunJobs();
            Assert.True(box.ClearButton!.IsVisible);

            box.Text = string.Empty;
            Dispatcher.UIThread.RunJobs();
            Assert.False(box.ClearButton!.IsVisible);
        }

        [AvaloniaFact]
        public void Click_TheClearButton_EmptiesTheFieldAndKeepsTheFocusInIt()
        {
            var (window, box) = Show(true, "Echo");
            box.FocusInput();
            Dispatcher.UIThread.RunJobs();
            var clear = BoundsIn(window, box.ClearButton!);

            window.MouseMove(clear.Center);
            window.MouseDown(clear.Center, MouseButton.Left);
            window.MouseUp(clear.Center, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(string.Empty, box.Text);
            Assert.True(InputOf(box).IsFocused);
            Assert.False(box.ClearButton!.Focusable);
        }

        [AvaloniaFact]
        public void Show_AReadOnlyField_DisablesTheClearButton()
        {
            var (_, box) = Show(true, "Echo");

            box.IsReadOnly = true;
            Dispatcher.UIThread.RunJobs();

            Assert.False(box.ClearButton!.IsEffectivelyEnabled);
        }

        [AvaloniaFact]
        public void Show_WithoutShowsClear_HasNoClearButtonAndTheOldPadding()
        {
            var (_, box) = Show(false, "Echo");

            Assert.Null(box.ClearButton);
            Assert.Empty(box.GetVisualDescendants().OfType<Button>());
            Assert.Equal(6, InputOf(box).Padding.Right);
        }

        [AvaloniaFact]
        public void Show_WithShowsClear_KeepsTheTextBeforeTheButton()
        {
            var (_, box) = Show(true);

            Assert.True(InputOf(box).Padding.Right >= box.ClearButton!.Width);
        }
    }
}
