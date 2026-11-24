using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // The undo and redo keys of the main window. The gestures come from the shortcut map.
    public class MainWindowShortcutTests
    {
        [AvaloniaFact]
        public void Press_CtrlZThenCtrlShiftZ_UndoesAndRedoesTheStep()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            var items = viewModel.Project.CompositionManager.ManagerData.Items;
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            TestServiceProviderFactory.Pump();
            window.Focus();
            Assert.Single(items);

            window.Press(PhysicalKey.Z, RawInputModifiers.Control);
            Assert.Empty(items);

            window.Press(PhysicalKey.Z, RawInputModifiers.Control | RawInputModifiers.Shift);
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

            window.Press(PhysicalKey.AltLeft, RawInputModifiers.None);

            // Avalonia shows the access keys at the press of Alt, and opens the menu at its release when they show.
            Assert.False(window.GetValue(global::Avalonia.Controls.Primitives.AccessText.ShowAccessKeyProperty));
            Assert.False(file.IsSelected);
            Assert.False(file.IsSubMenuOpen);
            Assert.False(menu.IsKeyboardFocusWithin);
        }
    }
}
