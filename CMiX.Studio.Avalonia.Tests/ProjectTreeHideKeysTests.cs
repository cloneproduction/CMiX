using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
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
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;
using static CMiX.Studio.Avalonia.Tests.Support.TreeTestScreen;

namespace CMiX.Studio.Avalonia.Tests
{
    // Tree.Hide (H) hides or shows the target row as a click on its eye does. Tree.ShowAll (Alt+H) shows every hidden row as one undo step.
    // Both show a notification with the undo key.
    public class ProjectTreeHideKeysTests : IDisposable
    {
        private Views.MainWindow? window;

        public void Dispose() => window?.Close();

        private sealed class MessageRecorder : IMessageSender
        {
            public List<IMessage> Sent { get; } = new();
            public void SendMessage(IMessage message) => Sent.Add(message);
        }

        private sealed record Screen(IServiceProvider Provider, Views.MainWindow Window, MainViewModel ViewModel, TreeViewProject View, PanelNotification Host)
        {
            public ProjectTree Tree => ViewModel.ProjectTree;
            public UndoManager Undo => Provider.GetRequiredService<UndoManager>();
            public TreeNode LayersNode => Tree.CompositionNodes()[0].Children[0];
            public TreeNode LayerNode(int index) => LayersNode.Children[index];

            public LinkedList<IUndoCommand> UndoStack =>
                (LinkedList<IUndoCommand>)typeof(UndoManager).GetField("_undoStack", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(Undo)!;

            public int UndoSteps => UndoStack.Count;
        }

        // One composition with three layers. Every row is open. The first layer is selected. The undo history is empty.
        private Screen Show()
        {
            var (provider, mainWindow, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            window = mainWindow;
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.ManagerData.Items[0];
            for (var i = 0; i < 3; i++)
                composition.LayerManager.AddItem(typeof(Layer));
            TestServiceProviderFactory.Pump();
            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            TestServiceProviderFactory.Pump();
            var view = mainWindow.GetVisualDescendants().OfType<TreeViewProject>().Single();
            var host = mainWindow.GetVisualDescendants().OfType<PanelNotification>().Single();
            var screen = new Screen(provider, mainWindow, viewModel, view, host);
            screen.Tree.SelectedNode = screen.LayerNode(0);
            Settle(mainWindow);
            screen.Undo.Clear();
            return screen;
        }

        private static void Focus(Screen screen, TreeNode node)
        {
            screen.Tree.CursorNode = node;
            Settle(screen.Window);
            ItemOf(screen.View, node).Focus();
            Settle(screen.Window);
            Assert.True(screen.View.IsKeyboardFocusWithin);
        }

        private static void Press(Screen screen, PhysicalKey key, RawInputModifiers modifiers = RawInputModifiers.None)
        {
            screen.Window.KeyPressQwerty(key, modifiers);
            Settle(screen.Window);
        }

        private static ToggleButton EyeOf(Screen screen, TreeNode node) =>
            ItemOf(screen.View, node).GetVisualDescendants().OfType<ToggleButton>()
                .First(button => button.Name is "visibilityToggle" or "groupHideToggle" && button.IsEffectivelyVisible
                    && button.FindAncestorOfType<TreeViewItem>() == ItemOf(screen.View, node));

        private static void Click(Screen screen, Control control)
        {
            var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), screen.Window)!.Value;
            screen.Window.MouseDown(point, MouseButton.Left);
            screen.Window.MouseUp(point, MouseButton.Left);
            Settle(screen.Window);
        }

        // ---------------------------------------------------------------- Tree.Hide

        [AvaloniaFact]
        public void Press_HAfterDown_HidesTheCursorRowAndNotTheSelectedRow()
        {
            var screen = Show();
            screen.LayerNode(0).IsExpanded = false;
            Focus(screen, screen.LayerNode(0));

            Press(screen, PhysicalKey.ArrowDown);
            Assert.Same(screen.LayerNode(1), screen.Tree.CursorNode);
            Press(screen, PhysicalKey.H);

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
            var screen = Show();
            var group = screen.LayersNode;
            Focus(screen, group);

            Press(screen, PhysicalKey.H);

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
            var screen = Show();
            var messenger = screen.Provider.GetRequiredService<ControlMessenger>();
            messenger.IsSendingBlocked = false;
            var recorder = new MessageRecorder();
            messenger.Register(recorder);
            var clicked = screen.LayerNode(1);
            var pressed = screen.LayerNode(2);

            Click(screen, EyeOf(screen, clicked));
            var clickStep = screen.UndoStack.Last!.Value;
            var clickMessage = recorder.Sent.Last();
            Focus(screen, pressed);
            Press(screen, PhysicalKey.H);
            var keyStep = screen.UndoStack.Last!.Value;
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
            var screen = Show();
            var node = screen.LayerNode(1);
            Focus(screen, node);

            Press(screen, PhysicalKey.H);

            Assert.True(screen.Host.IsShowing);
            Assert.Equal($"Hidden: {node.Header}", screen.Host.Notification.Text);
            Assert.Equal("Ctrl+Z", screen.Host.Notification.UndoGesture);

            Press(screen, PhysicalKey.H);

            Assert.False(node.IsHidden);
            Assert.Equal($"Shown: {node.Header}", screen.Host.Notification.Text);
            Assert.Equal("Ctrl+Z", screen.Host.Notification.UndoGesture);
        }

        // ---------------------------------------------------------------- Tree.ShowAll

        [AvaloniaFact]
        public void Press_AltH_ShowsThreeHiddenRowsInOneUndoStep()
        {
            var screen = Show();
            var group = screen.LayerNode(0).Children[0];
            group.Group!.Manager.Hidden.Value = true;
            screen.LayerNode(1).HiddenValue!.Value = true;
            screen.LayerNode(2).HiddenValue!.Value = true;
            // A hidden row under a closed row also shows again.
            screen.LayerNode(0).IsExpanded = false;
            Settle(screen.Window);
            var before = screen.UndoSteps;
            Focus(screen, screen.LayerNode(1));

            Press(screen, PhysicalKey.H, RawInputModifiers.Alt);

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
            var screen = Show();
            Focus(screen, screen.LayerNode(1));

            Press(screen, PhysicalKey.H, RawInputModifiers.Alt);

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
            var screen = Show();
            screen.LayerNode(2).HiddenValue!.Value = true;
            screen.Tree.CursorNode = screen.LayerNode(1);
            Settle(screen.Window);
            var box = screen.Window.GetVisualDescendants().OfType<TextBoxSearchTree>().First().GetVisualDescendants().OfType<TextBox>().First();
            box.Focus();
            Settle(screen.Window);
            Assert.True(box.IsFocused);
            var before = screen.UndoSteps;

            Press(screen, PhysicalKey.H);
            Press(screen, PhysicalKey.H, RawInputModifiers.Alt);

            Assert.False(screen.LayerNode(1).IsHidden);
            Assert.True(screen.LayerNode(2).IsHidden);
            Assert.Equal(before, screen.UndoSteps);
        }

        [AvaloniaFact]
        public void KeyBindings_OfTheWindow_HoldTheHideCommands()
        {
            var screen = Show();

            KeyBinding For(string name) => screen.Window.KeyBindings.Single(binding => Equals(binding.Gesture, screen.ViewModel.Shortcuts[name]));

            Assert.Equal(new KeyGesture(Key.H), screen.ViewModel.Shortcuts["Tree.Hide"]);
            Assert.Equal(new KeyGesture(Key.H, KeyModifiers.Alt), screen.ViewModel.Shortcuts["Tree.ShowAll"]);
            Assert.Same(screen.Tree.ToggleHideTargetCommand, For("Tree.Hide").Command);
            Assert.Same(screen.Tree.ShowAllCommand, For("Tree.ShowAll").Command);
        }
    }
}
