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
    // The keys of the selected row of the tree (Tree.Rename, Tree.Remove, Tree.Duplicate, Tree.MoveUp, Tree.MoveDown) are key bindings of
    // the main window. Every tree key has one condition (ProjectTree.CanRunTreeKey): the tree has the focus, or the pointer is over the
    // tree and no other panel has the focus. No key runs while a text box has the focus, and not during a drag of a row.
    public class TreeRowKeysTests : IDisposable
    {
        private sealed record Screen(Views.MainWindow Window, MainViewModel ViewModel, TreeViewProject View)
        {
            public ProjectTree Tree => ViewModel.ProjectTree;
            public Composition Composition => (Composition)ViewModel.Project.CompositionManager.ManagerData.Items[0];
            public IList<IControl> Layers => Composition.LayerManager.ManagerData.Items;
            public TreeNode LayerNode(int index) => Tree.CompositionNodes()[0].Children[0].Children[index];
        }

        private Views.MainWindow? window;

        public void Dispose() => window?.Close();

        // One composition with three layers. The first layer is selected. The focus is on the window, not in the tree.
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
            var view = mainWindow.GetVisualDescendants().OfType<TreeViewProject>().Single();
            var screen = new Screen(mainWindow, viewModel, view);
            screen.Tree.SelectedNode = screen.LayerNode(0);
            Settle(mainWindow);
            mainWindow.Focus();
            Settle(mainWindow);
            return screen;
        }

        private static void Press(Screen screen, PhysicalKey key, RawInputModifiers modifiers = RawInputModifiers.None)
        {
            screen.Window.KeyPressQwerty(key, modifiers);
            Settle(screen.Window);
        }

        private static void PointOnTree(Screen screen)
        {
            var row = RowOf(screen.View, screen.LayerNode(2));
            screen.Window.MouseMove(row.TranslatePoint(new Point(row.Bounds.Width / 2, row.Bounds.Height / 2), screen.Window)!.Value);
            Dispatcher.UIThread.RunJobs();
            Assert.True(screen.View.IsPointerOver);
        }

        private static void PointBesideTree(Screen screen)
        {
            var right = screen.View.TranslatePoint(new Point(screen.View.Bounds.Width, 0), screen.Window)!.Value.X;
            screen.Window.MouseMove(new Point(right + 10, 10));
            Dispatcher.UIThread.RunJobs();
            Assert.False(screen.View.IsPointerOver);
        }

        private static void FocusTheTree(Screen screen)
        {
            ItemOf(screen.View, screen.Tree.SelectedNode!).Focus();
            Settle(screen.Window);
            Assert.True(screen.View.IsKeyboardFocusWithin);
        }

        // A text box outside the tree: the search field in the bar of the panel.
        private static TextBox FocusATextBoxOutsideTheTree(Screen screen)
        {
            var box = screen.Window.GetVisualDescendants().OfType<TextBoxSearchTree>().First().GetVisualDescendants().OfType<TextBox>().First();
            box.Focus();
            Settle(screen.Window);
            Assert.True(box.IsFocused);
            Assert.False(screen.View.IsKeyboardFocusWithin);
            return box;
        }

        private static bool IsRenaming(Screen screen, TreeNode node) =>
            RowOf(screen.View, node).GetVisualDescendants().OfType<TextBoxEditable>().First().IsEditing;

        // ---------------------------------------------------------------- the pointer over the tree, the focus elsewhere

        [AvaloniaFact]
        public void Press_ShiftDWithThePointerOverTheTree_DuplicatesTheSelectedRowWithoutAClick()
        {
            var screen = Show();
            PointOnTree(screen);

            Press(screen, PhysicalKey.D, RawInputModifiers.Shift);

            Assert.Equal(4, screen.Layers.Count);
        }

        [AvaloniaFact]
        public void Press_DeleteWithThePointerOverTheTree_RemovesTheSelectedRowWithoutAClick()
        {
            var screen = Show();
            var first = screen.Layers[0];
            PointOnTree(screen);

            Press(screen, PhysicalKey.Delete);

            Assert.DoesNotContain(first, screen.Layers);
            Assert.Equal(2, screen.Layers.Count);
        }

        [AvaloniaFact]
        public void Press_CtrlRWithThePointerOverTheTree_StartsTheRenameWithoutAClick()
        {
            var screen = Show();
            PointOnTree(screen);

            Press(screen, PhysicalKey.R, RawInputModifiers.Control);

            Assert.True(IsRenaming(screen, screen.LayerNode(0)));
        }

        [AvaloniaFact]
        public void Press_AltDownThenAltUpWithThePointerOverTheTree_MovesTheSelectedRow()
        {
            var screen = Show();
            var first = screen.Layers[0];
            PointOnTree(screen);

            Press(screen, PhysicalKey.ArrowDown, RawInputModifiers.Alt);
            Assert.Same(first, screen.Layers[1]);

            Press(screen, PhysicalKey.ArrowUp, RawInputModifiers.Alt);
            Assert.Same(first, screen.Layers[0]);
        }

        // ---------------------------------------------------------------- the focus in the tree, the pointer elsewhere

        [AvaloniaFact]
        public void Press_ShiftDWithTheTreeFocusedAndThePointerBeside_DuplicatesTheSelectedRow()
        {
            var screen = Show();
            PointBesideTree(screen);
            FocusTheTree(screen);

            Press(screen, PhysicalKey.D, RawInputModifiers.Shift);

            Assert.Equal(4, screen.Layers.Count);
        }

        [AvaloniaFact]
        public void Press_DeleteWithTheTreeFocusedAndThePointerBeside_RemovesTheSelectedRow()
        {
            var screen = Show();
            var first = screen.Layers[0];
            PointBesideTree(screen);
            FocusTheTree(screen);

            Press(screen, PhysicalKey.Delete);

            Assert.DoesNotContain(first, screen.Layers);
        }

        [AvaloniaFact]
        public void Press_AltDownWithTheTreeFocusedAndThePointerBeside_MovesTheSelectedRow()
        {
            var screen = Show();
            var first = screen.Layers[0];
            PointBesideTree(screen);
            FocusTheTree(screen);

            Press(screen, PhysicalKey.ArrowDown, RawInputModifiers.Alt);

            Assert.Same(first, screen.Layers[1]);
        }

        [AvaloniaFact]
        public void Press_CtrlRWithTheTreeFocusedAndThePointerBeside_StartsTheRename()
        {
            var screen = Show();
            PointBesideTree(screen);
            FocusTheTree(screen);

            Press(screen, PhysicalKey.R, RawInputModifiers.Control);

            Assert.True(IsRenaming(screen, screen.LayerNode(0)));
        }

        // The tree takes the arrow keys for the cursor. An arrow with a modifier must still reach the key of the window.
        [AvaloniaFact]
        public void Press_AltUpWithTheTreeFocused_MovesTheSelectedRowUp()
        {
            var screen = Show();
            screen.Tree.SelectedNode = screen.LayerNode(1);
            var second = screen.Layers[1];
            PointBesideTree(screen);
            FocusTheTree(screen);

            Press(screen, PhysicalKey.ArrowUp, RawInputModifiers.Alt);

            Assert.Same(second, screen.Layers[0]);
        }

        [AvaloniaFact]
        public void Press_TheRowKeysWithThePointerBesideAndTheFocusElsewhere_DoNothing()
        {
            var screen = Show();
            PointBesideTree(screen);

            Press(screen, PhysicalKey.D, RawInputModifiers.Shift);
            Press(screen, PhysicalKey.Delete);
            Press(screen, PhysicalKey.ArrowDown, RawInputModifiers.Alt);

            Assert.Equal(3, screen.Layers.Count);
            Assert.Same(screen.LayerNode(0).Item, screen.Layers[0]);
        }

        // ---------------------------------------------------------------- the row of the arrow keys

        // The row keys act on the row of the arrow keys, not on the selected row (ProjectTree.TargetNode). The layers are closed, so
        // Down goes from layer to layer.
        private static void FocusTheTreeAndPressDown(Screen screen, int times)
        {
            for (var i = 0; i < 3; i++)
                screen.LayerNode(i).IsExpanded = false;
            Settle(screen.Window);
            PointBesideTree(screen);
            FocusTheTree(screen);
            for (var i = 0; i < times; i++)
                Press(screen, PhysicalKey.ArrowDown);

            Assert.Same(screen.LayerNode(times), screen.Tree.CursorNode);
            Assert.Same(screen.LayerNode(0), screen.Tree.SelectedNode);
        }

        [AvaloniaFact]
        public void RemoveByKey_AfterDownTwice_RemovesTheCursorRow()
        {
            var screen = Show();
            var (first, second, third) = (screen.Layers[0], screen.Layers[1], screen.Layers[2]);
            FocusTheTreeAndPressDown(screen, 2);

            Press(screen, PhysicalKey.Delete);

            Assert.Equal(new[] { first, second }, screen.Layers);
            Assert.DoesNotContain(third, screen.Layers);
        }

        [AvaloniaFact]
        public void RenameByKey_AfterDown_RenamesTheCursorRow()
        {
            var screen = Show();
            FocusTheTreeAndPressDown(screen, 1);

            Press(screen, PhysicalKey.R, RawInputModifiers.Control);

            Assert.True(IsRenaming(screen, screen.LayerNode(1)));
            Assert.False(IsRenaming(screen, screen.LayerNode(0)));
        }

        [AvaloniaFact]
        public void DuplicateByKey_AfterDown_DuplicatesTheCursorRow()
        {
            var screen = Show();
            var (first, second, third) = (screen.Layers[0], screen.Layers[1], screen.Layers[2]);
            FocusTheTreeAndPressDown(screen, 1);

            Press(screen, PhysicalKey.D, RawInputModifiers.Shift);

            Assert.Equal(4, screen.Layers.Count);
            Assert.Same(first, screen.Layers[0]);
            Assert.Same(second, screen.Layers[1]);
            Assert.DoesNotContain(screen.Layers[2], new[] { first, second, third });
            Assert.Same(third, screen.Layers[3]);
        }

        [AvaloniaFact]
        public void MoveByKey_AfterDown_MovesTheCursorRow()
        {
            var screen = Show();
            var (first, second) = (screen.Layers[0], screen.Layers[1]);
            FocusTheTreeAndPressDown(screen, 1);

            Press(screen, PhysicalKey.ArrowDown, RawInputModifiers.Alt);
            Assert.Same(first, screen.Layers[0]);
            Assert.Same(second, screen.Layers[2]);

            Press(screen, PhysicalKey.ArrowUp, RawInputModifiers.Alt);
            Assert.Same(second, screen.Layers[1]);
        }

        // ---------------------------------------------------------------- a text box has the focus

        // Ctrl+R and the key down of Shift+D change no text, so the text box does not take them, and the paused key binding of the window
        // could run. The command does not run while a text box has the focus.
        [AvaloniaFact]
        public void Press_TheRowKeysInATextBoxOutsideTheTree_DoNotTouchTheTree()
        {
            var screen = Show();
            PointOnTree(screen);
            FocusATextBoxOutsideTheTree(screen);

            Press(screen, PhysicalKey.R, RawInputModifiers.Control);
            Press(screen, PhysicalKey.D, RawInputModifiers.Shift);
            Press(screen, PhysicalKey.Delete);

            Assert.False(IsRenaming(screen, screen.LayerNode(0)));
            Assert.Equal(3, screen.Layers.Count);
        }

        [AvaloniaFact]
        public void Delete_InANameBoxThatIsEdited_RemovesNoItem()
        {
            var screen = Show();
            PointOnTree(screen);
            Press(screen, PhysicalKey.R, RawInputModifiers.Control);
            Press(screen, PhysicalKey.End);

            Press(screen, PhysicalKey.Delete);

            Assert.Equal(3, screen.Layers.Count);
        }

        // ---------------------------------------------------------------- a drag of a row

        [AvaloniaFact]
        public void Press_TheRowKeysDuringADrag_DoNothing()
        {
            var screen = Show();
            PointOnTree(screen);
            screen.Tree.BeginDrag(screen.LayerNode(0));

            Press(screen, PhysicalKey.D, RawInputModifiers.Shift);
            Press(screen, PhysicalKey.Delete);
            Press(screen, PhysicalKey.R, RawInputModifiers.Control);
            Press(screen, PhysicalKey.ArrowDown, RawInputModifiers.Alt);

            Assert.Equal(3, screen.Layers.Count);
            Assert.Same(screen.LayerNode(0).Item, screen.Layers[0]);
            Assert.False(IsRenaming(screen, screen.LayerNode(0)));
            screen.Tree.EndDrag();
        }

        // ---------------------------------------------------------------- the one condition of every tree key

        public enum TreeKey { Expand, Menu, Add, Remove, Rename, Search }

        public enum Place { FocusInTree, PointerOverWithFocusNowhere, PointerOverWithFocusInOtherPanel, TextBoxFocused, Drag, PointerBesideWithFocusNowhere }

        // A button of the other side panel. A button takes no letter key.
        private static void FocusAButtonOfTheOtherPanel(Screen screen)
        {
            var panel = screen.View.FindAncestorOfType<PanelSide>()!;
            var button = screen.Window.GetVisualDescendants().OfType<Button>().First(candidate =>
                candidate.Focusable && candidate.IsEffectivelyVisible && candidate.IsEffectivelyEnabled
                && candidate.FindAncestorOfType<PanelSide>() is { } host && !ReferenceEquals(host, panel));
            button.Focus();
            Settle(screen.Window);
            Assert.True(button.IsFocused);
            Assert.True(screen.Tree.Views.IsFocusInOtherPanel());
        }

        // Counts the runs of each tree key. The target row is the first layer. It has children, so Tree.Expand changes it.
        private sealed class KeyRuns
        {
            private readonly Screen screen;
            private readonly bool expanded;
            private readonly int layers;
            private int menus, adds, renames, searches;

            public KeyRuns(Screen screen)
            {
                this.screen = screen;
                expanded = screen.LayerNode(0).IsExpanded;
                layers = screen.Layers.Count;
                screen.Tree.MenuRequested += (_, _) => menus++;
                screen.Tree.AddRequested += (_, _) => adds++;
                screen.Tree.RenameRequested += (_, _) => renames++;
                screen.Tree.SearchFocusRequested += (_, _) => searches++;
            }

            public bool Ran(TreeKey key) => key switch
            {
                TreeKey.Expand => screen.LayerNode(0).IsExpanded != expanded,
                TreeKey.Menu => menus == 1,
                TreeKey.Add => adds == 1,
                TreeKey.Remove => screen.Layers.Count == layers - 1,
                TreeKey.Rename => renames == 1,
                _ => searches == 1
            };
        }

        private static void Press(Screen screen, TreeKey key)
        {
            var (physical, modifiers) = key switch
            {
                TreeKey.Expand => (PhysicalKey.E, RawInputModifiers.None),
                TreeKey.Menu => (PhysicalKey.W, RawInputModifiers.None),
                TreeKey.Add => (PhysicalKey.A, RawInputModifiers.Shift),
                TreeKey.Remove => (PhysicalKey.Delete, RawInputModifiers.None),
                TreeKey.Rename => (PhysicalKey.R, RawInputModifiers.Control),
                _ => (PhysicalKey.F, RawInputModifiers.Control)
            };
            Press(screen, physical, modifiers);
        }

        public static IEnumerable<object[]> KeysAndPlaces() =>
            from key in Enum.GetValues<TreeKey>()
            from place in Enum.GetValues<Place>()
            select new object[] { key, place };

        // The focus comes first: a key runs with the focus in the tree and the pointer anywhere. The pointer over the tree is enough
        // only while no other panel has the focus. During a drag, only Tree.Expand runs, and it goes to the row under the pointer.
        [AvaloniaTheory]
        [MemberData(nameof(KeysAndPlaces))]
        public void Press_ATreeKey_RunsOnTheOneConditionOfTheTreeKeys(TreeKey key, Place place)
        {
            var screen = Show();
            var dragToggles = 0;
            switch (place)
            {
                case Place.FocusInTree:
                    PointBesideTree(screen);
                    FocusTheTree(screen);
                    break;
                case Place.PointerOverWithFocusNowhere:
                    PointOnTree(screen);
                    Assert.False(screen.Tree.Views.IsFocusInOtherPanel());
                    break;
                case Place.PointerOverWithFocusInOtherPanel:
                    PointOnTree(screen);
                    FocusAButtonOfTheOtherPanel(screen);
                    break;
                case Place.TextBoxFocused:
                    PointOnTree(screen);
                    FocusATextBoxOutsideTheTree(screen);
                    break;
                case Place.Drag:
                    PointOnTree(screen);
                    screen.Tree.AttachView(new TreeViewHooks { ToggleDragRow = () => dragToggles++ });
                    screen.Tree.BeginDrag(screen.LayerNode(1));
                    break;
                default:
                    PointBesideTree(screen);
                    break;
            }

            var runs = new KeyRuns(screen);

            Press(screen, key);

            var expected = place is Place.FocusInTree or Place.PointerOverWithFocusNowhere;
            Assert.Equal(expected, runs.Ran(key));
            if (place == Place.Drag)
            {
                Assert.Equal(key == TreeKey.Expand ? 1 : 0, dragToggles);
                screen.Tree.EndDrag();
            }
        }

        // ---------------------------------------------------------------- the bindings

        [AvaloniaFact]
        public void KeyBindings_OfTheWindow_HoldTheKeyCommandsOfTheSelectedRow()
        {
            var screen = Show();
            var tree = screen.Tree;

            KeyBinding For(string name) => screen.Window.KeyBindings.Single(binding => Equals(binding.Gesture, screen.ViewModel.Shortcuts[name]));

            Assert.Same(tree.RenameSelectedByKeyCommand, For("Tree.Rename").Command);
            Assert.Same(tree.RemoveItemByKeyCommand, For("Tree.Remove").Command);
            Assert.Same(tree.DuplicateItemByKeyCommand, For("Tree.Duplicate").Command);
            Assert.Same(tree.MoveItemUpByKeyCommand, For("Tree.MoveUp").Command);
            Assert.Same(tree.MoveItemDownByKeyCommand, For("Tree.MoveDown").Command);
        }

        // ---------------------------------------------------------------- Tree.CollapseOthers

        [AvaloniaFact]
        public void Press_AltEWithThePointerOverTheTree_CollapsesEveryRowButThePathToTheSelection()
        {
            var screen = Show();
            var composition = screen.Tree.CompositionNodes()[0];
            var layers = composition.Children[0];
            screen.Tree.SelectedNode = screen.LayerNode(1);
            Settle(screen.Window);
            PointOnTree(screen);

            Press(screen, PhysicalKey.E, RawInputModifiers.Alt);

            Assert.True(composition.IsExpanded);
            Assert.True(layers.IsExpanded);
            Assert.All(new[] { screen.LayerNode(0), screen.LayerNode(2) }.Where(node => node.Children.Count > 0), node => Assert.False(node.IsExpanded));
        }

        [AvaloniaFact]
        public void Press_AltEInATextBoxOutsideTheTree_ChangesNoRow()
        {
            var screen = Show();
            screen.Tree.SelectedNode = screen.LayerNode(1);
            Settle(screen.Window);
            var open = screen.LayerNode(0).IsExpanded;
            PointOnTree(screen);
            FocusATextBoxOutsideTheTree(screen);

            Press(screen, PhysicalKey.E, RawInputModifiers.Alt);

            Assert.Equal(open, screen.LayerNode(0).IsExpanded);
        }

        [AvaloniaFact]
        public void KeyBindings_OfTheWindow_HoldTheCollapseOthersCommandOnAltE()
        {
            var screen = Show();

            var binding = screen.Window.KeyBindings.Single(candidate => Equals(candidate.Gesture, screen.ViewModel.Shortcuts["Tree.CollapseOthers"]));

            Assert.Equal(new KeyGesture(Key.E, KeyModifiers.Alt), screen.ViewModel.Shortcuts["Tree.CollapseOthers"]);
            Assert.Same(screen.Tree.CollapseOthersCommand, binding.Command);
        }
        [AvaloniaFact]
        public void KeyBindings_OfTheTree_HoldOnlyTheOpenCommand()
        {
            var screen = Show();

            var binding = Assert.Single(screen.View.KeyBindings);

            Assert.Same(screen.Tree.OpenSelectedCommand, binding.Command);
        }

        [AvaloniaFact]
        public void KeyBinding_AfterAChangeOfTheMap_UsesTheNewGesture()
        {
            var screen = Show();
            screen.ViewModel.Shortcuts.Set("Tree.Remove", new KeyGesture(Key.K, KeyModifiers.Control));
            Settle(screen.Window);
            PointOnTree(screen);

            Press(screen, PhysicalKey.Delete);
            Assert.Equal(3, screen.Layers.Count);

            Press(screen, PhysicalKey.K, RawInputModifiers.Control);
            Assert.Equal(2, screen.Layers.Count);
        }
    }
}
