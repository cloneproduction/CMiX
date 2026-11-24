using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Settings;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.UI.Popups;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // A remove in the tree shows a notification in the main window: "Removed" and the name, with the key of Edit.Undo as a key cap.
    public class RemoveNotificationTests : IDisposable
    {
        private Views.MainWindow? window;

        public void Dispose() => window?.Close();

        private sealed record Screen(IServiceProvider Provider, Views.MainWindow Window, MainViewModel ViewModel, PanelNotification Host)
        {
            public ProjectTree Tree => ViewModel.ProjectTree;
            public TreeNode LayerNode(int index) => Tree.CompositionNodes()[0].Children[0].Children[index];
        }

        // One composition with two layers. The first layer is selected.
        private Screen Show()
        {
            var main = ListTreeMainScreen.Show(layers: 2);
            window = main.Window;
            var host = main.Window.GetVisualDescendants().OfType<PanelNotification>().Single();
            var screen = new Screen(main.Provider, main.Window, main.ViewModel, host);
            screen.Tree.SelectedNode = screen.LayerNode(0);
            TestServiceProviderFactory.Pump();
            return screen;
        }

        private static string? KeyText(PanelNotification host) =>
            host.Notification.GetVisualDescendants().OfType<TextBlock>().SingleOrDefault(text => text.Name == "PART_KeyText")?.Text;

        [AvaloniaFact]
        public void RemoveItem_SelectedLayer_ShowsTheNameAndTheUndoKey()
        {
            var screen = Show();
            var name = screen.LayerNode(0).Header;

            screen.Tree.RemoveItemCommand.Execute(null);
            TestServiceProviderFactory.Pump();

            Assert.True(screen.Host.IsShowing);
            Assert.Equal($"Removed {name}", screen.Host.Notification.Text);
            Assert.Equal("Ctrl+Z", screen.Host.Notification.UndoGesture);
            Assert.Equal("Ctrl+Z", KeyText(screen.Host));
        }

        [AvaloniaFact]
        public void RemoveItem_RemappedUndo_ShowsTheNewKey()
        {
            var screen = Show();
            screen.Provider.GetRequiredService<ShortcutMap>().Set("Edit.Undo", new KeyGesture(Key.U, KeyModifiers.Control | KeyModifiers.Shift));

            screen.Tree.RemoveItemCommand.Execute(null);
            TestServiceProviderFactory.Pump();

            Assert.Equal("Ctrl+Shift+U", screen.Host.Notification.UndoGesture);
            Assert.Equal("Ctrl+Shift+U", KeyText(screen.Host));
        }

        [AvaloniaFact]
        public void RemoveItem_Twice_TheSecondNotificationReplacesTheFirst()
        {
            var screen = Show();
            var second = screen.LayerNode(1).Header;

            screen.Tree.RemoveItemCommand.Execute(null);
            TestServiceProviderFactory.Pump();
            screen.Tree.SelectedNode = screen.LayerNode(0);
            screen.Tree.RemoveItemCommand.Execute(null);
            TestServiceProviderFactory.Pump();

            Assert.Single(screen.Window.GetVisualDescendants().OfType<Notification>());
            Assert.Equal($"Removed {second}", screen.Host.Notification.Text);
        }

        [AvaloniaFact]
        public void RemoveItem_WithTheFocusInTheWindow_DoesNotMoveTheFocusToTheNotification()
        {
            var screen = Show();
            screen.Window.Focus();
            var focused = screen.Window.FocusManager!.GetFocusedElement();

            screen.Tree.RemoveItemCommand.Execute(null);
            TestServiceProviderFactory.Pump();

            Assert.Same(focused, screen.Window.FocusManager!.GetFocusedElement());
            Assert.False(screen.Host.IsHitTestVisible);
            Assert.False(screen.Host.Notification.Focusable);
        }
    }
}
