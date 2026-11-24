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
using static CMiX.Studio.Avalonia.Tests.Support.ListTreeTestScreen;

namespace CMiX.Studio.Avalonia.Tests
{
    // Tree.Hide (H) hides or shows the target row as a click on its eye does. Tree.ShowAll (Alt+H) shows every hidden row as one undo step.
    // Both show a notification with the undo key.
    public class ProjectTreeHideKeysTests : IDisposable
    {
        private Views.MainWindow? window;

        public void Dispose() => window?.Close();

        private sealed record Screen(IServiceProvider Provider, Views.MainWindow Window, MainViewModel ViewModel, ListTreeProject View, PanelNotification Host)
        {
            public ProjectTree Tree => ViewModel.ProjectTree;
            public UndoManager Undo => Provider.GetRequiredService<UndoManager>();
            public TreeNode LayersNode => Tree.CompositionNodes()[0].Children[0];
            public TreeNode LayerNode(int index) => LayersNode.Children[index];

            public int UndoSteps => UndoCounts.StepsOf(Undo);
        }

        // One composition with three layers. Every row is open. The first layer is selected. The undo history is empty.
        private Screen Show()
        {
            var main = ListTreeMainScreen.Show();
            window = main.Window;
            var host = main.Window.GetVisualDescendants().OfType<PanelNotification>().Single();
            var screen = new Screen(main.Provider, main.Window, main.ViewModel, main.View, host);
            screen.Tree.SelectedNode = screen.LayerNode(0);
            Settle(main.Window);
            screen.Undo.Clear();
            return screen;
        }

        private static void Focus(Screen screen, TreeNode node)
        {
            screen.Tree.CursorNode = node;
            Settle(screen.Window);
            screen.View.Rows.RowOf(node)!.Focus();
            Settle(screen.Window);
            Assert.True(screen.View.IsKeyboardFocusWithin);
        }

        private static ToggleButton EyeOf(Screen screen, TreeNode node) =>
            screen.View.Rows.RowOf(node)!.Toggles.GetVisualDescendants().OfType<ToggleButton>()
                .First(button => button.Name is "visibilityToggle" or "groupHideToggle" && button.IsEffectivelyVisible);

        // ---------------------------------------------------------------- Tree.Hide

        [AvaloniaFact]
        public void Press_HAfterDown_HidesTheCursorRowAndNotTheSelectedRow()
        {
            var screen = Show();
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
            var screen = Show();
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
            var screen = Show();
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
            var screen = Show();
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
            var screen = Show();
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
            var screen = Show();
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
            var screen = Show();

            KeyBinding For(string name) => screen.Window.KeyBindings.Single(binding => Equals(binding.Gesture, screen.ViewModel.Shortcuts[name]));

            Assert.Equal(new KeyGesture(Key.H), screen.ViewModel.Shortcuts["Tree.Hide"]);
            Assert.Equal(new KeyGesture(Key.H, KeyModifiers.Alt), screen.ViewModel.Shortcuts["Tree.ShowAll"]);
            Assert.Same(screen.Tree.ToggleHideTargetCommand, For("Tree.Hide").Command);
            Assert.Same(screen.Tree.ShowAllCommand, For("Tree.ShowAll").Command);
        }
    }
}
