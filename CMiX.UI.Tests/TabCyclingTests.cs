using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.Collection;
using CMiX.UI.Controls.Number;
using CMiX.UI.Controls.Vector;
using CMiX.UI.Panels;
using Xunit;

namespace CMiX.UI.Tests
{
    // Tab and Shift+Tab in an edit box move the edit mode from one value editor to the next one.
    // The loop is the nearest parent with EditCycle.IsLoop, or the window.
    public class TabCyclingTests
    {
        private static DragValue Drag(double value, bool readOnly = false) =>
            new() { Value = value, Minimum = 0, Maximum = 100, IsReadOnly = readOnly };

        private static Window Show(Control content)
        {
            var window = new Window { Content = content, Width = 400, Height = 400, FontSize = 12.5 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return window;
        }

        private static StackPanel Stack(params Control[] children)
        {
            var panel = new StackPanel();
            foreach (var child in children)
                panel.Children.Add(child);

            return panel;
        }

        private static StackPanel Loop(params Control[] children)
        {
            var panel = Stack(children);
            EditCycle.SetIsLoop(panel, true);
            return panel;
        }

        private static void Pump()
        {
            for (var i = 0; i < 10; i++)
                Dispatcher.UIThread.RunJobs();
        }

        private static void StartEdit(Control editor)
        {
            ((IEditableValue)editor).IsEditing = true;
            Pump();
        }

        private static void Press(Window window, Key key, bool shift = false)
        {
            window.KeyPress(key, shift ? RawInputModifiers.Shift : RawInputModifiers.None, PhysicalKey.None, null);
            Pump();
        }

        private static void Tab(Window window, bool shift = false) => Press(window, Key.Tab, shift);

        private static bool[] Editing(params Control[] editors) =>
            editors.Select(editor => ((IEditableValue)editor).IsEditing).ToArray();

        private static TextBox EditBox(Control editor) =>
            editor.GetVisualDescendants().OfType<TextBox>().First(box => box.Name == "PART_TextBox");

        [AvaloniaFact]
        public void Tab_MovesTheEditMode_ToTheNextEditor_AndWrapsAtTheEnd()
        {
            var (a, b, c) = (Drag(1), Drag(2), Drag(3));
            var window = Show(Loop(a, b, c));
            StartEdit(a);

            Tab(window);
            Assert.Equal(new[] { false, true, false }, Editing(a, b, c));
            Tab(window);
            Assert.Equal(new[] { false, false, true }, Editing(a, b, c));
            Tab(window);
            Assert.Equal(new[] { true, false, false }, Editing(a, b, c));
        }

        [AvaloniaFact]
        public void ShiftTab_MovesTheEditMode_Backwards_AndWrapsAtTheStart()
        {
            var (a, b, c) = (Drag(1), Drag(2), Drag(3));
            var window = Show(Loop(a, b, c));
            StartEdit(a);

            Tab(window, shift: true);
            Assert.Equal(new[] { false, false, true }, Editing(a, b, c));
            Tab(window, shift: true);
            Assert.Equal(new[] { false, true, false }, Editing(a, b, c));
            Tab(window, shift: true);
            Assert.Equal(new[] { true, false, false }, Editing(a, b, c));
        }

        [AvaloniaFact]
        public void Tab_KeepsTheValueThatWasTyped()
        {
            var (a, b) = (Drag(1), Drag(2));
            var window = Show(Loop(a, b));
            StartEdit(a);

            EditBox(a).Text = "7";
            Tab(window);

            Assert.Equal(7, a.Value);
            Assert.True(b.IsEditing);
        }

        [AvaloniaFact]
        public void Tab_SkipsAValueThatCannotBeEdited_AHiddenOne_AndADisabledOne()
        {
            var (a, modulated, hidden, disabled, last) = (Drag(1), Drag(2, readOnly: true), Drag(3), Drag(4), Drag(5));
            hidden.IsVisible = false;
            disabled.IsEnabled = false;
            var window = Show(Loop(a, modulated, hidden, disabled, last));
            StartEdit(a);

            Tab(window);

            Assert.Equal(new[] { false, false, false, false, true }, Editing(a, modulated, hidden, disabled, last));
        }

        [AvaloniaFact]
        public void Tab_IgnoresControlsThatAreNotEditors()
        {
            var (a, b) = (Drag(1), Drag(2));
            var window = Show(Loop(a, new Button { Content = "button" }, new ToggleButton { Content = "toggle" }, new TextBox(), b));
            StartEdit(a);

            Tab(window);

            Assert.True(b.IsEditing);
        }

        [AvaloniaFact]
        public void Tab_CyclesThroughSlidersAndDragValuesTogether()
        {
            var a = Drag(1);
            var slider = new SliderCMiX { Minimum = 0, Maximum = 10, Value = 5 };
            var c = Drag(3);
            var window = Show(Loop(a, slider, c));
            StartEdit(a);

            Tab(window);
            Assert.Equal(new[] { false, true, false }, Editing(a, slider, c));
            Tab(window);
            Assert.Equal(new[] { false, false, true }, Editing(a, slider, c));
            Tab(window, shift: true);
            Assert.Equal(new[] { false, true, false }, Editing(a, slider, c));
        }

        [AvaloniaFact]
        public void Tab_InAnIntegerValue_GoesOnToTheNextEditor()
        {
            var a = new IntegerValue { Value = 1, Minimum = 0, Maximum = 10 };
            var b = Drag(2);
            var window = Show(Loop(a, b));
            StartEdit(a);

            Tab(window);

            Assert.False(a.IsEditing);
            Assert.True(b.IsEditing);
        }

        [AvaloniaFact]
        public void TheNearestLoopWins_TabWrapsInsideIt()
        {
            var (a, b, c, d) = (Drag(1), Drag(2), Drag(3), Drag(4));
            var window = Show(Stack(Loop(a, b), Loop(c, d)));
            StartEdit(b);

            Tab(window);

            Assert.Equal(new[] { true, false, false, false }, Editing(a, b, c, d));
        }

        [AvaloniaFact]
        public void WithoutALoop_TabOnlyCommitsTheValue()
        {
            var (a, b) = (Drag(1), Drag(2));
            var window = Show(Stack(a, b));
            StartEdit(a);

            EditBox(a).Text = "4";
            Tab(window);

            Assert.Equal(new[] { false, false }, Editing(a, b));
            Assert.Equal(4, a.Value);
        }

        [AvaloniaFact]
        public void ALoop_DoesNotReachTheEditorsOfALoopInsideIt()
        {
            var (outer, inner) = (Drag(1), Drag(2));
            var other = Drag(3);
            var window = Show(Loop(outer, Loop(inner), other));
            StartEdit(outer);

            Tab(window);
            Assert.Equal(new[] { false, false, true }, Editing(outer, inner, other));
            Tab(window);
            Assert.Equal(new[] { true, false, false }, Editing(outer, inner, other));
        }

        [AvaloniaFact]
        public void TheBodyOfAnExpander_IsALoop()
        {
            var (a, b) = (Drag(1), Drag(2));
            var outside = Drag(3);
            var expander = new ExpanderIndented { Header = "Transform", IsExpanded = true, Content = Stack(a, b) };
            var window = Show(Loop(expander, outside));
            StartEdit(b);

            Tab(window);

            Assert.Equal(new[] { true, false, false }, Editing(a, b, outside));
        }

        [AvaloniaFact]
        public void TheTwoValuesOfAVector_CycleOnlyBetweenThemselves()
        {
            var resolution = new Integer2 { Caption = "Resolution" };
            var other = Drag(3);
            var window = Show(Stack(new TabControl { Items = { new TabItem { Header = "One", Content = resolution } } }, other));
            var (x, y) = (Values(resolution)[0], Values(resolution)[1]);
            StartEdit(x);

            Tab(window);
            Assert.Equal(new[] { false, true, false }, Editing(x, y, other));
            Tab(window);
            Assert.Equal(new[] { true, false, false }, Editing(x, y, other));
        }

        private static List<Control> Values(Control root) =>
            root.GetVisualDescendants().OfType<IEditableValue>().Cast<Control>().ToList();

        [AvaloniaFact]
        public void ASingleEditor_Tab_CommitsAndEndsTheEdit()
        {
            var a = Drag(1);
            var window = Show(Loop(a));
            StartEdit(a);

            EditBox(a).Text = "9";
            Tab(window);

            Assert.False(a.IsEditing);
            Assert.Equal(9, a.Value);
        }

        [AvaloniaFact]
        public void Escape_StillCancelsTheEdit_AfterATab()
        {
            var (a, b) = (Drag(1), Drag(2));
            var window = Show(Loop(a, b));
            StartEdit(a);
            Tab(window);

            EditBox(b).Text = "50";
            Press(window, Key.Escape);

            Assert.False(b.IsEditing);
            Assert.Equal(2, b.Value);
        }

        [AvaloniaFact]
        public void TheBodyOfAPanelItem_IsALoop_TabNeverLeavesIt()
        {
            var (a, b) = (Drag(1), Drag(2));
            var other = Drag(3);
            var item = new PanelItem { Title = "Item", IsExpanded = true, Content = Stack(a, b) };
            var window = Show(Stack(item, other));
            StartEdit(a);

            Tab(window);
            Assert.Equal(new[] { false, true, false }, Editing(a, b, other));
            Tab(window);
            Assert.Equal(new[] { true, false, false }, Editing(a, b, other));
            Tab(window, shift: true);
            Assert.Equal(new[] { false, true, false }, Editing(a, b, other));
        }

        [AvaloniaFact]
        public void TheContentOfATabPage_IsALoop()
        {
            var (a, b) = (Drag(1), Drag(2));
            var outside = Drag(3);
            var tabs = new TabControl { Items = { new TabItem { Header = "One", Content = Stack(a, b) } } };
            var window = Show(Stack(tabs, outside));
            StartEdit(b);

            Tab(window);

            Assert.Equal(new[] { true, false, false }, Editing(a, b, outside));
        }

        [AvaloniaFact]
        public void Tab_WhenNothingEdits_DoesNotStartAnEdit()
        {
            var (a, b) = (Drag(1), Drag(2));
            var window = Show(Loop(a, b));

            Tab(window);

            Assert.Equal(new[] { false, false }, Editing(a, b));
        }
    }
}
