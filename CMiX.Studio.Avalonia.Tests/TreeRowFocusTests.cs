using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls.String;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeTestScreen;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // The keyboard focus goes back to the row of the tree after a rename and after a move by key, so Enter, the arrow keys and the
    // tree keys go on working with the pointer away from the tree. A click on another control keeps the focus on that control.
    public class TreeRowFocusTests : IDisposable
    {
        private sealed record Screen(Views.MainWindow Window, MainViewModel ViewModel, ProjectTreeView View)
        {
            public ProjectTree Tree => ViewModel.ProjectTree;
            public Composition Composition => (Composition)ViewModel.Project.CompositionManager.ManagerData.Items[0];
            public IList<IControl> Layers => Composition.LayerManager.ManagerData.Items;
            public TreeNode LayerNode(int index) => Tree.CompositionNodes()[0].Children[0].Children[index];
        }

        private Views.MainWindow? window;

        public void Dispose() => window?.Close();

        // One composition with three layers. The first layer is selected, and its row has the focus.
        private Screen Show()
        {
            var (_, mainWindow, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            window = mainWindow;
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.ManagerData.Items[0];
            composition.LayerManager.AddItem(typeof(Layer));
            composition.LayerManager.AddItem(typeof(Layer));
            composition.LayerManager.AddItem(typeof(Layer));
            TestServiceProviderFactory.Pump();
            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            TestServiceProviderFactory.Pump();
            var view = mainWindow.GetVisualDescendants().OfType<ProjectTreeView>().Single();
            var screen = new Screen(mainWindow, viewModel, view);
            screen.Tree.SelectedNode = screen.LayerNode(0);
            Settle(mainWindow);
            ItemOf(view, screen.LayerNode(0)).Focus();
            Settle(mainWindow);
            Assert.True(view.IsKeyboardFocusWithin);
            return screen;
        }

        private static void Press(Screen screen, PhysicalKey key, RawInputModifiers modifiers = RawInputModifiers.None)
        {
            screen.Window.KeyPressQwerty(key, modifiers);
            Settle(screen.Window);
        }

        private static void PointBesideTree(Screen screen)
        {
            var right = screen.View.TranslatePoint(new Point(screen.View.Bounds.Width, 0), screen.Window)!.Value.X;
            screen.Window.MouseMove(new Point(right + 10, 10));
            Dispatcher.UIThread.RunJobs();
            Assert.False(screen.View.IsPointerOver);
        }

        private static TextBoxEditable NameBoxOf(Screen screen, TreeNode node) =>
            RowOf(screen.View, node).GetVisualDescendants().OfType<TextBoxEditable>().First();

        private static IInputElement? Focused(Screen screen) => screen.Window.FocusManager!.GetFocusedElement();

        // Starts the rename of the selected row with the key, and types a name.
        private static TextBoxEditable StartRename(Screen screen, string name)
        {
            var node = screen.Tree.SelectedNode!;
            Press(screen, PhysicalKey.R, RawInputModifiers.Control);
            var box = NameBoxOf(screen, node);
            Assert.True(box.IsEditing);
            var input = box.GetVisualDescendants().OfType<TextBox>().First(text => text.IsVisible);
            Assert.True(input.IsFocused);
            input.Text = name;
            Settle(screen.Window);
            return box;
        }

        // The focus is on the row of the node, not in a text box of the row.
        private static void AssertTheRowHasTheFocus(Screen screen, TreeNode node)
        {
            Assert.Same(ItemOf(screen.View, node), Focused(screen));
        }

        // ---------------------------------------------------------------- after a rename

        [AvaloniaFact]
        public void Enter_AfterARename_PutsTheFocusOnTheRowAndDownMovesTheKeyboardRow()
        {
            var screen = Show();
            StartRename(screen, "Renamed");

            Press(screen, PhysicalKey.Enter);

            Assert.Equal("Renamed", screen.LayerNode(0).Header);
            AssertTheRowHasTheFocus(screen, screen.LayerNode(0));

            // Down goes to the next row that shows, the first child of the open layer.
            var rows = screen.Tree.VisibleNodes().ToList();
            var next = rows[rows.IndexOf(screen.LayerNode(0)) + 1];
            Press(screen, PhysicalKey.ArrowDown);
            Assert.Same(next, screen.Tree.CursorNode);
        }

        [AvaloniaFact]
        public void ShiftD_AfterARenameWithThePointerAwayFromTheTree_DuplicatesTheRow()
        {
            var screen = Show();
            PointBesideTree(screen);
            StartRename(screen, "Renamed");
            Press(screen, PhysicalKey.Enter);

            Press(screen, PhysicalKey.D, RawInputModifiers.Shift);

            Assert.Equal(4, screen.Layers.Count);
        }

        [AvaloniaFact]
        public void Escape_DuringARename_KeepsTheFocusInTheTree()
        {
            var screen = Show();
            StartRename(screen, "Typed");

            Press(screen, PhysicalKey.Escape);

            Assert.NotEqual("Typed", screen.LayerNode(0).Header);
            AssertTheRowHasTheFocus(screen, screen.LayerNode(0));
        }

        [AvaloniaFact]
        public void Click_OnAnotherControlDuringARename_LeavesTheFocusOnThatControl()
        {
            var screen = Show();
            StartRename(screen, "Typed");
            var search = screen.Window.GetVisualDescendants().OfType<TreeSearchBox>().First().GetVisualDescendants().OfType<TextBox>().First();

            var point = search.TranslatePoint(new Point(search.Bounds.Width / 2, search.Bounds.Height / 2), screen.Window)!.Value;
            screen.Window.MouseMove(point);
            screen.Window.MouseDown(point, MouseButton.Left);
            screen.Window.MouseUp(point, MouseButton.Left);
            Settle(screen.Window);

            Assert.False(NameBoxOf(screen, screen.LayerNode(0)).IsEditing);
            Assert.True(search.IsFocused);
        }

        // ---------------------------------------------------------------- after a move by key

        [AvaloniaFact]
        public void AltDown_WithTheTreeFocused_KeepsTheFocusOnTheMovedRow()
        {
            var screen = Show();
            var first = screen.Layers[0];
            PointBesideTree(screen);

            Press(screen, PhysicalKey.ArrowDown, RawInputModifiers.Alt);

            Assert.Same(first, screen.Layers[1]);
            Assert.True(screen.View.IsKeyboardFocusWithin);
            AssertTheRowHasTheFocus(screen, screen.Tree.SelectedNode!);

            Press(screen, PhysicalKey.ArrowDown, RawInputModifiers.Alt);

            Assert.Same(first, screen.Layers[2]);
        }

        // ---------------------------------------------------------------- after a detach

        [AvaloniaFact]
        public void GiveTheFocusBack_AfterADetachAndAttach_FocusesTheSelectedRow()
        {
            var screen = Show();
            var holder = screen.View.GetVisualParent()!.FindAncestorOfType<ContentControl>(true)!;
            var tree = screen.View.DataContext;

            holder.Content = null;
            Settle(screen.Window);
            holder.Content = screen.View;
            screen.View.DataContext = tree;
            Settle(screen.Window);

            AssertTheRowHasTheFocus(screen, screen.LayerNode(0));
        }
    }
}
