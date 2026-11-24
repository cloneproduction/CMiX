using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Composition = CMiX.Core.Compositing.Composition;

namespace CMiX.Studio.Avalonia.Tests
{
    // The search field of the tree: always shown in the bar of the panel, at the right, as wide as TreeSearchWidth. Tree.Search gives it
    // the focus. The number of matches and the clear button show while it holds text. Escape clears the text, and Escape in an empty field
    // gives the focus back to the rows. Enter and Shift+Enter move the cursor from match to match, and Down gives the focus to the cursor
    // row. There is no search during a drag.
    public class TextBoxSearchTreeTests : IDisposable
    {
        private sealed record Screen(Window Window, TextBoxSearchTree Box, ListTreeProject View)
        {
            // The text box inside the search field.
            public TextBox Input => Box.Field.GetVisualDescendants().OfType<TextBox>().First();
        }

        private Window? window;
        private readonly TestWindows windows = new();

        public void Dispose()
        {
            window?.Close();
            windows.Dispose();
        }

        // A composition with three layers, the search field over the tree, as in the bar of the panel.
        private (Screen Screen, Scene Scene) Show()
        {
            var scene = CreateScene([3]);
            var (shown, box) = ListTreeTestScreen.ShowWithSearch(scene.Tree, 400, 700);
            window = shown.Window;
            return (new Screen(shown.Window, box, shown.View), scene);
        }


        // Focuses the field and types a text.
        private static void Search(Screen screen, string text)
        {
            screen.Box.Field.FocusInput();
            screen.Window.Settle();
            screen.Box.Field.Text = text;
            screen.Window.Settle();
        }

        [AvaloniaFact]
        public void Show_TheTree_ShowsTheFieldWithItsWidthAndNoClear()
        {
            var (screen, _) = Show();
            var width = Convert.ToDouble(Application.Current!.FindResource("TreeSearchWidth"));

            Assert.True(screen.Box.Field.IsEffectivelyVisible);
            Assert.Equal(width, screen.Box.Field.Bounds.Width, 0.51);
            Assert.Equal(width, screen.Box.Bounds.Width, 0.51);
            Assert.False(screen.Box.Clear.IsVisible);
        }

        // The field is all there is: no number of matches beside it, and the clear button inside it.
        [AvaloniaFact]
        public void Type_AText_SetsTheSearchAndShowsTheClearInsideTheField()
        {
            var (screen, scene) = Show();

            Search(screen, "Layer");

            Assert.Equal("Layer", scene.Tree.SearchText);
            Assert.True(scene.Tree.SearchMatchCount > 0);
            Assert.True(screen.Box.Clear.IsVisible);
            Assert.Empty(screen.Box.GetVisualDescendants().OfType<TextBlock>().Where(text => text.Text == scene.Tree.SearchMatchCount.ToString()));
            var field = new Rect(screen.Box.Field.TranslatePoint(default, screen.Window)!.Value, screen.Box.Field.Bounds.Size);
            var clear = new Rect(screen.Box.Clear.TranslatePoint(default, screen.Window)!.Value, screen.Box.Clear.Bounds.Size);
            Assert.InRange(clear.Left, field.Left, field.Right);
            Assert.InRange(clear.Right, field.Left, field.Right + 0.5);
            Assert.Equal(field.Center.Y, clear.Center.Y, 0.51);
        }

        [AvaloniaFact]
        public void FocusSearch_WithAText_GivesTheFieldTheFocusAndSelectsTheText()
        {
            var (screen, scene) = Show();
            scene.Tree.SearchText = "Layer";
            screen.Window.Settle();

            scene.Tree.FocusSearch();
            screen.Window.Settle();

            Assert.True(screen.Input.IsFocused);
            Assert.Equal(0, screen.Input.SelectionStart);
            Assert.Equal("Layer".Length, screen.Input.SelectionEnd);
        }

        [AvaloniaFact]
        public void Escape_InTheField_ClearsTheTextThenGivesTheFocusBackToTheTree()
        {
            var (screen, scene) = Show();
            Search(screen, "Layer");

            screen.Window.Press(PhysicalKey.Escape);

            Assert.Equal(string.Empty, scene.Tree.SearchText);
            Assert.True(screen.Input.IsFocused);
            Assert.False(screen.Box.Clear.IsVisible);

            screen.Window.Press(PhysicalKey.Escape);

            Assert.False(screen.Input.IsFocused);
            Assert.True(screen.View.IsKeyboardFocusWithin);
            Assert.True(screen.Box.Field.IsEffectivelyVisible);
        }

        // The composition row is selected and does not match "Layer", so the cursor starts on a row that is not a match.
        [AvaloniaFact]
        public void Enter_InTheField_MovesTheCursorThroughTheMatchesAndGoesRound()
        {
            var (screen, scene) = Show();
            scene.Tree.SelectedNode = scene.NodeA;
            Search(screen, "Layer");
            var matches = scene.Tree.SearchMatches.ToList();
            Assert.True(matches.Count >= 3);
            Assert.DoesNotContain(scene.NodeA, matches);

            screen.Window.Press(PhysicalKey.Enter);
            Assert.Same(matches[0], scene.Tree.CursorNode);

            for (var i = 1; i < matches.Count; i++)
                screen.Window.Press(PhysicalKey.Enter);
            Assert.Same(matches[^1], scene.Tree.CursorNode);

            screen.Window.Press(PhysicalKey.Enter);
            Assert.Same(matches[0], scene.Tree.CursorNode);
        }

        [AvaloniaFact]
        public void ShiftEnter_InTheField_MovesTheCursorToTheMatchBefore()
        {
            var (screen, scene) = Show();
            scene.Tree.SelectedNode = scene.NodeA;
            Search(screen, "Layer");
            var matches = scene.Tree.SearchMatches.ToList();

            screen.Window.Press(PhysicalKey.Enter, RawInputModifiers.Shift);
            Assert.Same(matches[^1], scene.Tree.CursorNode);

            screen.Window.Press(PhysicalKey.Enter, RawInputModifiers.Shift);
            Assert.Same(matches[^2], scene.Tree.CursorNode);
        }

        // D4: the search moves the cursor only. The selection, and so the Inspector, does not change, and no undo step is recorded.
        [AvaloniaTheory]
        [InlineData(RawInputModifiers.None)]
        [InlineData(RawInputModifiers.Shift)]
        public void Enter_InTheField_DoesNotChangeTheSelectionAndKeepsTheFocusInTheField(RawInputModifiers modifiers)
        {
            var (screen, scene) = Show();
            scene.ClearUndo();
            scene.Tree.SelectedNode = scene.NodeA;
            Search(screen, "Layer");
            var before = scene.Undo.CanUndo;

            screen.Window.Press(PhysicalKey.Enter, modifiers);
            screen.Window.Press(PhysicalKey.Enter, modifiers);

            Assert.NotSame(scene.NodeA, scene.Tree.CursorNode);
            Assert.Contains(scene.Tree.CursorNode!, scene.Tree.SearchMatches);
            Assert.Same(scene.NodeA, scene.Tree.SelectedNode);
            Assert.Equal(before, scene.Undo.CanUndo);
            Assert.True(screen.Input.IsFocused);
            Assert.Equal("Layer", scene.Tree.SearchText);
        }

        [AvaloniaFact]
        public void Enter_OnAMatchUnderACollapsedRow_OpensItsParentsAndMovesTheCursorToIt()
        {
            var (screen, scene) = Show();
            scene.Tree.SelectedNode = null;
            var layer = scene.LayerNodeOfA(2);
            layer.Parent!.IsExpanded = false;
            screen.Window.Settle();
            Search(screen, layer.Header);
            Assert.Contains(layer, scene.Tree.SearchMatches);

            while (!ReferenceEquals(scene.Tree.CursorNode, layer))
                screen.Window.Press(PhysicalKey.Enter);

            Assert.True(layer.Parent!.IsExpanded);
            Assert.NotNull(screen.View.Rows.RowOf(layer));
            Assert.Null(scene.Tree.SelectedNode);
        }

        [AvaloniaTheory]
        [InlineData(RawInputModifiers.None)]
        [InlineData(RawInputModifiers.Shift)]
        public void F3_InTheField_DoesNothing(RawInputModifiers modifiers)
        {
            var (screen, scene) = Show();
            scene.Tree.SelectedNode = scene.NodeA;
            Search(screen, "Layer");

            screen.Window.Press(PhysicalKey.F3, modifiers);

            Assert.Same(scene.NodeA, scene.Tree.CursorNode);
            Assert.Same(scene.NodeA, scene.Tree.SelectedNode);
        }

        // Down puts the focus on the row of the match that Enter found. Enter there selects that row.
        [AvaloniaFact]
        public void Down_InTheFieldAfterEnter_FocusesTheTreeOnTheMatch()
        {
            var (screen, scene) = Show();
            scene.Tree.SelectedNode = scene.NodeA;
            Search(screen, "Layer");
            screen.Window.Press(PhysicalKey.Enter);
            screen.Window.Press(PhysicalKey.Enter);
            var match = scene.Tree.CursorNode!;

            screen.Window.Press(PhysicalKey.ArrowDown);

            Assert.False(screen.Input.IsFocused);
            Assert.Same(screen.View.Rows.RowOf(match), screen.Window.FocusManager!.GetFocusedElement());
            Assert.Same(match, scene.Tree.CursorNode);
            Assert.Same(scene.NodeA, scene.Tree.SelectedNode);

            scene.Tree.OpenSelectedCommand.Execute(null);
            Assert.Same(match, scene.Tree.SelectedNode);
        }

        [AvaloniaFact]
        public void Click_TheClearButton_EmptiesTheTextAndKeepsTheFocusInTheField()
        {
            var (screen, scene) = Show();
            Search(screen, "Layer");

            screen.Window.Click(screen.Box.Clear);
            screen.Window.Settle();

            Assert.Equal(string.Empty, scene.Tree.SearchText);
            Assert.True(screen.Input.IsFocused);
        }

        // The clear button ends the search as Escape does: the rows that the search opened close again.
        [AvaloniaFact]
        public void Click_TheClearButton_ClosesTheRowsThatTheSearchOpened()
        {
            var (screen, scene) = Show();
            scene.Tree.SelectedNode = null;
            var layers = scene.LayerNodeOfA(0).Parent!;
            layers.IsExpanded = false;
            screen.Window.Settle();
            Search(screen, scene.LayerNodeOfA(2).Header);
            Assert.True(layers.IsExpanded);

            screen.Window.Click(screen.Box.Clear);
            screen.Window.Settle();

            Assert.False(layers.IsExpanded);
        }

        // With a selected match, the clear button and Escape end the search, the parents of the match stay open, and its row is in view.
        [AvaloniaTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void Clear_WithASelectedMatch_KeepsItsParentsOpenAndItsRowInView(bool byButton)
        {
            var (screen, scene) = Show();
            var layers = scene.LayerNodeOfA(0).Parent!;
            var layer = scene.LayerNodeOfA(2);
            layers.IsExpanded = false;
            screen.Window.Settle();
            Search(screen, layer.Header);
            scene.Tree.SelectedNode = layer;
            screen.Window.Settle();
            screen.Box.Field.FocusInput();
            screen.Window.Settle();

            if (byButton)
                screen.Window.Click(screen.Box.Clear);
            else
                screen.Window.Press(PhysicalKey.Escape);
            screen.Window.Settle();

            Assert.Equal(string.Empty, scene.Tree.SearchText);
            Assert.True(layers.IsExpanded);
            var row = screen.View.Rows.RowOf(layer)!;
            var scroller = screen.View.Scroller;
            var top = row.TranslatePoint(new Point(0, 0), scroller)!.Value.Y;
            Assert.InRange(top, 0, scroller.Viewport.Height - row.Bounds.Height);
        }

        // The text stays when the focus goes back to the tree.
        [AvaloniaFact]
        public void Focus_TheTreeWithATextInTheField_KeepsTheSearch()
        {
            var (screen, scene) = Show();
            Search(screen, "Layer");

            screen.View.Rows.RowOf(scene.LayerNodeOfA(0))!.Focus();
            screen.Window.Settle();

            Assert.False(screen.Input.IsFocused);
            Assert.Equal("Layer", scene.Tree.SearchText);
            Assert.True(screen.Box.Clear.IsVisible);
        }

        // ---------------------------------------------------------------- no search during a drag

        [AvaloniaFact]
        public void BeginDrag_WithASearch_TurnsTheCommandOffAndTheFieldTakesNoText()
        {
            var (screen, scene) = Show();
            Search(screen, "Layer");
            var changes = 0;
            scene.Tree.FocusSearchCommand.CanExecuteChanged += (_, _) => changes++;

            scene.Tree.BeginDrag(scene.LayerNodeOfA(0));
            screen.Window.Settle();

            Assert.True(changes > 0);
            Assert.False(scene.Tree.FocusSearchCommand.CanExecute(null));
            Assert.True(screen.Input.IsReadOnly);
            Assert.False(screen.Box.Clear.IsEffectivelyEnabled);
            screen.Input.Focus();
            screen.Window.KeyTextInput("x");
            screen.Window.Settle();
            Assert.Equal("Layer", scene.Tree.SearchText);
            scene.Tree.EndDrag();
        }

        [AvaloniaFact]
        public void Enter_InTheFieldDuringADrag_DoesNotMoveTheCursor()
        {
            var (screen, scene) = Show();
            scene.Tree.SelectedNode = scene.NodeA;
            Search(screen, "Layer");
            scene.Tree.BeginDrag(scene.LayerNodeOfA(0));
            screen.Window.Settle();

            screen.Window.Press(PhysicalKey.Enter);
            scene.Tree.MoveCursorToNextMatch();

            Assert.Same(scene.NodeA, scene.Tree.CursorNode);
            Assert.Same(scene.NodeA, scene.Tree.SelectedNode);
            scene.Tree.EndDrag();
        }

        [AvaloniaFact]
        public void FocusSearch_DuringADrag_LeavesTheFocusWhereItIs()
        {
            var (screen, scene) = Show();
            scene.Tree.BeginDrag(scene.LayerNodeOfA(0));
            screen.Window.Settle();

            scene.Tree.FocusSearch();
            screen.Window.Settle();

            Assert.False(screen.Input.IsFocused);
            scene.Tree.EndDrag();
        }

        [AvaloniaFact]
        public void EndDrag_AfterADrag_GivesTheSearchBackWithTheSameText()
        {
            var (screen, scene) = Show();
            Search(screen, "Layer");
            scene.Tree.BeginDrag(scene.LayerNodeOfA(0));
            screen.Window.Settle();

            scene.Tree.EndDrag();
            screen.Window.Settle();

            Assert.Equal("Layer", scene.Tree.SearchText);
            Assert.False(screen.Input.IsReadOnly);
            screen.Input.Focus();
            screen.Input.CaretIndex = screen.Input.Text!.Length;
            screen.Window.KeyTextInput(" 0");
            screen.Window.Settle();
            Assert.Equal("Layer 0", scene.Tree.SearchText);
        }

        // The field has the focus during a pointer drag of a row: Escape ends the drag without a drop, as it does in the tree.
        [AvaloniaFact]
        public void Escape_InTheFieldDuringAPointerDrag_EndsTheDragWithoutADrop()
        {
            var (screen, scene) = Show();
            Search(screen, "Layer");
            var rows = scene.Tree.CompositionNodes()[0].Children[0].Children;
            var before = rows.Select(node => node.Item).ToList();
            RowListTree RowOf(TreeNode node) => screen.View.Rows.RowOf(node)!;
            Point Middle(RowListTree row) => row.TranslatePoint(new Point(row.Bounds.Width / 2, row.Bounds.Height / 2), screen.Window)!.Value;

            TreeDrag.Over(screen.Window, Middle(RowOf(rows[0])), Middle(RowOf(rows[2])));
            Assert.True(screen.View.RowDrag.IsDragging);
            screen.Input.Focus();
            screen.Window.Settle();

            screen.Input.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Escape });
            screen.Window.Settle();

            Assert.False(screen.View.RowDrag.IsDragging);
            Assert.False(scene.Tree.IsDragging);
            Assert.Equal(before, rows.Select(node => node.Item).ToList());
            Assert.Equal("Layer", scene.Tree.SearchText);
        }

        // ---------------------------------------------------------------- the bar of the panel and the shortcut of the main window

        private static (PanelSide Host, TextBoxSearchTree Box) TreeHostOf(Window window) =>
            window.GetVisualDescendants().OfType<PanelSide>()
                .Select(host => (Host: host, Box: host.GetVisualDescendants().OfType<TextBoxSearchTree>().FirstOrDefault()))
                .First(pair => pair.Box != null)!;

        // The field is in the bar of the panel that shows the tree, at the right end, on the same line as the button of the panel choice.
        [AvaloniaFact]
        public void Show_TheMainWindow_PutsTheFieldAtTheRightOfTheBarOfTheTreePanel()
        {
            var (_, window, _) = windows.ShowMainWindow();
            TestServiceProviderFactory.Pump();
            var (host, box) = TreeHostOf(window);
            var bar = host.GetVisualDescendants().OfType<Border>().First(border => border.Name == "bar");
            var picker = host.GetVisualDescendants().OfType<Control>().First(control => control.Name == "pickerToggle");

            var barRight = bar.TranslatePoint(new Point(bar.Bounds.Width - bar.Padding.Right, 0), window)!.Value.X;
            var boxRight = box.TranslatePoint(new Point(box.Bounds.Width, 0), window)!.Value.X;
            var boxMiddle = box.TranslatePoint(new Point(0, box.Bounds.Height / 2), window)!.Value.Y;
            var pickerMiddle = picker.TranslatePoint(new Point(0, picker.Bounds.Height / 2), window)!.Value.Y;

            Assert.Equal(barRight, boxRight, 0.51);
            Assert.Equal(pickerMiddle, boxMiddle, 0.51);
            Assert.True(box.Bounds.Width < bar.Bounds.Width / 2);
        }

        [AvaloniaFact]
        public void Pick_AnotherPanelChoice_TakesTheFieldOutOfTheBar()
        {
            var (_, window, _) = windows.ShowMainWindow();
            TestServiceProviderFactory.Pump();
            var (host, _) = TreeHostOf(window);

            host.Choice = host.Choices!.Cast<PanelChoice>().First(choice => choice is not TreePanelChoice);
            TestServiceProviderFactory.Pump();

            Assert.Empty(host.GetVisualDescendants().OfType<TextBoxSearchTree>());
        }

        [AvaloniaFact]
        public void KeyBindings_OfTheWindow_HoldTheSearchCommandOfTheTreeOnCtrlF()
        {
            var (_, window, viewModel) = windows.ShowMainWindow();

            var binding = window.KeyBindings.Single(candidate => Equals(candidate.Gesture, viewModel.Shortcuts["Tree.Search"]));

            Assert.Equal(new KeyGesture(Key.F, KeyModifiers.Control), viewModel.Shortcuts["Tree.Search"]);
            Assert.Same(viewModel.ProjectTree.FocusSearchCommand, binding.Command);
        }

        [AvaloniaFact]
        public void Press_CtrlFOverTheTree_FocusesTheField()
        {
            var (_, window, viewModel) = windows.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            TestServiceProviderFactory.Pump();
            window.Focus();
            var view = window.GetVisualDescendants().OfType<ListTreeProject>().Single();
            window.MouseMove(view.TranslatePoint(new Point(view.Bounds.Width / 2, view.Bounds.Height / 2), window)!.Value);
            Dispatcher.UIThread.RunJobs();

            window.KeyPressQwerty(PhysicalKey.F, RawInputModifiers.Control);
            TestServiceProviderFactory.Pump();

            var input = TreeHostOf(window).Box.Field.GetVisualDescendants().OfType<TextBox>().First();
            Assert.True(input.IsFocused);
        }

        [AvaloniaFact]
        public void Press_CtrlFOverTheTreeDuringADrag_LeavesTheFieldWithoutTheFocus()
        {
            var (_, window, viewModel) = windows.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            TestServiceProviderFactory.Pump();
            window.Focus();
            var view = window.GetVisualDescendants().OfType<ListTreeProject>().Single();
            window.MouseMove(view.TranslatePoint(new Point(view.Bounds.Width / 2, view.Bounds.Height / 2), window)!.Value);
            Dispatcher.UIThread.RunJobs();
            viewModel.ProjectTree.BeginDrag(viewModel.ProjectTree.CompositionNodes()[0]);

            window.KeyPressQwerty(PhysicalKey.F, RawInputModifiers.Control);
            TestServiceProviderFactory.Pump();

            var input = TreeHostOf(window).Box.Field.GetVisualDescendants().OfType<TextBox>().First();
            Assert.False(input.IsFocused);
            viewModel.ProjectTree.EndDrag();
        }
    }
}
