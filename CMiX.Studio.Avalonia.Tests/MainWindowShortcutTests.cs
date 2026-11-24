using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // The undo and redo keys of the main window. The gestures come from the shortcut map.
    public class MainWindowShortcutTests
    {
        private static void Press(Views.MainWindow window, PhysicalKey key, RawInputModifiers modifiers)
        {
            window.KeyPressQwerty(key, modifiers);
            TestServiceProviderFactory.Pump();
        }

        [AvaloniaFact]
        public void Press_CtrlZThenCtrlShiftZ_UndoesAndRedoesTheStep()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            var items = viewModel.Project.CompositionManager.ManagerData.Items;
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            TestServiceProviderFactory.Pump();
            window.Focus();
            Assert.Single(items);

            Press(window, PhysicalKey.Z, RawInputModifiers.Control);
            Assert.Empty(items);

            Press(window, PhysicalKey.Z, RawInputModifiers.Control | RawInputModifiers.Shift);
            Assert.Single(items);
        }

        [AvaloniaFact]
        public void KeyBindings_OfTheWindow_FollowTheMap()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            var undo = window.KeyBindings.Single(binding => Equals(binding.Gesture, new KeyGesture(Key.Z, KeyModifiers.Control)));
            var gesture = new KeyGesture(Key.U, KeyModifiers.Control | KeyModifiers.Alt);

            viewModel.Shortcuts.Set("Edit.Undo", gesture);
            TestServiceProviderFactory.Pump();

            Assert.Equal(gesture, undo.Gesture);
        }

        [AvaloniaFact]
        public void KeyBindings_OfTheWindow_HoldTheUndoAndRedoCommands()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();

            KeyBinding For(string name) => window.KeyBindings.Single(binding => Equals(binding.Gesture, viewModel.Shortcuts[name]));

            Assert.Same(viewModel.UndoCommand, For("Edit.Undo").Command);
            Assert.Same(viewModel.RedoCommand, For("Edit.Redo").Command);
            Assert.Same(viewModel.RedoCommand, For("Edit.RedoAlt").Command);
        }

        [AvaloniaFact]
        public void KeyBindings_OfTheWindow_HoldThePanelCommands()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            var panelFocus = viewModel.PanelFocus;

            KeyBinding For(string name) => window.KeyBindings.Single(binding => Equals(binding.Gesture, viewModel.Shortcuts[name]));

            Assert.Same(panelFocus.NextPanelCommand, For("Window.NextPanel").Command);
            Assert.Same(panelFocus.PreviousPanelCommand, For("Window.PreviousPanel").Command);
            Assert.Same(panelFocus.FocusPanel1Command, For("Window.FocusPanel1").Command);
            Assert.Same(panelFocus.FocusPanel2Command, For("Window.FocusPanel2").Command);
            Assert.Same(panelFocus.FocusPanel3Command, For("Window.FocusPanel3").Command);
            Assert.Same(panelFocus.FocusPanel4Command, For("Window.FocusPanel4").Command);

            // Tree.Focus has no default gesture. Its binding takes the gesture that the user sets.
            var gesture = new KeyGesture(Key.T, KeyModifiers.Control);
            viewModel.Shortcuts.Set("Tree.Focus", gesture);
            TestServiceProviderFactory.Pump();
            Assert.Same(panelFocus.FocusTreeCommand, For("Tree.Focus").Command);
        }

        // The pointer on the middle of the tree, or beside it.
        private static void PointOnTree(Window window)
        {
            var view = window.GetVisualDescendants().OfType<ListTreeProject>().Single();
            window.MouseMove(view.TranslatePoint(new Point(view.Bounds.Width / 2, view.Bounds.Height / 2), window)!.Value);
            Dispatcher.UIThread.RunJobs();
        }

        private static void PointBesideTree(Window window)
        {
            var view = window.GetVisualDescendants().OfType<ListTreeProject>().Single();
            var right = view.TranslatePoint(new Point(view.Bounds.Width, 0), window)!.Value.X;
            window.MouseMove(new Point(right + 10, 10));
            Dispatcher.UIThread.RunJobs();
            Assert.False(view.IsPointerOver);
        }

        [AvaloniaFact]
        public void KeyBindings_OfTheWindow_HoldTheAddCommandOfTheTree()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();

            var add = window.KeyBindings.Single(binding => Equals(binding.Gesture, viewModel.Shortcuts["Tree.Add"]));

            Assert.Same(viewModel.ProjectTree.AddAtTargetCommand, add.Command);
        }

        [AvaloniaFact]
        public void Press_ShiftAWithTheFocusOutsideTheTree_OpensTheAddPanelOfTheSelectedRow()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.ManagerData.Items[0];
            composition.LayerManager.AddItem(typeof(Layer));
            TestServiceProviderFactory.Pump();
            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            TestServiceProviderFactory.Pump();
            var layer = viewModel.ProjectTree.CompositionNodes()[0].Children[0].Children[0];
            viewModel.ProjectTree.SelectedNode = layer;
            TestServiceProviderFactory.Pump();
            window.Focus();
            PointOnTree(window);

            Press(window, PhysicalKey.A, RawInputModifiers.Shift);

            // The panel opens at once, with no context menu first.
            var view = window.GetVisualDescendants().OfType<ListTreeProject>().Single();
            TestServiceProviderFactory.Pump();
            Assert.True(view.MenuAddPanel.IsOpen);
            var groups = view.MenuAddPanel.List.Groups!.ToList();
            Assert.Equal("Project", groups[^1].Header);

            // It is the panel of the selected row (a layer takes content), not of the project, and it has the title Add.
            Assert.Contains(groups, group => group.Header == "Content");
            Assert.Equal("Add", view.MenuAddPanel.Title.Text);
            Assert.All(groups, group => Assert.True(view.MenuAddPanel.List.IsOpen(group)));
            view.MenuAddPanel.Close();
        }

        [AvaloniaFact]
        public void Press_ShiftAWithThePointerBesideTheTree_DoesNothing()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            TestServiceProviderFactory.Pump();
            viewModel.ProjectTree.SelectedNode = null;
            var before = viewModel.Project.CompositionManager.ManagerData.Items.Count;
            window.Focus();
            PointBesideTree(window);

            Press(window, PhysicalKey.A, RawInputModifiers.Shift);

            Assert.Equal(before, viewModel.Project.CompositionManager.ManagerData.Items.Count);
            Assert.False(window.GetVisualDescendants().OfType<ListTreeProject>().Single().MenuAddPanel.IsOpen);
        }

        [AvaloniaFact]
        public void Press_ShiftAOverAnEmptyTree_OpensThePanelOfTheProjectAndAddsNothing()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            TestServiceProviderFactory.Pump();
            viewModel.ProjectTree.SelectedNode = null;
            var before = viewModel.Project.CompositionManager.ManagerData.Items.Count;
            window.Focus();
            PointOnTree(window);

            Press(window, PhysicalKey.A, RawInputModifiers.Shift);
            TestServiceProviderFactory.Pump();

            var view = window.GetVisualDescendants().OfType<ListTreeProject>().Single();
            Assert.Equal(before, viewModel.Project.CompositionManager.ManagerData.Items.Count);
            Assert.True(view.MenuAddPanel.IsOpen);
            Assert.Equal(new[] { "Project" }, view.MenuAddPanel.List.Groups!.Select(group => group.Header));
            view.MenuAddPanel.Close();
        }

        // The menu of the tree, and the texts that it shows.
        private static (ContextMenu? Menu, IReadOnlyList<string> Texts) MenuOf(Window window)
        {
            var menu = window.GetVisualDescendants().OfType<ListTreeProject>().Single().Menu;
            var texts = menu?.GetVisualDescendants().OfType<TextBlock>().Select(text => text.Text ?? "").ToList() ?? new List<string>();
            return (menu, texts);
        }

        [AvaloniaFact]
        public void KeyBindings_OfTheWindow_HoldTheMenuCommandOfTheTreeOnW()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();

            var menu = window.KeyBindings.Single(binding => Equals(binding.Gesture, viewModel.Shortcuts["Tree.Menu"]));

            Assert.Equal(new KeyGesture(Key.W), viewModel.Shortcuts["Tree.Menu"]);
            Assert.Same(viewModel.ProjectTree.OpenMenuAtTargetCommand, menu.Command);
        }

        [AvaloniaFact]
        public void KeyBindings_OfTheWindow_HoldTheExpandCommandOfTheTree()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();

            var expand = window.KeyBindings.Single(binding => Equals(binding.Gesture, viewModel.Shortcuts["Tree.Expand"]));

            Assert.Same(viewModel.ProjectTree.ToggleExpandTargetCommand, expand.Command);
        }

        [AvaloniaFact]
        public void Press_WOverTheTree_OpensTheMenuOfTheSelectedRow()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.ManagerData.Items[0];
            composition.LayerManager.AddItem(typeof(Layer));
            TestServiceProviderFactory.Pump();
            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            TestServiceProviderFactory.Pump();
            var layer = viewModel.ProjectTree.CompositionNodes()[0].Children[0].Children[0];
            viewModel.ProjectTree.SelectedNode = layer;
            TestServiceProviderFactory.Pump();
            window.Focus();
            PointOnTree(window);

            Press(window, PhysicalKey.W, RawInputModifiers.None);
            TestServiceProviderFactory.Pump();

            var (menu, texts) = MenuOf(window);
            Assert.True(menu?.IsOpen);
            Assert.Contains(layer.Header, texts);
            menu!.Close();
        }

        [AvaloniaFact]
        public void Press_WOverTheTreeWithNothingSelected_OpensTheMenuOfTheProject()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            TestServiceProviderFactory.Pump();
            viewModel.ProjectTree.SelectedNode = null;
            window.Focus();
            PointOnTree(window);

            Press(window, PhysicalKey.W, RawInputModifiers.None);
            TestServiceProviderFactory.Pump();

            var (menu, texts) = MenuOf(window);
            Assert.True(menu?.IsOpen);
            Assert.Contains("Project", texts);
            menu!.Close();
        }

        [AvaloniaFact]
        public void Press_WBesideTheTree_OpensNoMenu()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            TestServiceProviderFactory.Pump();
            window.Focus();
            PointBesideTree(window);

            Press(window, PhysicalKey.W, RawInputModifiers.None);
            TestServiceProviderFactory.Pump();

            Assert.NotEqual(true, MenuOf(window).Menu?.IsOpen);
        }

        // The key is a plain letter: in a name box it types the letter and opens no menu.
        [AvaloniaFact]
        public void Press_WInANameBoxOverTheTree_OpensNoMenu()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            TestServiceProviderFactory.Pump();
            var node = viewModel.ProjectTree.CompositionNodes()[0];
            viewModel.ProjectTree.SelectedNode = node;
            TestServiceProviderFactory.Pump();
            PointOnTree(window);
            viewModel.ProjectTree.RenameSelectedCommand.Execute(null);
            TestServiceProviderFactory.Pump();
            Assert.IsAssignableFrom<TextBox>(window.FocusManager!.GetFocusedElement());

            Press(window, PhysicalKey.W, RawInputModifiers.None);
            TestServiceProviderFactory.Pump();

            Assert.NotEqual(true, MenuOf(window).Menu?.IsOpen);
        }

        [AvaloniaFact]
        public void Press_EOverTheTree_CollapsesAndExpandsTheSelectedRow()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            TestServiceProviderFactory.Pump();
            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            var node = viewModel.ProjectTree.CompositionNodes()[0];
            viewModel.ProjectTree.SelectedNode = node;
            TestServiceProviderFactory.Pump();
            window.Focus();
            PointOnTree(window);
            Assert.True(node.IsExpanded);

            Press(window, PhysicalKey.E, RawInputModifiers.None);
            Assert.False(node.IsExpanded);

            Press(window, PhysicalKey.E, RawInputModifiers.None);
            Assert.True(node.IsExpanded);
        }

        [AvaloniaFact]
        public void Press_EBesideTheTree_ChangesNothing()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            TestServiceProviderFactory.Pump();
            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            var node = viewModel.ProjectTree.CompositionNodes()[0];
            viewModel.ProjectTree.SelectedNode = node;
            TestServiceProviderFactory.Pump();
            window.Focus();
            PointBesideTree(window);

            Press(window, PhysicalKey.E, RawInputModifiers.None);

            Assert.True(node.IsExpanded);
        }

        // E and Shift+A are letters: in a name box they type the letter and do nothing else.
        [AvaloniaFact]
        public void Press_EOrShiftAInANameBoxOverTheTree_DoesNothingElse()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            TestServiceProviderFactory.Pump();
            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            var node = viewModel.ProjectTree.CompositionNodes()[0];
            viewModel.ProjectTree.SelectedNode = node;
            TestServiceProviderFactory.Pump();
            PointOnTree(window);
            viewModel.ProjectTree.RenameSelectedCommand.Execute(null);
            TestServiceProviderFactory.Pump();
            Assert.IsAssignableFrom<TextBox>(window.FocusManager!.GetFocusedElement());

            Press(window, PhysicalKey.E, RawInputModifiers.None);
            Press(window, PhysicalKey.A, RawInputModifiers.Shift);

            Assert.True(node.IsExpanded);
            Assert.False(window.GetVisualDescendants().OfType<ListTreeProject>().Single().MenuAddPanel.IsOpen);
        }

        [AvaloniaFact]
        public void KeyBindings_OfTheWindow_HoldTheOpenCommandOfTheMenuOnCtrlO()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            TestServiceProviderFactory.Pump();

            var open = window.KeyBindings.Single(binding => Equals(binding.Gesture, viewModel.Shortcuts["File.Open"]));
            var row = window.GetVisualDescendants().OfType<Menu>().First().Items.OfType<MenuItem>().First()
                .Items.OfType<MenuItem>().Single(item => Equals(item.Header, "Open"));

            Assert.Equal(new KeyGesture(Key.O, KeyModifiers.Control), viewModel.Shortcuts["File.Open"]);
            Assert.Same(viewModel.ProjectFile.OpenProjectCommand, open.Command);
            Assert.Equal(new KeyGesture(Key.O, KeyModifiers.Control), row.InputGesture);
        }

        // Alt is a part of the tree keys. Alone, it must not give the keyboard to the main menu.
        [AvaloniaFact]
        public void Press_AltAlone_LeavesTheMainMenuAsItWas()
        {
            var (_, window, _) = TestServiceProviderFactory.ShowMainWindow();
            TestServiceProviderFactory.Pump();
            window.Focus();
            var menu = window.GetVisualDescendants().OfType<Menu>().First();
            var file = menu.Items.OfType<MenuItem>().First();

            Press(window, PhysicalKey.AltLeft, RawInputModifiers.None);

            // Avalonia shows the access keys at the press of Alt, and opens the menu at its release when they show.
            Assert.False(window.GetValue(global::Avalonia.Controls.Primitives.AccessText.ShowAccessKeyProperty));
            Assert.False(file.IsSelected);
            Assert.False(file.IsSubMenuOpen);
            Assert.False(menu.IsKeyboardFocusWithin);
        }

        [AvaloniaFact]
        public void Set_AnotherGestureForTreeAdd_ReplacesShiftA()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            TestServiceProviderFactory.Pump();
            viewModel.ProjectTree.SelectedNode = viewModel.ProjectTree.CompositionNodes()[0];
            viewModel.Shortcuts.Set("Tree.Add", new KeyGesture(Key.N, KeyModifiers.Control));
            TestServiceProviderFactory.Pump();
            window.Focus();
            PointOnTree(window);
            var view = window.GetVisualDescendants().OfType<ListTreeProject>().Single();

            Press(window, PhysicalKey.A, RawInputModifiers.Shift);
            TestServiceProviderFactory.Pump();
            Assert.False(view.MenuAddPanel.IsOpen);

            Press(window, PhysicalKey.N, RawInputModifiers.Control);
            TestServiceProviderFactory.Pump();
            Assert.True(view.MenuAddPanel.IsOpen);
            view.MenuAddPanel.Close();
        }
    }
}
