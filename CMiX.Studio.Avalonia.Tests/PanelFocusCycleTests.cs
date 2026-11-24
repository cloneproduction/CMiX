using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.ListTreeTestScreen;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // Ctrl+Tab, Ctrl+Shift+Tab and Ctrl+1 to Ctrl+4 move the keyboard focus between the side panels. Each panel keeps its last
    // focused control. The accent line of the panel follows the focus.
    public class PanelFocusCycleTests : IDisposable
    {
        private sealed record Screen(Views.MainWindow Window, MainViewModel ViewModel, PanelSide Left, PanelSide Right)
        {
            public ProjectTree Tree => ViewModel.ProjectTree;
            public ListTreeProject View => Left.GetVisualDescendants().OfType<ListTreeProject>().Single();
            public TreeNode LayerNode => Tree.CompositionNodes()[0].Children[0].Children[0];
            public RowListTree LayerItem => View.Rows.RowOf(LayerNode)!;
            public ToggleButton RightPicker => Right.FindControl<ToggleButton>("pickerToggle")!;
            public object? Focused => Window.FocusManager!.GetFocusedElement();
        }

        private Screen? screen;

        public void Dispose()
        {
            screen?.Window.Close();
            Dispatcher.UIThread.RunJobs();
        }

        // The tree on the left, the tabs on the right. One composition with one layer, and the layer is selected. With focusTree,
        // the focus is on the layer row.
        private Screen Show(bool focusTree = true)
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.ManagerData.Items[0];
            composition.LayerManager.AddItem(typeof(Layer));
            TestServiceProviderFactory.Pump();
            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            TestServiceProviderFactory.Pump();

            var hosts = window.GetVisualDescendants().OfType<PanelSide>()
                .OrderBy(host => host.TranslatePoint(default, window)!.Value.X).ToList();
            screen = new Screen(window, viewModel, hosts[0], hosts[1]);
            screen.Tree.SelectedNode = screen.LayerNode;
            Settle(window);
            if (focusTree)
            {
                screen.LayerItem.Focus();
                Settle(window);
            }

            return screen;
        }

        private static void Press(Screen screen, PhysicalKey key, RawInputModifiers modifiers = RawInputModifiers.None)
        {
            screen.Window.KeyPressQwerty(key, modifiers);
            screen.Window.KeyReleaseQwerty(key, modifiers);
            Settle(screen.Window);
        }

        private static bool IsIn(PanelSide panel, object? focused) =>
            focused is Visual visual && panel.IsVisualAncestorOf(visual);

        private static bool IsLineOn(PanelSide panel) => panel.FindControl<Border>("focusLine")!.IsEffectivelyVisible;

        [AvaloniaFact]
        public void Panels_OfTheMainWindow_AreTheTwoSidePanelsFromLeftToRight()
        {
            var screen = Show();

            Assert.Equal(new[] { screen.Left, screen.Right }, screen.ViewModel.PanelFocus.Panels());
        }

        [AvaloniaFact]
        public void Press_CtrlTab_MovesTheFocusToTheNextPanelAndBack()
        {
            var screen = Show();

            Press(screen, PhysicalKey.Tab, RawInputModifiers.Control);

            Assert.True(IsIn(screen.Right, screen.Focused));
            Assert.True(IsLineOn(screen.Right));
            Assert.False(IsLineOn(screen.Left));

            Press(screen, PhysicalKey.Tab, RawInputModifiers.Control);

            Assert.Same(screen.LayerItem, screen.Focused);
            Assert.True(IsLineOn(screen.Left));
            Assert.False(IsLineOn(screen.Right));
        }

        [AvaloniaFact]
        public void Press_CtrlShiftTab_MovesTheFocusToThePreviousPanel()
        {
            var screen = Show();

            // The left panel is the first panel, so the previous panel is the last panel.
            Press(screen, PhysicalKey.Tab, RawInputModifiers.Control | RawInputModifiers.Shift);
            Assert.True(IsIn(screen.Right, screen.Focused));

            Press(screen, PhysicalKey.Tab, RawInputModifiers.Control | RawInputModifiers.Shift);
            Assert.Same(screen.LayerItem, screen.Focused);
        }

        [AvaloniaFact]
        public void Press_CtrlTabTwice_PutsTheFocusBackOnTheLastFocusedControl()
        {
            var screen = Show();
            screen.RightPicker.Focus();
            Settle(screen.Window);

            Press(screen, PhysicalKey.Tab, RawInputModifiers.Control);
            Assert.Same(screen.LayerItem, screen.Focused);

            Press(screen, PhysicalKey.Tab, RawInputModifiers.Control);
            Assert.Same(screen.RightPicker, screen.Focused);
        }

        [AvaloniaFact]
        public void Press_CtrlNumber_JumpsToThatPanel()
        {
            var screen = Show();

            Press(screen, PhysicalKey.Digit2, RawInputModifiers.Control);
            Assert.True(IsIn(screen.Right, screen.Focused));
            Assert.True(IsLineOn(screen.Right));

            Press(screen, PhysicalKey.Digit1, RawInputModifiers.Control);
            Assert.Same(screen.LayerItem, screen.Focused);
            Assert.True(IsLineOn(screen.Left));

            // There are two panels: Ctrl+3 and Ctrl+4 do nothing.
            Press(screen, PhysicalKey.Digit3, RawInputModifiers.Control);
            Assert.Same(screen.LayerItem, screen.Focused);
            Press(screen, PhysicalKey.Digit4, RawInputModifiers.Control);
            Assert.Same(screen.LayerItem, screen.Focused);
        }

        // With no last focused control, the focus goes to the cursor row of the tree.
        [AvaloniaFact]
        public void Press_CtrlOneIntoATreePanelNeverFocused_FocusesTheCursorRow()
        {
            var screen = Show(focusTree: false);
            screen.RightPicker.Focus();
            Settle(screen.Window);

            Press(screen, PhysicalKey.Digit1, RawInputModifiers.Control);

            Assert.Same(screen.LayerItem, screen.Focused);
        }

        // Ctrl+Tab in a text box moves the focus to the next panel. It types nothing.
        [AvaloniaFact]
        public void Press_CtrlTabInATextBox_MovesTheFocusAndKeepsTheText()
        {
            var screen = Show();
            var box = screen.Left.GetVisualDescendants().OfType<TextBox>().First();
            box.Focus();
            Settle(screen.Window);
            var text = box.Text;

            Press(screen, PhysicalKey.Tab, RawInputModifiers.Control);

            Assert.True(IsIn(screen.Right, screen.Focused));
            Assert.Equal(text, box.Text);

            // The text box is the last focused control of the left panel.
            Press(screen, PhysicalKey.Tab, RawInputModifiers.Control);
            Assert.Same(box, screen.Focused);
        }

        [AvaloniaFact]
        public void FocusTreeCommand_FocusInOtherPanel_FocusesTheCursorRow()
        {
            var screen = Show(focusTree: false);
            screen.RightPicker.Focus();
            Settle(screen.Window);

            screen.ViewModel.PanelFocus.FocusTreeCommand.Execute(null);
            Settle(screen.Window);

            Assert.Same(screen.LayerItem, screen.Focused);
            Assert.True(IsLineOn(screen.Left));
        }

        // Tree.Focus has no default gesture. A gesture that the user sets works.
        [AvaloniaFact]
        public void Press_GestureSetForTreeFocus_FocusesTheCursorRow()
        {
            var screen = Show(focusTree: false);
            screen.ViewModel.Shortcuts.Set("Tree.Focus", new KeyGesture(Key.T, KeyModifiers.Control));
            screen.RightPicker.Focus();
            Settle(screen.Window);

            Press(screen, PhysicalKey.T, RawInputModifiers.Control);

            Assert.Same(screen.LayerItem, screen.Focused);
        }

        [AvaloniaFact]
        public void Press_EscapeInOtherPanel_FocusesTheCursorRow()
        {
            var screen = Show();
            screen.RightPicker.Focus();
            Settle(screen.Window);

            Press(screen, PhysicalKey.Escape);

            Assert.Same(screen.LayerItem, screen.Focused);
        }

        // The text box takes the first Escape. The second Escape goes back to the tree.
        [AvaloniaFact]
        public void Press_EscapeInATextBox_LeavesTheTextBoxOnly()
        {
            var screen = Show();
            var editable = new CMiX.UI.Controls.String.TextBoxEditable { Text = "name" };
            AddControl(screen.Right, editable);
            editable.IsEditing = true;
            Settle(screen.Window);
            Assert.IsType<TextBox>(screen.Focused);

            Press(screen, PhysicalKey.Escape);

            Assert.False(editable.IsEditing);
            Assert.False(IsIn(screen.Left, screen.Focused));

            Press(screen, PhysicalKey.Escape);

            Assert.Same(screen.LayerItem, screen.Focused);
        }

        // A plain text box keeps the Escape.
        [AvaloniaFact]
        public void Press_EscapeInAPlainTextBox_KeepsTheFocusInTheTextBox()
        {
            var screen = Show();
            var box = AddTextBox(screen.Right);
            box.Focus();
            Settle(screen.Window);

            Press(screen, PhysicalKey.Escape);

            Assert.Same(box, screen.Focused);
        }

        // An editor that cancels on Escape and does not mark the key as handled keeps the Escape too.
        [AvaloniaFact]
        public void Press_EscapeInATextBoxThatLeavesTheFocus_DoesNotFocusTheTree()
        {
            var screen = Show();
            var box = AddTextBox(screen.Right);
            box.KeyDown += (_, e) =>
            {
                if (e.Key == Key.Escape)
                    screen.RightPicker.Focus();
            };
            box.Focus();
            Settle(screen.Window);

            Press(screen, PhysicalKey.Escape);

            Assert.Same(screen.RightPicker, screen.Focused);
        }

        [AvaloniaFact]
        public void Press_EscapeHandledInOtherPanel_KeepsTheFocus()
        {
            var screen = Show();
            screen.RightPicker.KeyDown += (_, e) => e.Handled = e.Key == Key.Escape;
            screen.RightPicker.Focus();
            Settle(screen.Window);

            Press(screen, PhysicalKey.Escape);

            Assert.Same(screen.RightPicker, screen.Focused);
        }

        [AvaloniaFact]
        public void Press_EscapeInTheTree_KeepsTheCursorRow()
        {
            var screen = Show();
            var cursor = screen.Tree.CursorNode;

            Press(screen, PhysicalKey.Escape);

            Assert.Same(screen.LayerItem, screen.Focused);
            Assert.Same(cursor, screen.Tree.CursorNode);
        }

        // A text box at the end of the content of the panel, for the tests that need one.
        private static TextBox AddTextBox(PanelSide panel) => AddControl(panel, new TextBox { Text = "text" });

        // Adds the control to the first visible panel of the side panel.
        private static T AddControl<T>(PanelSide panel, T control) where T : Control
        {
            var host = panel.GetVisualDescendants().OfType<Panel>().First(found => found.IsEffectivelyVisible && found.Children.Count > 0 && found is StackPanel or DockPanel or Grid);
            host.Children.Add(control);
            Settle((Window)TopLevel.GetTopLevel(panel)!);
            return control;
        }

        [AvaloniaFact]
        public void Wheel_OverOtherPanel_KeepsTheFocus()
        {
            var screen = Show();
            var point = screen.Right.TranslatePoint(new Point(screen.Right.Bounds.Width / 2, screen.Right.Bounds.Height / 2), screen.Window)!.Value;

            screen.Window.MouseMove(point);
            screen.Window.MouseWheel(point, new Vector(0, -1));
            Settle(screen.Window);

            Assert.Same(screen.LayerItem, screen.Focused);
        }

        // A click on a part of the other panel that cannot take the focus moves the focus to that panel.
        [AvaloniaFact]
        public void Click_PlainPartOfOtherPanel_MovesTheFocusToThatPanel()
        {
            var screen = Show();
            var bar = screen.Right.GetVisualDescendants().OfType<Border>()
                .First(border => border.IsEffectivelyVisible && border.Bounds.Width > 20 && border.Bounds.Height > 10 && !border.GetVisualDescendants().OfType<InputElement>().Any(e => e.Focusable));
            var point = bar.TranslatePoint(new Point(2, 2), screen.Window)!.Value;

            screen.Window.MouseDown(point, MouseButton.Left);
            screen.Window.MouseUp(point, MouseButton.Left);
            Settle(screen.Window);

            Assert.True(IsIn(screen.Right, screen.Focused));
        }

        // A toggle acts on its row only. The focus, the cursor and the selection stay on the layer row.
        [AvaloniaFact]
        public void Click_TheToggleOfARowWithoutTheFocus_KeepsTheCursorAndTheSelection()
        {
            var screen = Show();
            screen.Tree.CursorNode = screen.LayerNode;
            Settle(screen.Window);
            var other = screen.Tree.CompositionNodes()[0];
            var toggle = screen.View.Rows.RowOf(other)!.Toggles.GetVisualDescendants().OfType<ToggleButton>()
                .First(button => button.Name == "visibilityToggle" && button.IsEffectivelyVisible);
            var before = toggle.IsChecked;
            var point = toggle.TranslatePoint(new Point(toggle.Bounds.Width / 2, toggle.Bounds.Height / 2), screen.Window)!.Value;

            screen.Window.MouseDown(point, MouseButton.Left);
            screen.Window.MouseUp(point, MouseButton.Left);
            Settle(screen.Window);

            Assert.NotEqual(before, toggle.IsChecked);
            Assert.Same(screen.LayerNode, screen.Tree.CursorNode);
            Assert.Same(screen.LayerNode, screen.Tree.SelectedNode);
            Assert.Same(screen.LayerItem, screen.Focused);
        }

        // A click on a toggle of the tree, with the focus in the other panel, keeps the cursor and the selection.
        [AvaloniaFact]
        public void Click_TheToggleOfARowFromOtherPanel_KeepsTheCursorAndTheSelection()
        {
            var screen = Show();
            screen.Tree.CursorNode = screen.LayerNode;
            screen.RightPicker.Focus();
            Settle(screen.Window);
            var other = screen.Tree.CompositionNodes()[0];
            var toggle = screen.View.Rows.RowOf(other)!.Toggles.GetVisualDescendants().OfType<ToggleButton>()
                .First(button => button.Name == "visibilityToggle" && button.IsEffectivelyVisible);
            var point = toggle.TranslatePoint(new Point(toggle.Bounds.Width / 2, toggle.Bounds.Height / 2), screen.Window)!.Value;

            screen.Window.MouseDown(point, MouseButton.Left);
            screen.Window.MouseUp(point, MouseButton.Left);
            Settle(screen.Window);

            Assert.Same(screen.LayerNode, screen.Tree.CursorNode);
            Assert.Same(screen.LayerNode, screen.Tree.SelectedNode);
        }
    }
}
