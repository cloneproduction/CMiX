using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CMiX.UI.Controls.Boolean;
using CMiX.UI.Controls.Enum;
using CMiX.UI.Controls.Number;
using CMiX.UI.Controls.Parts;
using CommunityToolkit.Mvvm.Input;
using Xunit;

namespace CMiX.UI.Tests
{
    // Copy and Paste in the menu of a value editor use the clipboard of the system.
    public class ValueMenuCopyPasteTests
    {
        private enum Mode { Add, Multiply }

        private static T Show<T>(T editor) where T : Control
        {
            new Window { Content = editor, Width = 300, Height = 100 }.Show();
            Dispatcher.UIThread.RunJobs();
            return editor;
        }

        private static IValueText Text(Control editor) => (IValueText)editor;

        private static DragValue Drag(double value = 0, string caption = "Position X") =>
            Show(new DragValue { Caption = caption, Value = value, Minimum = 0, Maximum = 10, ResetCommand = new RelayCommand(() => { }) });

        private static MenuItem Row(Control editor, string header) =>
            editor.ContextMenu!.Items.OfType<MenuItem>().Single(item => Equals(item.Header, header));

        private static async Task Click(MenuItem row)
        {
            row.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
            await Task.Delay(50);
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void TheMenu_HasCopyAndPaste_AfterTheTitle()
        {
            var box = Drag();

            Assert.Equal(new object?[] { "Position X", "-", "Copy", "Paste", "-", "Reset" },
                box.ContextMenu!.Items.OfType<MenuItem>().Select(item => item.Header).ToArray());
        }

        [AvaloniaFact]
        public async Task CopyOnOneValue_AndPasteOnAnother_MovesTheValue()
        {
            var source = Drag(2.5);
            var target = Drag(9);

            await Click(Row(source, "Copy"));
            await Click(Row(target, "Paste"));

            Assert.Equal(2.5, target.Value);
        }

        [AvaloniaFact]
        public void ADragValue_KeepsThePastedValueInItsRange()
        {
            var box = Drag();

            Text(box).PasteValueText("50");
            Assert.Equal(10, box.Value);

            Text(box).PasteValueText("-3");
            Assert.Equal(0, box.Value);
        }

        [AvaloniaFact]
        public void AnIntegerValue_RoundsThePastedValue()
        {
            var box = Show(new IntegerValue { Value = 0, Minimum = 0, Maximum = 10 });

            Text(box).PasteValueText("3.6");

            Assert.Equal(4, box.Value);
        }

        [AvaloniaFact]
        public void ANumber_IsCopiedWithoutTheCulture()
        {
            var box = Drag(1.5);

            Assert.Equal("1.5", Text(box).CopyValueText());
        }

        [AvaloniaTheory]
        [InlineData("")]
        [InlineData("abc")]
        [InlineData("NaN")]
        public void ATextThatIsNotANumber_CannotBePasted(string text)
        {
            var box = Drag(3);

            Assert.False(Text(box).CanPasteValueText(text));
            Text(box).PasteValueText(text);
            Assert.Equal(3, box.Value);
        }

        [AvaloniaFact]
        public void AReadOnlyDragValue_CannotBePastedInto()
        {
            var box = Drag(3);
            box.IsReadOnly = true;

            Assert.False(Text(box).CanPasteValueText("5"));
            Text(box).PasteValueText("5");
            Assert.Equal(3, box.Value);
        }

        [AvaloniaFact]
        public void ASlider_CopiesAndPastesItsValue()
        {
            var slider = Show(new SliderCMiX { Minimum = 0, Maximum = 1, Value = 0.25 });

            Assert.Equal("0.25", Text(slider).CopyValueText());
            Text(slider).PasteValueText("5");
            Assert.Equal(1, slider.Value);
        }

        [AvaloniaFact]
        public void AToggle_CopiesAndPastesItsState()
        {
            var toggle = Show(new ToggleButtonCMiX { IsChecked = true });

            Assert.Equal("True", Text(toggle).CopyValueText());
            Text(toggle).PasteValueText("False");
            Assert.False(toggle.IsChecked);
            Assert.False(Text(toggle).CanPasteValueText("3"));
        }

        [AvaloniaFact]
        public void ACombo_CopiesTheNameOfTheItem_AndPastesItByName()
        {
            var combo = Show(new ComboBoxCaptioned { ItemsSource = new[] { Mode.Add, Mode.Multiply }, SelectedItem = Mode.Add });

            Assert.True(Text(combo).CanCopyValueText());
            Assert.Equal("Add", Text(combo).CopyValueText());

            Text(combo).PasteValueText("multiply");
            Assert.Equal(Mode.Multiply, combo.SelectedItem);
            Assert.False(Text(combo).CanPasteValueText("Divide"));
        }

        [AvaloniaFact]
        public void ACombo_OfObjects_HasNothingToCopy()
        {
            var combo = Show(new ComboBoxCaptioned { ItemsSource = new[] { new object(), new object() } });
            combo.SelectedItem = ((object[])combo.ItemsSource)[0];

            Assert.False(Text(combo).CanCopyValueText());
            Assert.False(Text(combo).CanPasteValueText("System.Object"));
        }
    }
}
