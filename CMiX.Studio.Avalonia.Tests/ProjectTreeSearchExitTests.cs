using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Panels;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;
using static CMiX.Studio.Avalonia.Tests.Support.ListTreeTestScreen;

namespace CMiX.Studio.Avalonia.Tests
{
    // The way out of the tree search. Text in the field means the filter is on, and the rows stay dimmed until the text is cleared.
    // Escape in the tree clears an active search: the focus and the cursor row stay. Tree.ClearSearch (Alt+F) clears it from anywhere
    // and puts the focus on the cursor row. While the filter is on and the focus is not in the field, the field has the accent border,
    // and with the focus in the tree the hint strip shows the keys of the filter.
    public class ProjectTreeSearchExitTests : IDisposable
    {
        private Views.MainWindow? window;
        private ListTreeProject? view;

        // A grab that a test leaves running would take the input of the next tests, so it ends here first.
        public void Dispose()
        {
            view?.Grab.Detach();
            window?.Close();
        }

        private sealed record Screen(Views.MainWindow Window, MainViewModel ViewModel, ListTreeProject View, TextBoxSearchTree Box)
        {
            public ProjectTree Tree => ViewModel.ProjectTree;
            public TreeNode LayerNode(int index) => Tree.CompositionNodes()[0].Children[0].Children[index];
            public TextBox Input => Box.Field.GetVisualDescendants().OfType<TextBox>().First();
            public IInputElement? Focused => Window.FocusManager!.GetFocusedElement();
        }

        // One composition with three layers. Every row is open. The composition is selected.
        private Screen Show()
        {
            var (_, mainWindow, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            window = mainWindow;
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.ManagerData.Items[0];
            for (var i = 0; i < 3; i++)
                composition.LayerManager.AddItem(typeof(Layer));
            TestServiceProviderFactory.Pump();
            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            TestServiceProviderFactory.Pump();
            var treeView = mainWindow.GetVisualDescendants().OfType<ListTreeProject>().Single();
            view = treeView;
            var box = mainWindow.GetVisualDescendants().OfType<TextBoxSearchTree>().Single();
            var screen = new Screen(mainWindow, viewModel, treeView, box);
            screen.Tree.SelectedNode = screen.Tree.CompositionNodes()[0];
            Settle(mainWindow);
            return screen;
        }

        private static void Press(Screen screen, PhysicalKey key, RawInputModifiers modifiers = RawInputModifiers.None)
        {
            screen.Window.KeyPressQwerty(key, modifiers);
            Settle(screen.Window);
        }

        // Focuses the field and types the name of the second layer: one row matches.
        private static TreeNode SearchOneLayer(Screen screen)
        {
            var layer = screen.LayerNode(1);
            screen.Box.Field.FocusInput();
            Settle(screen.Window);
            screen.Box.Field.Text = layer.Header;
            Settle(screen.Window);
            Assert.Equal(new[] { layer }, screen.Tree.SearchMatches);
            Assert.True(screen.Tree.IsSearchActive);
            return layer;
        }

        private static void Click(Screen screen, TreeNode node)
        {
            var row = screen.View.Rows.RowOf(node)!;
            var point = row.TranslatePoint(new Point(row.Bounds.Width / 2, row.Bounds.Height / 2), screen.Window)!.Value;
            screen.Window.MouseMove(point);
            screen.Window.MouseDown(point, MouseButton.Left);
            screen.Window.MouseUp(point, MouseButton.Left);
            Settle(screen.Window);
        }

        // The search is off: no text, no dimmed row and no dimmed cover.
        private static void AssertNoSearch(Screen screen)
        {
            Assert.Equal(string.Empty, screen.Tree.SearchText);
            Assert.False(screen.Tree.IsSearchActive);
            foreach (var node in All(screen.Tree.Roots))
                Assert.False(node.IsSearchDimmed, $"{node.Header} is dimmed.");
            Assert.All(screen.View.Rows.RealizedRows, row => Assert.DoesNotContain(RowListTree.DimmedClass, row.Classes));
        }

        private static void AssertFocusOnCursorRow(Screen screen)
        {
            Assert.NotNull(screen.Tree.CursorNode);
            Assert.Same(screen.View.Rows.RowOf(screen.Tree.CursorNode!), screen.Focused);
        }

        private static Color BorderColorOf(Screen screen) =>
            ((ISolidColorBrush)screen.Input.GetVisualDescendants().OfType<Border>().First(border => border.Name == "border").BorderBrush!).Color;

        private static Color AccentColor => ((ISolidColorBrush)Application.Current!.FindResource("AccentBrush")!).Color;

        // ---------------------------------------------------------------- Escape in the tree

        [AvaloniaFact]
        public void Escape_InTreeAfterDownFromSearch_ClearsSearchAndDim()
        {
            var screen = Show();
            SearchOneLayer(screen);
            Press(screen, PhysicalKey.ArrowDown);
            Assert.False(screen.Input.IsFocused);
            var cursor = screen.Tree.CursorNode;
            Assert.Contains(screen.Tree.Roots, root => root.IsSearchDimmed);

            Press(screen, PhysicalKey.Escape);

            AssertNoSearch(screen);
            Assert.Same(cursor, screen.Tree.CursorNode);
            AssertFocusOnCursorRow(screen);
        }

        // The cursor row is under a row that the search opened: its parents stay open, so the focused row stays in sight.
        [AvaloniaFact]
        public void Escape_InTreeAfterAClickOnARow_ClearsSearchAndDimAndKeepsTheCursorRowOpen()
        {
            var screen = Show();
            var layers = screen.LayerNode(0).Parent!;
            layers.IsExpanded = false;
            Settle(screen.Window);
            var layer = SearchOneLayer(screen);
            Assert.True(layers.IsExpanded);

            Click(screen, layer);
            Assert.Same(layer, screen.Tree.CursorNode);
            Assert.Equal(layer.Header, screen.Tree.SearchText);

            Press(screen, PhysicalKey.Escape);

            AssertNoSearch(screen);
            Assert.True(layers.IsExpanded);
            Assert.Same(layer, screen.Tree.CursorNode);
            AssertFocusOnCursorRow(screen);
        }

        [AvaloniaFact]
        public void Escape_InTreeWithNoSearch_ChangesNothing()
        {
            var screen = Show();
            var layer = screen.LayerNode(1);
            screen.Tree.CursorNode = layer;
            Settle(screen.Window);
            screen.View.Rows.RowOf(layer)!.Focus(NavigationMethod.Tab);
            Settle(screen.Window);
            var expanded = All(screen.Tree.Roots).Select(node => node.IsExpanded).ToList();
            var selected = screen.Tree.SelectedNode;

            Press(screen, PhysicalKey.Escape);

            Assert.Same(layer, screen.Tree.CursorNode);
            Assert.Same(selected, screen.Tree.SelectedNode);
            AssertFocusOnCursorRow(screen);
            Assert.Equal(expanded, All(screen.Tree.Roots).Select(node => node.IsExpanded).ToList());
            Assert.Equal(string.Empty, screen.Tree.SearchText);
        }

        // ---------------------------------------------------------------- Tree.ClearSearch (Alt+F)

        [AvaloniaFact]
        public void AltF_InTheTextBoxSearchTree_ClearsSearchAndFocusesTheCursorRow()
        {
            var screen = Show();
            SearchOneLayer(screen);
            Assert.True(screen.Input.IsFocused);

            Press(screen, PhysicalKey.F, RawInputModifiers.Alt);

            AssertNoSearch(screen);
            Assert.False(screen.Input.IsFocused);
            AssertFocusOnCursorRow(screen);
        }

        [AvaloniaFact]
        public void AltF_InAnotherPanel_ClearsSearchAndFocusesTheCursorRow()
        {
            var screen = Show();
            SearchOneLayer(screen);
            var other = screen.Window.GetVisualDescendants().OfType<PanelSide>()
                .First(host => host.IsEffectivelyVisible && !host.GetVisualDescendants().OfType<ListTreeProject>().Any());
            screen.ViewModel.PanelFocus.FocusPanel(other);
            Settle(screen.Window);
            Assert.True(other.IsKeyboardFocusWithin);

            Press(screen, PhysicalKey.F, RawInputModifiers.Alt);

            AssertNoSearch(screen);
            Assert.True(screen.View.IsKeyboardFocusWithin);
            AssertFocusOnCursorRow(screen);
        }

        [AvaloniaFact]
        public void AltF_DuringAGrab_DoesNothing()
        {
            var screen = Show();
            screen.Tree.SearchText = screen.LayerNode(1).Header;
            screen.Tree.CursorNode = screen.LayerNode(0);
            Settle(screen.Window);
            screen.View.Rows.RowOf(screen.LayerNode(0))!.Focus(NavigationMethod.Tab);
            Settle(screen.Window);
            Press(screen, PhysicalKey.G);
            Assert.True(screen.View.Grab.IsGrabbing);
            Assert.False(screen.Tree.ClearSearchCommand.CanExecute(null));

            Press(screen, PhysicalKey.F, RawInputModifiers.Alt);

            Assert.True(screen.View.Grab.IsGrabbing);
            Assert.Equal(screen.LayerNode(1).Header, screen.Tree.SearchText);
            Assert.True(screen.Tree.IsSearchActive);
        }

        [AvaloniaFact]
        public void ClearSearchCommand_WithNoSearch_CannotRun()
        {
            var screen = Show();

            Assert.False(screen.Tree.ClearSearchCommand.CanExecute(null));
            screen.Tree.SearchText = "   ";
            Assert.False(screen.Tree.ClearSearchCommand.CanExecute(null));
            screen.Tree.SearchText = "Layer";
            Assert.True(screen.Tree.ClearSearchCommand.CanExecute(null));
        }

        [AvaloniaFact]
        public void KeyBindings_OfTheWindow_HoldTheClearSearchCommandOnAltF()
        {
            var screen = Show();

            var binding = screen.Window.KeyBindings.Single(candidate => Equals(candidate.Gesture, screen.ViewModel.Shortcuts["Tree.ClearSearch"]));

            Assert.Same(screen.Tree.ClearSearchCommand, binding.Command);
            Assert.Equal(new KeyGesture(Key.F, KeyModifiers.Alt), screen.ViewModel.Shortcuts["Tree.ClearSearch"]);
        }

        // Alt+F is Tree.ClearSearch. An access key on F in the main menu (a header with "_F") would take Alt+F first and open the menu.
        [AvaloniaFact]
        public void Headers_OfTheMainMenu_HaveNoAccessKeyOnF()
        {
            var screen = Show();
            var menu = screen.Window.GetVisualDescendants().OfType<Menu>().First();

            static IEnumerable<MenuItem> ItemsOf(ItemsControl control) =>
                control.Items.OfType<MenuItem>().SelectMany(item => new[] { item }.Concat(ItemsOf(item)));

            var headers = ItemsOf(menu).Select(item => item.Header).OfType<string>().ToList();
            Assert.NotEmpty(headers);
            Assert.DoesNotContain(headers, header => header.Contains("_F", StringComparison.OrdinalIgnoreCase));
        }

        // ---------------------------------------------------------------- the look of the filter

        [AvaloniaFact]
        public void Down_FromTheSearchWithAText_GivesTheFieldTheAccentBorderUntilTheSearchIsCleared()
        {
            var screen = Show();
            SearchOneLayer(screen);
            Assert.DoesNotContain(TextBoxSearchTree.FilteringClass, screen.Box.Field.Classes);

            Press(screen, PhysicalKey.ArrowDown);

            Assert.Contains(TextBoxSearchTree.FilteringClass, screen.Box.Field.Classes);
            Assert.Equal(AccentColor, BorderColorOf(screen));

            Press(screen, PhysicalKey.Escape);

            Assert.DoesNotContain(TextBoxSearchTree.FilteringClass, screen.Box.Field.Classes);
            Assert.NotEqual(AccentColor, BorderColorOf(screen));
        }

        [AvaloniaFact]
        public void Focus_TheTreeWithAnEmptyField_GivesTheFieldNoAccentBorder()
        {
            var screen = Show();
            screen.Box.Field.FocusInput();
            Settle(screen.Window);

            Press(screen, PhysicalKey.ArrowDown);

            Assert.True(screen.View.IsKeyboardFocusWithin);
            Assert.DoesNotContain(TextBoxSearchTree.FilteringClass, screen.Box.Field.Classes);
            Assert.NotEqual(AccentColor, BorderColorOf(screen));
        }

        [AvaloniaFact]
        public void Down_FromTheSearchWithAText_ShowsTheFilterHintsThenTheIdleHintsAfterEscape()
        {
            var screen = Show();
            SearchOneLayer(screen);
            Assert.Equal(TreeHintMode.Search, screen.View.HintMode);

            Press(screen, PhysicalKey.ArrowDown);

            Assert.Equal(TreeHintMode.Filter, screen.View.HintMode);
            var hints = screen.View.HintStrip.Hints!;
            Assert.Equal(new[] { "clear search", "edit search" }, hints.Take(2).Select(hint => hint.Words));
            Assert.Equal(TreeKeyHints.Escape, hints[0].Keys.Single());
            Assert.Equal("Ctrl+F", hints[1].Keys.Single());

            Press(screen, PhysicalKey.Escape);

            Assert.Equal(TreeHintMode.Idle, screen.View.HintMode);
            Assert.Equal("move", screen.View.HintStrip.Hints![0].Words);
        }
    }
}
