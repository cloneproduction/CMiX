using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CMiX.UI.Controls.Boolean;
using CMiX.UI.Controls.ColorPicker;
using CMiX.UI.Controls.Enum;
using CMiX.UI.Controls.Number;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Tests.Support;
using CommunityToolkit.Mvvm.Input;
using Xunit;

namespace CMiX.UI.Tests
{
    // Copy and Paste in the menu of a value editor go through IValueText: a drag value, an integer, a slider,
    // a toggle, a combo box and the color selector.
    public class ValueMenuCopyPasteTests
    {
        private enum Mode { Add, Multiply }

        // A clipboard in memory, so a test does not touch the clipboard of the system and does not wait for it.
        private sealed class MemoryClipboard : IValueClipboard
        {
            private string? text;

            public Task<string?> GetTextAsync() => Task.FromResult(text);

            public Task SetTextAsync(string value)
            {
                text = value;
                return Task.CompletedTask;
            }
        }

        private static T Show<T>(T editor) where T : Control
        {
            TestWindow.Show(editor, 300, 100);
            return editor;
        }

        private static IValueText Text(Control editor) => (IValueText)editor;

        private static DragValue Drag(double value = 0, string caption = "Position X") =>
            Show(new DragValue { Caption = caption, Value = value, Minimum = 0, Maximum = 10, ResetCommand = new RelayCommand(() => { }) });

        private static MenuItem Row(Control editor, string header) =>
            editor.ContextMenu!.Items.OfType<MenuItem>().Single(item => Equals(item.Header, header));

        private static void Click(MenuItem row)
        {
            row.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void Copy_OnOneValueThenPasteOnAnother_MovesTheValue()
        {
            var clipboard = new MemoryClipboard();
            var source = Drag(2.5);
            var target = Drag(9);
            ValueMenu.SetClipboard(TopLevel.GetTopLevel(source)!, clipboard);
            ValueMenu.SetClipboard(TopLevel.GetTopLevel(target)!, clipboard);

            Click(Row(source, "Copy"));
            Click(Row(target, "Paste"));

            Assert.Equal(2.5, target.Value);
        }

        [AvaloniaFact]
        public void Paste_OnADragValue_KeepsTheValueInItsRange()
        {
            var box = Drag();

            Text(box).PasteValueText("50");
            Assert.Equal(10, box.Value);

            Text(box).PasteValueText("-3");
            Assert.Equal(0, box.Value);
        }

        [AvaloniaFact]
        public void Paste_OnADragValueInteger_RoundsTheValue()
        {
            var box = Show(new DragValueInteger { Value = 0, Minimum = 0, Maximum = 10 });

            Text(box).PasteValueText("3.6");

            Assert.Equal(4, box.Value);
        }

        [AvaloniaFact]
        public void Copy_ANumber_WritesItWithoutTheCulture()
        {
            var box = Drag(1.5);

            Assert.Equal("1.5", Text(box).CopyValueText());
        }

        [AvaloniaTheory]
        [InlineData("")]
        [InlineData("abc")]
        [InlineData("NaN")]
        public void Paste_ATextThatIsNotANumber_IsRefused(string text)
        {
            var box = Drag(3);

            Assert.False(Text(box).CanPasteValueText(text));
            Text(box).PasteValueText(text);
            Assert.Equal(3, box.Value);
        }

        [AvaloniaFact]
        public void Paste_OnAReadOnlyDragValue_IsRefused()
        {
            var box = Drag(3);
            box.IsReadOnly = true;

            Assert.False(Text(box).CanPasteValueText("5"));
            Text(box).PasteValueText("5");
            Assert.Equal(3, box.Value);
        }

        [AvaloniaFact]
        public void CopyAndPaste_OnASlider_KeepTheValue()
        {
            var slider = Show(new SliderCMiX { Minimum = 0, Maximum = 1, Value = 0.25 });

            Assert.Equal("0.25", Text(slider).CopyValueText());
            Text(slider).PasteValueText("5");
            Assert.Equal(1, slider.Value);
        }

        [AvaloniaFact]
        public void CopyAndPaste_OnAToggle_KeepTheState()
        {
            var toggle = Show(new ToggleButtonCMiX { IsChecked = true });

            Assert.Equal("True", Text(toggle).CopyValueText());
            Text(toggle).PasteValueText("False");
            Assert.False(toggle.IsChecked);
            Assert.False(Text(toggle).CanPasteValueText("3"));
        }

        [AvaloniaFact]
        public void CopyAndPaste_OnACombo_UseTheNameOfTheItem()
        {
            var combo = Show(new ComboBoxCaptioned { ItemsSource = new[] { Mode.Add, Mode.Multiply }, SelectedItem = Mode.Add });

            Assert.True(Text(combo).CanCopyValueText());
            Assert.Equal("Add", Text(combo).CopyValueText());

            Text(combo).PasteValueText("multiply");
            Assert.Equal(Mode.Multiply, combo.SelectedItem);
            Assert.False(Text(combo).CanPasteValueText("Divide"));
        }

        [AvaloniaFact]
        public void Copy_OnAComboOfObjects_HasNothing()
        {
            var combo = Show(new ComboBoxCaptioned { ItemsSource = new[] { new object(), new object() } });
            combo.SelectedItem = ((object[])combo.ItemsSource)[0];

            Assert.False(Text(combo).CanCopyValueText());
            Assert.False(Text(combo).CanPasteValueText("System.Object"));
        }

        // The SelectorColor has the same menu as the other value editors.

        private static (SelectorColor Selector, StringValue Model) ShowSelector(string value = "#FF336699")
        {
            var model = new StringValue { Value = value };
            var selector = new SelectorColor { Caption = "Color", DataContext = model, Width = 200, Height = 40 };
            TestWindow.Show(selector, 300, 100);
            return (selector, model);
        }

        [AvaloniaFact]
        public void Open_TheMenuOfASelectorColor_HasTheTitleCopyPasteAndReset()
        {
            var (selector, _) = ShowSelector();

            Assert.Equal(new object?[] { "Color", "-", "Copy", "Paste", "-", "Reset" },
                selector.ContextMenu!.Items.OfType<MenuItem>().Select(item => item.Header).ToArray());
        }

        [AvaloniaFact]
        public void SelectedColor_OfASelectorColor_FollowsTheModelBothWays()
        {
            var (selector, model) = ShowSelector();

            Assert.Equal("#FF336699", Text(selector).CopyValueText());

            model.Value = "#80FF0000";
            Assert.Equal("#80FF0000", Text(selector).CopyValueText());
        }

        [AvaloniaTheory]
        [InlineData("#FF00FF00", "#FF00FF00")]
        [InlineData("#00FF00", "#FF00FF00")]
        [InlineData("  red ", "#FFFF0000")]
        public void Paste_OnASelectorColor_SetsTheModel(string text, string expected)
        {
            var (selector, model) = ShowSelector();

            Assert.True(Text(selector).CanPasteValueText(text));
            Text(selector).PasteValueText(text);
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(expected, model.Value);
        }

        [AvaloniaTheory]
        [InlineData("")]
        [InlineData("not a color")]
        [InlineData("12.5")]
        public void Paste_ATextThatIsNotAColor_IsRefused(string text)
        {
            var (selector, model) = ShowSelector();

            Assert.False(Text(selector).CanPasteValueText(text));
            Text(selector).PasteValueText(text);

            Assert.Equal("#FF336699", model.Value);
        }

        [AvaloniaFact]
        public void Reset_OnASelectorColor_RestoresTheModel()
        {
            var (selector, model) = ShowSelector();

            selector.ContextMenu!.Items.OfType<MenuItem>().Single(item => Equals(item.Header, "Reset"))
                .Command!.Execute(null);

            Assert.Equal("#FF000000", model.Value);
        }
    }
}
