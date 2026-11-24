// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Popups;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.ListTreeTestScreen;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Composition = CMiX.Core.Compositing.Composition;
using Echo = CMiX.Core.Texturing.Filters.Echo;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // The Tree commands of the main window, with their key bindings and the text box gate.
    // Tree.Hide (H) hides or shows the target row as a click on its eye does. Tree.ShowAll (Alt+H) shows every hidden row as one
    // undo step. Both show a notification with the undo key.
    // Tree.ExpandBranch (Shift+E), Tree.CloseLevel (Q), Tree.OpenLevel (Shift+Q) and Tree.CollapseAll (Shift+Alt+E) change the open
    // state of the rows only.
    // Tree.Add (Shift+A), Tree.Menu (W) and Tree.Expand (E) act on the target row when the pointer or the focus is in the tree.
    public class TreeKeyCommandsTests : IDisposable
    {
        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

        private sealed record HideScreen(IServiceProvider Provider, Views.MainWindow Window, MainViewModel ViewModel, ListTreeProject View, PanelNotification Host)
        {
            public ProjectTree Tree => ViewModel.ProjectTree;
            public UndoManager Undo => Provider.GetRequiredService<UndoManager>();
            public TreeNode LayersNode => Tree.CompositionNodes()[0].Children[0];
            public TreeNode LayerNode(int index) => LayersNode.Children[index];

            public int UndoSteps => UndoCounts.StepsOf(Undo);
        }

        // One composition with three layers. Every row is open. The first layer is selected. The undo history is empty.
        private HideScreen ShowHide()
        {
            var main = ListTreeMainScreen.Show(windows);
            var host = main.Window.GetVisualDescendants().OfType<PanelNotification>().Single();
            var screen = new HideScreen(main.Provider, main.Window, main.ViewModel, main.View, host);
            screen.Tree.SelectedNode = screen.LayerNode(0);
            Settle(main.Window);
            screen.Undo.Clear();
            return screen;
        }

        private static void Focus(HideScreen screen, TreeNode node)
        {
            screen.Tree.CursorNode = node;
            Settle(screen.Window);
            screen.View.Rows.RowOf(node)!.Focus();
            Settle(screen.Window);
            Assert.True(screen.View.IsKeyboardFocusWithin);
        }

        private static ToggleButton EyeOf(HideScreen screen, TreeNode node) =>
            screen.View.Rows.RowOf(node)!.Toggles.GetVisualDescendants().OfType<ToggleButton>()
                .First(button => button.Name is "visibilityToggle" or "groupHideToggle" && button.IsEffectivelyVisible);

        [AvaloniaFact]
        public void Press_HAfterDown_HidesTheCursorRowAndNotTheSelectedRow()
        {
            var screen = ShowHide();
            screen.LayerNode(0).IsExpanded = false;
            Focus(screen, screen.LayerNode(0));

            screen.Window.Press(PhysicalKey.ArrowDown);
            Assert.Same(screen.LayerNode(1), screen.Tree.CursorNode);
            screen.Window.Press(PhysicalKey.H);

            Assert.True(screen.LayerNode(1).IsHidden);
            Assert.False(screen.LayerNode(0).IsHidden);
            Assert.Same(screen.LayerNode(0), screen.Tree.SelectedNode);
            Assert.Equal(1, screen.UndoSteps);

            screen.Undo.Undo();

            Assert.False(screen.LayerNode(1).IsHidden);
        }

        [AvaloniaFact]
        public void Press_HOnAGroupRow_TogglesTheHideOfTheList()
        {
            var screen = ShowHide();
            var group = screen.LayersNode;
            Focus(screen, group);

            screen.Window.Press(PhysicalKey.H);

            Assert.True(group.Group!.Manager.Hidden.Value);
            Assert.All(group.Children, layer => Assert.False(layer.IsHidden));
            Assert.Equal(1, screen.UndoSteps);

            screen.Undo.Undo();

            Assert.False(group.Group.Manager.Hidden.Value);
        }

        // The eye click and H write the same undo step and send the same wire message. The undo sends the reverse value.
        [AvaloniaFact]
        public void ToggleHideTarget_AndTheEyeClick_WriteTheSameUndoStepAndMessage()
        {
            var screen = ShowHide();
            var messenger = screen.Provider.GetRequiredService<ControlMessenger>();
            messenger.IsSendingBlocked = false;
            var recorder = new MessageRecorder();
            messenger.Register(recorder);
            var clicked = screen.LayerNode(1);
            var pressed = screen.LayerNode(2);

            screen.Window.Click(EyeOf(screen, clicked));
            var clickStep = UndoCounts.LastStepOf(screen.Undo);
            var clickMessage = recorder.Sent.Last();
            Focus(screen, pressed);
            screen.Window.Press(PhysicalKey.H);
            var keyStep = UndoCounts.LastStepOf(screen.Undo);
            var keyMessage = recorder.Sent.Last();

            Assert.True(clicked.IsHidden);
            Assert.True(pressed.IsHidden);
            Assert.Equal(2, screen.UndoSteps);
            Assert.Equal(clickStep.GetType(), keyStep.GetType());
            Assert.Equal(clicked.HiddenValue!.ID, ((ValueChangedCommand)clickStep).ControlID);
            Assert.Equal(pressed.HiddenValue!.ID, ((ValueChangedCommand)keyStep).ControlID);
            Assert.Equal(clickMessage.GetType(), keyMessage.GetType());
            Assert.Equal(clicked.HiddenValue.ID, ((MessageValueChanged)clickMessage).ID);
            Assert.Equal(pressed.HiddenValue.ID, ((MessageValueChanged)keyMessage).ID);

            recorder.Sent.Clear();
            screen.Undo.Undo();

            Assert.False(pressed.IsHidden);
            var reverse = Assert.IsType<MessageValueChanged>(Assert.Single(recorder.Sent));
            Assert.Equal(pressed.HiddenValue.ID, reverse.ID);
        }

        [AvaloniaFact]
        public void Press_HTwice_ShowsHiddenThenShownWithTheUndoKey()
        {
            var screen = ShowHide();
            var node = screen.LayerNode(1);
            Focus(screen, node);

            screen.Window.Press(PhysicalKey.H);

            Assert.True(screen.Host.IsShowing);
            Assert.Equal($"Hidden: {node.Header}", screen.Host.Notification.Text);
            Assert.Equal("Ctrl+Z", screen.Host.Notification.UndoGesture);

            screen.Window.Press(PhysicalKey.H);

            Assert.False(node.IsHidden);
            Assert.Equal($"Shown: {node.Header}", screen.Host.Notification.Text);
            Assert.Equal("Ctrl+Z", screen.Host.Notification.UndoGesture);
        }

        // ---------------------------------------------------------------- Tree.ShowAll

        [AvaloniaFact]
        public void Press_AltH_ShowsThreeHiddenRowsInOneUndoStep()
        {
            var screen = ShowHide();
            var group = screen.LayerNode(0).Children[0];
            group.Group!.Manager.Hidden.Value = true;
            screen.LayerNode(1).HiddenValue!.Value = true;
            screen.LayerNode(2).HiddenValue!.Value = true;
            // A hidden row under a closed row also shows again.
            screen.LayerNode(0).IsExpanded = false;
            Settle(screen.Window);
            var before = screen.UndoSteps;
            Focus(screen, screen.LayerNode(1));

            screen.Window.Press(PhysicalKey.H, RawInputModifiers.Alt);

            Assert.False(group.IsHidden);
            Assert.False(screen.LayerNode(1).IsHidden);
            Assert.False(screen.LayerNode(2).IsHidden);
            Assert.Equal(before + 1, screen.UndoSteps);
            Assert.Equal("All rows shown", screen.Host.Notification.Text);
            Assert.Equal("Ctrl+Z", screen.Host.Notification.UndoGesture);

            screen.Undo.Undo();

            Assert.True(group.IsHidden);
            Assert.True(screen.LayerNode(1).IsHidden);
            Assert.True(screen.LayerNode(2).IsHidden);
        }

        [AvaloniaFact]
        public void Press_AltHWithNothingHidden_AddsNoUndoStepAndNoNotification()
        {
            var screen = ShowHide();
            Focus(screen, screen.LayerNode(1));

            screen.Window.Press(PhysicalKey.H, RawInputModifiers.Alt);

            Assert.Equal(0, screen.UndoSteps);
            Assert.False(screen.Host.IsShowing);

            screen.Tree.ShowAll();

            Assert.Equal(0, screen.UndoSteps);
            Assert.False(screen.Host.IsShowing);
        }

        // ---------------------------------------------------------------- gate and bindings

        [AvaloniaFact]
        public void Press_HAndAltHInATextBox_ChangeNoRow()
        {
            var screen = ShowHide();
            screen.LayerNode(2).HiddenValue!.Value = true;
            screen.Tree.CursorNode = screen.LayerNode(1);
            Settle(screen.Window);
            var box = screen.Window.GetVisualDescendants().OfType<TextBoxSearchTree>().First().GetVisualDescendants().OfType<TextBox>().First();
            box.Focus();
            Settle(screen.Window);
            Assert.True(box.IsFocused);
            var before = screen.UndoSteps;

            screen.Window.Press(PhysicalKey.H);
            screen.Window.Press(PhysicalKey.H, RawInputModifiers.Alt);

            Assert.False(screen.LayerNode(1).IsHidden);
            Assert.True(screen.LayerNode(2).IsHidden);
            Assert.Equal(before, screen.UndoSteps);
        }

        [AvaloniaFact]
        public void KeyBindings_OfTheWindow_HoldTheHideCommands()
        {
            var screen = ShowHide();

            KeyBinding For(string name) => screen.Window.KeyBindings.Single(binding => Equals(binding.Gesture, screen.ViewModel.Shortcuts[name]));

            Assert.Equal(new KeyGesture(Key.H), screen.ViewModel.Shortcuts["Tree.Hide"]);
            Assert.Equal(new KeyGesture(Key.H, KeyModifiers.Alt), screen.ViewModel.Shortcuts["Tree.ShowAll"]);
            Assert.Same(screen.Tree.ToggleHideTargetCommand, For("Tree.Hide").Command);
            Assert.Same(screen.Tree.ShowAllCommand, For("Tree.ShowAll").Command);
        }

        private static void AssertCursorShows(ProjectTree tree) =>
            Assert.Contains(tree.CursorNode!, tree.VisibleNodes());

        // One composition with three layers, each with an Echo filter. Every row is open. The composition is selected.
        private (Views.MainWindow Window, MainViewModel ViewModel, ListTreeProject View) ShowLevels()
        {
            var (_, mainWindow, viewModel) = windows.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.ManagerData.Items[0];
            for (var i = 0; i < 3; i++)
            {
                composition.LayerManager.AddItem(typeof(Layer));
                ((Layer)composition.LayerManager.ManagerData.Items[i]).TextureModifierManager.AddItem(typeof(Echo));
            }

            TestServiceProviderFactory.Pump();
            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            TestServiceProviderFactory.Pump();
            var view = mainWindow.GetVisualDescendants().OfType<ListTreeProject>().Single();
            viewModel.ProjectTree.SelectedNode = viewModel.ProjectTree.CompositionNodes()[0];
            Settle(mainWindow);
            return (mainWindow, viewModel, view);
        }

        private static void FocusTheCursorRow(Views.MainWindow mainWindow, ListTreeProject view, ProjectTree tree)
        {
            view.Rows.RowOf(tree.CursorNode!)!.Focus();
            Settle(mainWindow);
            Assert.True(view.IsKeyboardFocusWithin);
        }

        private static IEnumerable<TreeNode> RowsWithChildren(ProjectTree tree) => ListTreeTestScreen.All(tree.Roots).Where(node => node.Children.Count > 0);

        [AvaloniaFact]
        public void Press_ShiftEInTheTree_OpensTheBranchOfTheCursorRow()
        {
            var (mainWindow, viewModel, view) = ShowLevels();
            var tree = viewModel.ProjectTree;
            var composition = tree.CompositionNodes()[0];
            foreach (var node in RowsWithChildren(tree).Where(node => node.IsOrIsUnder(composition) && !ReferenceEquals(node, composition)))
                node.IsExpanded = false;
            Settle(mainWindow);
            FocusTheCursorRow(mainWindow, view, tree);

            mainWindow.Press(PhysicalKey.E, RawInputModifiers.Shift);

            Assert.All(ListTreeTestScreen.All(new[] { composition }).Where(node => node.Children.Count > 0), node => Assert.True(node.IsExpanded));
        }

        [AvaloniaFact]
        public void Press_QInTheTree_ClosesTheDeepestOpenLevel()
        {
            var (mainWindow, viewModel, view) = ShowLevels();
            var tree = viewModel.ProjectTree;
            FocusTheCursorRow(mainWindow, view, tree);
            var before = RowsWithChildren(tree).Count(node => node.IsExpanded);

            mainWindow.Press(PhysicalKey.Q);

            Assert.True(RowsWithChildren(tree).Count(node => node.IsExpanded) < before);
            Assert.True(tree.CompositionNodes()[0].IsExpanded);
            AssertCursorShows(tree);
        }

        [AvaloniaFact]
        public void Press_ShiftQInTheTree_OpensTheShallowestClosedLevel()
        {
            var (mainWindow, viewModel, view) = ShowLevels();
            var tree = viewModel.ProjectTree;
            var layers = Scene.LayersOf(tree.CompositionNodes()[0]);
            layers.IsExpanded = false;
            Settle(mainWindow);
            FocusTheCursorRow(mainWindow, view, tree);

            mainWindow.Press(PhysicalKey.Q, RawInputModifiers.Shift);

            Assert.True(layers.IsExpanded);
        }

        [AvaloniaFact]
        public void Press_ShiftAltEInTheTree_ClosesEveryRowAndKeepsTheCursorOnARowThatShows()
        {
            var (mainWindow, viewModel, view) = ShowLevels();
            var tree = viewModel.ProjectTree;
            FocusTheCursorRow(mainWindow, view, tree);

            mainWindow.Press(PhysicalKey.E, RawInputModifiers.Shift | RawInputModifiers.Alt);

            Assert.All(RowsWithChildren(tree), node => Assert.False(node.IsExpanded));
            AssertCursorShows(tree);
        }

        [AvaloniaFact]
        public void Press_TheLevelKeysInATextBox_ChangeNoRow()
        {
            var (mainWindow, viewModel, _) = ShowLevels();
            var tree = viewModel.ProjectTree;
            Scene.LayersOf(tree.CompositionNodes()[0]).IsExpanded = false;
            Settle(mainWindow);
            var box = mainWindow.GetVisualDescendants().OfType<TextBoxSearchTree>().First().GetVisualDescendants().OfType<TextBox>().First();
            box.Focus();
            Settle(mainWindow);
            Assert.True(box.IsFocused);
            var before = RowsWithChildren(tree).Select(node => node.IsExpanded).ToList();

            mainWindow.Press(PhysicalKey.E, RawInputModifiers.Shift | RawInputModifiers.Alt);
            mainWindow.Press(PhysicalKey.E, RawInputModifiers.Shift);
            mainWindow.Press(PhysicalKey.Q, RawInputModifiers.Shift);
            mainWindow.Press(PhysicalKey.Q);

            Assert.Equal(before, RowsWithChildren(tree).Select(node => node.IsExpanded).ToList());
        }

        [AvaloniaFact]
        public void KeyBindings_OfTheWindow_HoldTheLevelCommands()
        {
            var (mainWindow, viewModel, _) = ShowLevels();
            var tree = viewModel.ProjectTree;

            KeyBinding For(string name) => mainWindow.KeyBindings.Single(binding => Equals(binding.Gesture, viewModel.Shortcuts[name]));

            Assert.Same(tree.ExpandBranchCommand, For("Tree.ExpandBranch").Command);
            Assert.Same(tree.CloseLevelCommand, For("Tree.CloseLevel").Command);
            Assert.Same(tree.OpenLevelCommand, For("Tree.OpenLevel").Command);
            Assert.Same(tree.CollapseAllCommand, For("Tree.CollapseAll").Command);
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
            var (_, window, viewModel) = windows.ShowMainWindow();

            var add = window.KeyBindings.Single(binding => Equals(binding.Gesture, viewModel.Shortcuts["Tree.Add"]));

            Assert.Same(viewModel.ProjectTree.AddAtTargetCommand, add.Command);
        }

        [AvaloniaFact]
        public void Press_ShiftAWithTheFocusOutsideTheTree_OpensTheAddPanelOfTheSelectedRow()
        {
            var (_, window, viewModel) = windows.ShowMainWindow();
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

            window.Press(PhysicalKey.A, RawInputModifiers.Shift);

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
            var (_, window, viewModel) = windows.ShowMainWindow();
            TestServiceProviderFactory.Pump();
            viewModel.ProjectTree.SelectedNode = null;
            var before = viewModel.Project.CompositionManager.ManagerData.Items.Count;
            window.Focus();
            PointBesideTree(window);

            window.Press(PhysicalKey.A, RawInputModifiers.Shift);

            Assert.Equal(before, viewModel.Project.CompositionManager.ManagerData.Items.Count);
            Assert.False(window.GetVisualDescendants().OfType<ListTreeProject>().Single().MenuAddPanel.IsOpen);
        }

        [AvaloniaFact]
        public void Press_ShiftAOverAnEmptyTree_OpensThePanelOfTheProjectAndAddsNothing()
        {
            var (_, window, viewModel) = windows.ShowMainWindow();
            TestServiceProviderFactory.Pump();
            viewModel.ProjectTree.SelectedNode = null;
            var before = viewModel.Project.CompositionManager.ManagerData.Items.Count;
            window.Focus();
            PointOnTree(window);

            window.Press(PhysicalKey.A, RawInputModifiers.Shift);
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
            var (_, window, viewModel) = windows.ShowMainWindow();

            var menu = window.KeyBindings.Single(binding => Equals(binding.Gesture, viewModel.Shortcuts["Tree.Menu"]));

            Assert.Equal(new KeyGesture(Key.W), viewModel.Shortcuts["Tree.Menu"]);
            Assert.Same(viewModel.ProjectTree.OpenMenuAtTargetCommand, menu.Command);
        }

        [AvaloniaFact]
        public void KeyBindings_OfTheWindow_HoldTheExpandCommandOfTheTree()
        {
            var (_, window, viewModel) = windows.ShowMainWindow();

            var expand = window.KeyBindings.Single(binding => Equals(binding.Gesture, viewModel.Shortcuts["Tree.Expand"]));

            Assert.Same(viewModel.ProjectTree.ToggleExpandTargetCommand, expand.Command);
        }

        [AvaloniaFact]
        public void Press_WOverTheTree_OpensTheMenuOfTheSelectedRow()
        {
            var (_, window, viewModel) = windows.ShowMainWindow();
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

            window.Press(PhysicalKey.W, RawInputModifiers.None);
            TestServiceProviderFactory.Pump();

            var (menu, texts) = MenuOf(window);
            Assert.True(menu?.IsOpen);
            Assert.Contains(layer.Header, texts);
            menu!.Close();
        }

        [AvaloniaFact]
        public void Press_WOverTheTreeWithNothingSelected_OpensTheMenuOfTheProject()
        {
            var (_, window, viewModel) = windows.ShowMainWindow();
            TestServiceProviderFactory.Pump();
            viewModel.ProjectTree.SelectedNode = null;
            window.Focus();
            PointOnTree(window);

            window.Press(PhysicalKey.W, RawInputModifiers.None);
            TestServiceProviderFactory.Pump();

            var (menu, texts) = MenuOf(window);
            Assert.True(menu?.IsOpen);
            Assert.Contains("Project", texts);
            menu!.Close();
        }

        [AvaloniaFact]
        public void Press_WBesideTheTree_OpensNoMenu()
        {
            var (_, window, viewModel) = windows.ShowMainWindow();
            TestServiceProviderFactory.Pump();
            window.Focus();
            PointBesideTree(window);

            window.Press(PhysicalKey.W, RawInputModifiers.None);
            TestServiceProviderFactory.Pump();

            Assert.NotEqual(true, MenuOf(window).Menu?.IsOpen);
        }

        // The key is a plain letter: in a name box it types the letter and opens no menu.
        [AvaloniaFact]
        public void Press_WInANameBoxOverTheTree_OpensNoMenu()
        {
            var (_, window, viewModel) = windows.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            TestServiceProviderFactory.Pump();
            var node = viewModel.ProjectTree.CompositionNodes()[0];
            viewModel.ProjectTree.SelectedNode = node;
            TestServiceProviderFactory.Pump();
            PointOnTree(window);
            viewModel.ProjectTree.RenameSelectedCommand.Execute(null);
            TestServiceProviderFactory.Pump();
            Assert.IsAssignableFrom<TextBox>(window.FocusManager!.GetFocusedElement());

            window.Press(PhysicalKey.W, RawInputModifiers.None);
            TestServiceProviderFactory.Pump();

            Assert.NotEqual(true, MenuOf(window).Menu?.IsOpen);
        }

        [AvaloniaFact]
        public void Press_EOverTheTree_CollapsesAndExpandsTheSelectedRow()
        {
            var (_, window, viewModel) = windows.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            TestServiceProviderFactory.Pump();
            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            var node = viewModel.ProjectTree.CompositionNodes()[0];
            viewModel.ProjectTree.SelectedNode = node;
            TestServiceProviderFactory.Pump();
            window.Focus();
            PointOnTree(window);
            Assert.True(node.IsExpanded);

            window.Press(PhysicalKey.E, RawInputModifiers.None);
            Assert.False(node.IsExpanded);

            window.Press(PhysicalKey.E, RawInputModifiers.None);
            Assert.True(node.IsExpanded);
        }

        [AvaloniaFact]
        public void Press_EBesideTheTree_ChangesNothing()
        {
            var (_, window, viewModel) = windows.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            TestServiceProviderFactory.Pump();
            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            var node = viewModel.ProjectTree.CompositionNodes()[0];
            viewModel.ProjectTree.SelectedNode = node;
            TestServiceProviderFactory.Pump();
            window.Focus();
            PointBesideTree(window);

            window.Press(PhysicalKey.E, RawInputModifiers.None);

            Assert.True(node.IsExpanded);
        }

        // E and Shift+A are letters: in a name box they type the letter and do nothing else.
        [AvaloniaFact]
        public void Press_EOrShiftAInANameBoxOverTheTree_DoesNothingElse()
        {
            var (_, window, viewModel) = windows.ShowMainWindow();
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

            window.Press(PhysicalKey.E, RawInputModifiers.None);
            window.Press(PhysicalKey.A, RawInputModifiers.Shift);

            Assert.True(node.IsExpanded);
            Assert.False(window.GetVisualDescendants().OfType<ListTreeProject>().Single().MenuAddPanel.IsOpen);
        }

        [AvaloniaFact]
        public void Set_AnotherGestureForTreeAdd_ReplacesShiftA()
        {
            var (_, window, viewModel) = windows.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            TestServiceProviderFactory.Pump();
            viewModel.ProjectTree.SelectedNode = viewModel.ProjectTree.CompositionNodes()[0];
            viewModel.Shortcuts.Set("Tree.Add", new KeyGesture(Key.N, KeyModifiers.Control));
            TestServiceProviderFactory.Pump();
            window.Focus();
            PointOnTree(window);
            var view = window.GetVisualDescendants().OfType<ListTreeProject>().Single();

            window.Press(PhysicalKey.A, RawInputModifiers.Shift);
            TestServiceProviderFactory.Pump();
            Assert.False(view.MenuAddPanel.IsOpen);

            window.Press(PhysicalKey.N, RawInputModifiers.Control);
            TestServiceProviderFactory.Pump();
            Assert.True(view.MenuAddPanel.IsOpen);
            view.MenuAddPanel.Close();
        }
    }
}
