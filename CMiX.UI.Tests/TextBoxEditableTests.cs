// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.String;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // The name of a row: a text that a double tap turns into an input, with a suffix that follows the text.
    public class TextBoxEditableTests
    {
        private sealed record Shown(Window Window, TextBoxEditable Box, TextBlock Text, TextBox Input, Button Other);

        // The box and a second focusable control, so the box can lose the focus.
        private static Shown Show(string text = "Layer 1", Control? suffix = null)
        {
            var box = new TextBoxEditable { Text = text, Suffix = suffix };
            var other = new Button { Content = "other", Focusable = true };
            var window = TestWindow.Show(new StackPanel { Children = { box, other } }, 400, 200);
            return new Shown(window, box, box.FindControl<TextBlock>("textDisplay")!, box.FindControl<TextBox>("inputValue")!, other);
        }

        // The box in a row of a list that has its own menu.
        private static (Shown Shown, Border Row) ShowInARow()
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
            var shown = new Shown(window, box, box.FindControl<TextBlock>("textDisplay")!, box.FindControl<TextBox>("inputValue")!, new Button());
            return (shown, row);
        }

        private static void StartEdit(Shown shown, string newText)
        {
            shown.Box.IsEditing = true;
            Dispatcher.UIThread.RunJobs();
            shown.Input.Text = newText;
        }

        private static void Press(Window window, Key key, PhysicalKey physicalKey)
        {
            window.KeyPress(key, RawInputModifiers.None, physicalKey, null);
            Dispatcher.UIThread.RunJobs();
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

        private static void DoubleClick(Window window, Point point)
        {
            window.UpdateLayout();
            TestWindow.Render();
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
        }

        private static void RightClick(Window window, Control control) =>
            PointerInput.Click(window, control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value, MouseButton.Right);

        private static double SuffixLeft(Shown shown, Control suffix) => suffix.TranslatePoint(new Point(0, 0), shown.Window)!.Value.X;

        // The start of the edit: a double tap on the text starts it, a double tap on the rest of the row is left to the row.

        [AvaloniaFact]
        public void DoubleTap_OnTheText_StartsTheEditModeAndHandlesTheTap()
        {
            var shown = Show();

            var handled = DoubleTap(shown.Window, shown.Text);

            Assert.True(shown.Input.IsVisible);
            Assert.True(handled);
        }

        [AvaloniaFact]
        public void DoubleTap_AwayFromTheText_LeavesTheTapToTheRow()
        {
            var shown = Show();
            var rest = (Grid)shown.Box.Content!;

            var handled = DoubleTap(shown.Window, rest);

            Assert.False(shown.Input.IsVisible);
            Assert.False(handled);
        }

        [AvaloniaFact]
        public void DoubleTap_InTheInputWhileEditing_KeepsTheEditModeAndHandlesTheTap()
        {
            var shown = Show();
            shown.Box.IsEditing = true;
            Dispatcher.UIThread.RunJobs();
            Assert.True(shown.Input.IsVisible);

            var handled = DoubleTap(shown.Window, shown.Input);

            Assert.True(shown.Input.IsVisible);
            Assert.True(handled);
        }

        [AvaloniaFact]
        public void DoubleClick_OnTheTextOrTheRow_StartsTheEditModeOnTheTextOnly()
        {
            var shown = Show();
            var onText = shown.Text.TranslatePoint(new Point(shown.Text.Bounds.Width / 2, shown.Text.Bounds.Height / 2), shown.Window)!.Value;
            var away = shown.Box.TranslatePoint(new Point(shown.Box.Bounds.Width - 20, shown.Box.Bounds.Height / 2), shown.Window)!.Value;

            DoubleClick(shown.Window, away);
            Assert.False(shown.Input.IsVisible);

            DoubleClick(shown.Window, onText);
            Assert.True(shown.Input.IsVisible);
        }

        // The end of the edit: Enter commits, Escape and a removal from the tree cancel, an empty text keeps the old one.

        [AvaloniaFact]
        public void Escape_DuringAnEdit_KeepsTheOldText()
        {
            var shown = Show("Layer.001");
            StartEdit(shown, "Floor");

            Press(shown.Window, Key.Escape, PhysicalKey.Escape);

            Assert.Equal("Layer.001", shown.Box.Text);
            Assert.False(shown.Input.IsVisible);
            Assert.False(shown.Box.IsEditing);
            Assert.False(shown.Box.HasTopLevelHandlers);
        }

        [AvaloniaFact]
        public void Enter_DuringAnEdit_CommitsTheText()
        {
            var shown = Show("Layer.001");
            StartEdit(shown, "Floor");

            Press(shown.Window, Key.Enter, PhysicalKey.Enter);

            Assert.Equal("Floor", shown.Box.Text);
            Assert.False(shown.Input.IsVisible);
        }

        [AvaloniaFact]
        public void Detach_DuringAnEdit_KeepsTheOldTextAndRemovesTheHandlers()
        {
            var shown = Show("Layer.001");
            StartEdit(shown, "Floor");
            Assert.True(shown.Box.HasTopLevelHandlers);

            shown.Window.Content = new Border();
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("Layer.001", shown.Box.Text);
            Assert.False(shown.Input.IsVisible);
            Assert.False(shown.Box.HasTopLevelHandlers);
        }

        [AvaloniaFact]
        public void Enter_WithAnEmptyText_KeepsTheOldText()
        {
            var shown = Show("Layer.001");
            StartEdit(shown, string.Empty);

            Press(shown.Window, Key.Enter, PhysicalKey.Enter);

            Assert.Equal("Layer.001", shown.Box.Text);
            Assert.False(shown.Input.IsVisible);
        }

        [AvaloniaFact]
        public void Enter_WithOnlySpaces_KeepsTheOldText()
        {
            var shown = Show("Layer.001");
            StartEdit(shown, "   ");

            Press(shown.Window, Key.Enter, PhysicalKey.Enter);

            Assert.Equal("Layer.001", shown.Box.Text);
        }

        [AvaloniaFact]
        public void LostFocus_WithAnEmptyText_KeepsTheOldText()
        {
            var shown = Show("Layer.001");
            StartEdit(shown, string.Empty);

            shown.Other.Focus();
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("Layer.001", shown.Box.Text);
        }

        [AvaloniaFact]
        public void Edit_AfterAnEmptyText_ShowsTheOldTextAgain()
        {
            var shown = Show("Layer.001");
            StartEdit(shown, string.Empty);
            Press(shown.Window, Key.Enter, PhysicalKey.Enter);

            shown.Box.IsEditing = true;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("Layer.001", shown.Input.Text);
        }

        [AvaloniaFact]
        public void LostFocus_WithoutAnEdit_WritesNoStaleText()
        {
            var shown = Show("Layer.001");
            StartEdit(shown, "Floor");
            shown.Box.IsEditing = false;
            shown.Other.Focus();
            Dispatcher.UIThread.RunJobs();
            shown.Box.Text = "Changed outside";

            shown.Other.Focus();
            shown.Input.Focus();
            shown.Other.Focus();
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("Changed outside", shown.Box.Text);
        }

        // The menu of the row: it does not open in the edit mode, nor on the right click that ends the edit mode.

        [AvaloniaFact]
        public void ContextRequest_OutsideTheEditMode_OpensTheMenuOfTheRow()
        {
            var (shown, row) = ShowInARow();

            ContextMenuInput.Request(shown.Input);

            Assert.True(row.ContextMenu!.IsOpen);
        }

        [AvaloniaFact]
        public void ContextRequest_WhileEditing_OpensNoMenu()
        {
            var (shown, row) = ShowInARow();
            shown.Box.IsEditing = true;
            Dispatcher.UIThread.RunJobs();

            ContextMenuInput.Request(shown.Input);

            Assert.False(row.ContextMenu!.IsOpen);
        }

        [AvaloniaFact]
        public void ContextRequest_AfterADoubleClickEditMode_OpensNoMenu()
        {
            var (shown, row) = ShowInARow();
            DoubleTap(shown.Window, shown.Text);
            Assert.True(shown.Input.IsVisible);

            ContextMenuInput.Request(shown.Input);

            Assert.False(row.ContextMenu!.IsOpen);
        }

        [AvaloniaFact]
        public void RightClick_WhileEditing_EndsTheEditAndKeepsTheMenuClosed()
        {
            var (shown, row) = ShowInARow();
            shown.Box.IsEditing = true;
            Dispatcher.UIThread.RunJobs();

            RightClick(shown.Window, shown.Box);

            Assert.False(shown.Box.IsEditing);
            Assert.False(shown.Input.IsVisible);
            Assert.False(row.ContextMenu!.IsOpen);
        }

        [AvaloniaFact]
        public void ContextRequest_AfterTheRightClickThatEndedEditing_OpensTheMenu()
        {
            var (shown, row) = ShowInARow();
            shown.Box.IsEditing = true;
            Dispatcher.UIThread.RunJobs();
            RightClick(shown.Window, shown.Box);

            ContextMenuInput.Request(shown.Input);

            Assert.True(row.ContextMenu!.IsOpen);
        }

        // The layout: the text is as wide as its text and does not move when the edit starts. The suffix follows the text.

        [AvaloniaFact]
        public void Show_TheText_IsAsWideAsItsTextAndNotAsTheBox()
        {
            var shown = Show("Layer 1");

            Assert.True(shown.Text.Bounds.Width < shown.Box.Bounds.Width / 2, $"The text is {shown.Text.Bounds.Width} px wide in a box of {shown.Box.Bounds.Width} px.");
        }

        [AvaloniaFact]
        public void Show_AnEmptyName_KeepsATextToDoubleTap()
        {
            var shown = Show("");
            var height = Tokens.Get<double>("ControlHeightMD");

            Assert.True(shown.Text.Bounds.Width >= height);
        }

        [AvaloniaFact]
        public void Edit_AnyText_KeepsTheTextInTheSamePlace()
        {
            var box = new TextBoxEditable { Text = "Layer 1", FontSize = 12.5 };
            TestWindow.Show(box, 300, 60);
            var grid = (Grid)box.Content!;
            var block = box.FindControl<TextBlock>("textDisplay")!;
            var shownOrigin = block.TranslatePoint(new Point(block.Padding.Left, block.Padding.Top), grid)!.Value;

            box.IsEditing = true;
            box.FindControl<TextBox>("inputValue")!.IsVisible = true;
            Dispatcher.UIThread.RunJobs();
            var presenter = box.GetVisualDescendants().OfType<TextPresenter>().First();
            var editedOrigin = presenter.TranslatePoint(new Point(0, 0), grid)!.Value;

            Assert.Equal(shownOrigin.X, editedOrigin.X);
            Assert.Equal(shownOrigin.Y, editedOrigin.Y);
        }

        [AvaloniaFact]
        public void Show_AnyText_KeepsTheSuffixAtAFixedDistanceFromItsEnd()
        {
            var suffix = new Border { Width = 20, Height = 10 };
            var shown = Show(suffix: suffix);

            var textRight = shown.Text.TranslatePoint(new Point(shown.Text.Bounds.Width, 0), shown.Window)!.Value.X;

            Assert.Equal(4, SuffixLeft(shown, suffix) - textRight, 1);
        }

        [AvaloniaFact]
        public void Show_ALongerText_MovesTheSuffix()
        {
            var shortSuffix = new Border { Width = 20, Height = 10 };
            var shortLeft = SuffixLeft(Show("A", shortSuffix), shortSuffix);

            var longSuffix = new Border { Width = 20, Height = 10 };
            var longLeft = SuffixLeft(Show("A much longer name", longSuffix), longSuffix);

            Assert.True(longLeft > shortLeft);
        }

        [AvaloniaFact]
        public void Show_TheSuffix_IsCenteredInTheHeightOfTheBox()
        {
            var suffix = new Border { Width = 20, Height = 10 };
            var shown = Show(suffix: suffix);

            var middle = suffix.TranslatePoint(new Point(0, suffix.Bounds.Height / 2), shown.Box)!.Value.Y;

            Assert.Equal(shown.Box.Bounds.Height / 2, middle, 1);
        }

        [AvaloniaFact]
        public void Edit_TheText_HidesTheSuffix()
        {
            var suffix = new Border { Width = 20, Height = 10 };
            var shown = Show(suffix: suffix);
            Assert.True(suffix.IsEffectivelyVisible);

            shown.Box.IsEditing = true;
            Dispatcher.UIThread.RunJobs();

            Assert.False(suffix.IsEffectivelyVisible);
        }

        [AvaloniaFact]
        public void Show_ABoxWithoutASuffix_KeepsTheTextPlace()
        {
            var withSuffix = Show(suffix: new Border { Width = 20, Height = 10 }).Text.Bounds;

            var withoutSuffix = Show().Text.Bounds;

            Assert.Equal(withoutSuffix, withSuffix);
        }
    }
}
