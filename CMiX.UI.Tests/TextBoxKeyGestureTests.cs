using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using CMiX.UI.Controls.String;
using CMiX.UI.Tests.Support;
using CommunityToolkit.Mvvm.Input;
using Xunit;

namespace CMiX.UI.Tests
{
    // TextBoxKeyGesture shows a gesture as text. While it has the focus, it records the next key combination.
    public class TextBoxKeyGestureTests
    {
        private static (Window Window, TextBoxKeyGesture Box) ShowFocused(KeyGesture? gesture)
        {
            var box = new TextBoxKeyGesture { Gesture = gesture };
            var window = TestWindow.Show(box, 300, 100);
            box.Focus();
            Dispatcher.UIThread.RunJobs();
            Assert.True(box.IsFocused);
            return (window, box);
        }

        private static void Press(Window window, Key key, RawInputModifiers modifiers = RawInputModifiers.None)
        {
            window.KeyPress(key, modifiers, PhysicalKey.None, null);
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void Show_AGesture_ShowsItsText()
        {
            var box = new TextBoxKeyGesture { Gesture = new KeyGesture(Key.S, KeyModifiers.Control) };
            TestWindow.Show(box, 300, 100);

            Assert.Equal("Ctrl+S", box.ShownText);
        }

        [AvaloniaFact]
        public void Show_ANumberKey_ShowsTheDigit()
        {
            var box = new TextBoxKeyGesture { Gesture = new KeyGesture(Key.D1, KeyModifiers.Control) };
            TestWindow.Show(box, 300, 100);

            Assert.Equal("Ctrl+1", box.ShownText);
        }

        [AvaloniaFact]
        public void Show_NoGesture_ShowsNone()
        {
            var box = new TextBoxKeyGesture();
            TestWindow.Show(box, 300, 100);

            Assert.Equal("None", box.ShownText);
        }

        [AvaloniaFact]
        public void KeyDown_WhileFocused_RecordsTheChord()
        {
            var (window, box) = ShowFocused(null);

            Press(window, Key.Z, RawInputModifiers.Control | RawInputModifiers.Shift);

            Assert.Equal(KeyGesture.Parse("Ctrl+Shift+Z"), box.Gesture);
            Assert.False(box.IsFocused);
            Assert.Equal("Ctrl+Shift+Z", box.ShownText);
        }

        [AvaloniaFact]
        public void KeyDown_AModifierAlone_RecordsNothing()
        {
            var old = new KeyGesture(Key.S, KeyModifiers.Control);
            var (window, box) = ShowFocused(old);

            Press(window, Key.LeftCtrl, RawInputModifiers.Control);
            Press(window, Key.LeftShift, RawInputModifiers.Shift);

            Assert.Equal(old, box.Gesture);
            Assert.True(box.IsFocused);
            Assert.Equal("Press keys", box.ShownText);
        }

        [AvaloniaFact]
        public void Escape_WhileFocused_KeepsTheOldGesture()
        {
            var old = new KeyGesture(Key.S, KeyModifiers.Control);
            var (window, box) = ShowFocused(old);

            Press(window, Key.Escape);

            Assert.Equal(old, box.Gesture);
            Assert.False(box.IsFocused);
        }

        [AvaloniaFact]
        public void Backspace_WhileFocused_ClearsTheGesture()
        {
            var (window, box) = ShowFocused(new KeyGesture(Key.S, KeyModifiers.Control));

            Press(window, Key.Back);

            Assert.Null(box.Gesture);
        }

        [AvaloniaFact]
        public void Box_WithoutResetCommand_HasNoMenu()
        {
            var box = new TextBoxKeyGesture();
            TestWindow.Show(box, 300, 100);

            Assert.Null(box.ContextMenu);
        }

        [AvaloniaFact]
        public void Box_WithResetCommand_HasAResetMenuThatRunsIt()
        {
            var ran = false;
            var box = new TextBoxKeyGesture { ResetCommand = new RelayCommand(() => ran = true) };
            TestWindow.Show(box, 300, 100);

            box.ContextMenu!.Items.OfType<MenuItem>().Single(i => Equals(i.Header, "Reset")).Command!.Execute(null);

            Assert.True(ran);
        }

        [AvaloniaFact]
        public void Focus_TheBox_ShowsTheRecordingLook()
        {
            var (_, box) = ShowFocused(null);
            var frame = box.FindControl<Border>("frame")!;

            Assert.True(box.Classes.Contains(":recording"));
            Assert.Equal(Tokens.Get<ISolidColorBrush>("AccentBrush").Color, ((ISolidColorBrush)frame.BorderBrush!).Color);
        }
    }
}
