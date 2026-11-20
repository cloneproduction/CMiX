using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Controls.String;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeTestScreen;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // A key opens the menu and the Add panel at the cursor row (ProjectTree.TargetNode), not at the pointer. A right click opens the menu
    // at the pointer and puts the cursor on the clicked row.
    public class TreeMenuAtCursorTests : IDisposable
    {
        private sealed record Screen(Views.MainWindow Window, MainViewModel ViewModel, TreeViewProject View)
        {
            public ProjectTree Tree => ViewModel.ProjectTree;
            public TreeNode LayerNode(int index) => Tree.CompositionNodes()[0].Children[0].Children[index];
            public TreeView TreeView => View.GetVisualDescendants().OfType<TreeView>().First();
            public ContextMenu? Menu => TreeView.ContextMenu;
        }

        private Screen? screen;

        public void Dispose()
        {
            if (screen == null)
                return;

            screen.Menu?.Close();
            screen.View.MenuAddPanel.Close();
            screen.Window.Close();
            Dispatcher.UIThread.RunJobs();
        }

        // One composition with three layers. The first layer is selected. The focus is on its row.
        private Screen Show()
        {
            var (_, mainWindow, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.ManagerData.Items[0];
            composition.LayerManager.AddItem(typeof(Layer));
            composition.LayerManager.AddItem(typeof(Layer));
            composition.LayerManager.AddItem(typeof(Layer));
            TestServiceProviderFactory.Pump();
            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            TestServiceProviderFactory.Pump();
            var view = mainWindow.GetVisualDescendants().OfType<TreeViewProject>().Single();
            screen = new Screen(mainWindow, viewModel, view);
            screen.Tree.SelectedNode = screen.LayerNode(0);
            Settle(mainWindow);
            ItemOf(view, screen.LayerNode(0)).Focus();
            Settle(mainWindow);
            return screen;
        }

        // Down moves the cursor away from the selected row. The pointer rests on the last layer, away from the cursor row.
        private static TreeNode MoveCursorDown(Screen screen)
        {
            var away = RowOf(screen.View, screen.LayerNode(2));
            screen.Window.MouseMove(away.TranslatePoint(new Point(away.Bounds.Width / 2, away.Bounds.Height / 2), screen.Window)!.Value);
            Press(screen, PhysicalKey.ArrowDown);
            var cursor = screen.Tree.CursorNode!;
            Assert.NotSame(screen.Tree.SelectedNode, cursor);
            Assert.NotSame(screen.LayerNode(2), cursor);
            return cursor;
        }

        private static void Press(Screen screen, PhysicalKey key, RawInputModifiers modifiers = RawInputModifiers.None)
        {
            screen.Window.KeyPressQwerty(key, modifiers);
            screen.Window.KeyReleaseQwerty(key, modifiers);
            Settle(screen.Window);
        }

        private static Point CenterOf(Screen screen, Control control) =>
            control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), screen.Window)!.Value;

        // The left edge of the name and the bottom of the row, in the window.
        private static Point BelowNameOf(Screen screen, TreeNode node)
        {
            var row = RowOf(screen.View, node);
            var name = row.GetVisualDescendants().OfType<TextBoxEditable>().First();
            return new Point(name.TranslatePoint(default, screen.Window)!.Value.X, row.TranslatePoint(new Point(0, row.Bounds.Height), screen.Window)!.Value.Y);
        }

        // The visible panel of the menu, without the margin of the shadow.
        private static Rect MenuBox(Screen screen)
        {
            var frame = screen.Menu!.GetVisualDescendants().OfType<PopupFrame>().First();
            var origin = frame.TranslatePoint(default, screen.Window)!.Value;
            return new Rect(origin.X + PopupFrame.ShadowMargin.Left, origin.Y + PopupFrame.ShadowMargin.Top,
                frame.Bounds.Width - PopupFrame.ShadowMargin.Left - PopupFrame.ShadowMargin.Right,
                frame.Bounds.Height - PopupFrame.ShadowMargin.Top - PopupFrame.ShadowMargin.Bottom);
        }

        private static string? MenuTitle(Screen screen) =>
            screen.Menu!.Items.OfType<MenuItem>().First().Header as string;

        private static void AssertMenuUnderRow(Screen screen, TreeNode node)
        {
            Assert.True(screen.Menu?.IsOpen);
            Assert.Equal(node.Header, MenuTitle(screen));
            var box = MenuBox(screen);
            var below = BelowNameOf(screen, node);
            Assert.Equal(below.X, box.X, 0.51);
            Assert.Equal(below.Y, box.Y, 0.51);
        }

        [AvaloniaFact]
        public void ShiftA_AfterDown_OpensTheAddPanelAtTheCursorRow()
        {
            var screen = Show();
            var cursor = MoveCursorDown(screen);

            Press(screen, PhysicalKey.A, RawInputModifiers.Shift);

            var panel = screen.View.MenuAddPanel;
            Assert.True(panel.IsOpen);
            var search = panel.List.GetVisualDescendants().OfType<TextBox>().Single(box => box.Name == "PART_Search");
            var row = CenterOf(screen, RowOf(screen.View, cursor));
            var field = CenterOf(screen, search);
            Assert.Equal(row.X, field.X, 1.5);
            Assert.Equal(row.Y, field.Y, 1.5);
        }

        [AvaloniaFact]
        public void W_AfterDown_OpensTheMenuUnderTheCursorRowAtTheName()
        {
            var screen = Show();
            var cursor = MoveCursorDown(screen);

            Press(screen, PhysicalKey.W);

            AssertMenuUnderRow(screen, cursor);
        }

        [AvaloniaFact]
        public void ContextMenuKey_AfterDown_OpensTheMenuUnderTheFocusedRow()
        {
            var screen = Show();
            var cursor = MoveCursorDown(screen);
            Assert.True(ItemOf(screen.View, cursor).IsFocused);

            Press(screen, PhysicalKey.ContextMenu, RawInputModifiers.None);

            AssertMenuUnderRow(screen, cursor);
        }

        [AvaloniaFact]
        public void RightClick_SelectedRowAfterDown_OpensTheMenuAtThePointerAndMovesTheCursorThere()
        {
            var screen = Show();
            var selected = screen.Tree.SelectedNode!;
            MoveCursorDown(screen);
            var point = CenterOf(screen, RowOf(screen.View, selected));

            screen.Window.MouseMove(point);
            screen.Window.MouseDown(point, MouseButton.Right);
            screen.Window.MouseUp(point, MouseButton.Right);
            Settle(screen.Window);

            Assert.Same(selected, screen.Tree.CursorNode);
            Assert.Same(selected, screen.Tree.SelectedNode);
            Assert.True(screen.Menu?.IsOpen);
            Assert.Equal(selected.Header, MenuTitle(screen));
            Assert.True(MenuBox(screen).Contains(point));
        }
    }
}
