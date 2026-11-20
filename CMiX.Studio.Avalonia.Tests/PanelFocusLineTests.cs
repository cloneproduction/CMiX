using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeTestScreen;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // The panel with the keyboard focus shows a 2 px accent line under its header. The line stays on while a popup of the panel is open.
    public class PanelFocusLineTests : IDisposable
    {
        private sealed record Screen(Views.MainWindow Window, MainViewModel ViewModel, PanelSide Left, PanelSide Right)
        {
            public ProjectTree Tree => ViewModel.ProjectTree;
            public TreeViewProject View => Left.GetVisualDescendants().OfType<TreeViewProject>().Single();
            public TreeView TreeView => View.GetVisualDescendants().OfType<TreeView>().First();
            public TreeNode LayerNode => Tree.CompositionNodes()[0].Children[0].Children[0];
        }

        private Screen? screen;

        public void Dispose()
        {
            if (screen == null)
                return;

            screen.TreeView.ContextMenu?.Close();
            screen.View.MenuAddPanel.Close();
            screen.Window.Close();
            Dispatcher.UIThread.RunJobs();
        }

        // The tree on the left, the tabs on the right. One composition with one layer. The focus is on the layer row.
        private Screen Show()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.ManagerData.Items[0];
            composition.LayerManager.AddItem(typeof(Layer));
            TestServiceProviderFactory.Pump();
            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            TestServiceProviderFactory.Pump();

            var hosts = window.GetVisualDescendants().OfType<PanelSide>()
                .OrderBy(host => host.TranslatePoint(default, window)!.Value.X).ToList();
            screen = new Screen(window, viewModel, hosts[0], hosts[1]);
            screen.Tree.SelectedNode = screen.LayerNode;
            Settle(window);
            ItemOf(screen.View, screen.LayerNode).Focus();
            Settle(window);
            return screen;
        }

        private static Border LineOf(PanelSide host) => host.FindControl<Border>("focusLine")!;

        private static void Press(Screen screen, PhysicalKey key, RawInputModifiers modifiers = RawInputModifiers.None)
        {
            screen.Window.KeyPressQwerty(key, modifiers);
            screen.Window.KeyReleaseQwerty(key, modifiers);
            Settle(screen.Window);
        }

        // The line is shown, as thick as SplitterIndicatorSize, in AccentBrush, and at the bottom of the bar.
        private static void AssertLineOn(PanelSide host)
        {
            var line = LineOf(host);
            var bar = host.FindControl<Border>("bar")!;
            Assert.True(line.IsEffectivelyVisible);
            Assert.Equal((double)host.FindResource("SplitterIndicatorSize")!, line.Bounds.Height);
            Assert.Equal(2, line.Bounds.Height);
            Assert.Same(host.FindResource("AccentBrush"), line.Background);
            Assert.Equal(((ISolidColorBrush)host.FindResource("AccentBrush")!).Color, ((ISolidColorBrush)line.Background!).Color);
            Assert.Equal(bar.TranslatePoint(new Point(0, bar.Bounds.Height), host)!.Value.Y, line.TranslatePoint(new Point(0, line.Bounds.Height), host)!.Value.Y);
            Assert.Equal(bar.Bounds.Width, line.Bounds.Width);
        }

        private static void AssertLineOff(PanelSide host) => Assert.False(LineOf(host).IsEffectivelyVisible);

        [AvaloniaFact]
        public void FocusLine_FocusInTree_ShowsTheLineOfTheTreePanelOnly()
        {
            var screen = Show();

            AssertLineOn(screen.Left);
            AssertLineOff(screen.Right);
        }

        [AvaloniaFact]
        public void FocusLine_FocusInOtherPanel_MovesTheLineToThatPanel()
        {
            var screen = Show();

            screen.Right.FindControl<ToggleButton>("pickerToggle")!.Focus();
            Settle(screen.Window);

            AssertLineOff(screen.Left);
            AssertLineOn(screen.Right);
        }

        [AvaloniaFact]
        public void FocusLine_TreeContextMenuOpen_KeepsTheLineOfTheTreePanel()
        {
            var screen = Show();

            Press(screen, PhysicalKey.W);

            Assert.True(screen.TreeView.ContextMenu?.IsOpen);
            AssertLineOn(screen.Left);
            AssertLineOff(screen.Right);

            screen.TreeView.ContextMenu!.Close();
            screen.Right.FindControl<ToggleButton>("pickerToggle")!.Focus();
            Settle(screen.Window);

            AssertLineOff(screen.Left);
            AssertLineOn(screen.Right);
        }

        [AvaloniaFact]
        public void FocusLine_AddPanelOpen_KeepsTheLineOfTheTreePanel()
        {
            var screen = Show();

            Press(screen, PhysicalKey.A, RawInputModifiers.Shift);

            Assert.True(screen.View.MenuAddPanel.IsOpen);
            AssertLineOn(screen.Left);
            AssertLineOff(screen.Right);

            screen.View.MenuAddPanel.Close();
            screen.Right.FindControl<ToggleButton>("pickerToggle")!.Focus();
            Settle(screen.Window);

            AssertLineOff(screen.Left);
        }
    }
}
