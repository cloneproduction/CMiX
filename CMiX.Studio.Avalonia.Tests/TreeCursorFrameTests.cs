using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.AttachedProperties;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeTestScreen;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // The cursor row of the tree has the class cursor and shows CursorFrame: a thick bright frame while the tree has the focus,
    // a thin grey frame when it does not.
    public class TreeCursorFrameTests : IDisposable
    {
        private Views.MainWindow? window;

        public void Dispose() => window?.Close();

        private sealed record Screen(Views.MainWindow Window, MainViewModel ViewModel, ProjectTreeView View)
        {
            public ProjectTree Tree => ViewModel.ProjectTree;
            public TreeNode LayerNode(int index) => Tree.CompositionNodes()[0].Children[0].Children[index];
        }

        // One composition with two layers. The first layer is selected, and its row has the focus.
        private Screen Show()
        {
            var (_, mainWindow, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            window = mainWindow;
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.ManagerData.Items[0];
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
            return screen;
        }

        private static Color Brush(string key) => ((ISolidColorBrush)Application.Current!.FindResource(key)!).Color;

        private static Thickness Thickness(string key) => (Thickness)Application.Current!.FindResource(key)!;

        private static TreeNode NextRow(Screen screen, TreeNode node)
        {
            var rows = screen.Tree.VisibleNodes().ToList();
            return rows[rows.IndexOf(node) + 1];
        }

        // The toggles of a row at rest have the colour of its text (TreeTextRest), icon included: the icon takes the Foreground of the toggle.
        [AvaloniaFact]
        public void Row_AtRest_ToggleIconsHaveTheTextColour()
        {
            var screen = Show();
            var node = screen.LayerNode(1);
            var item = screen.View.GetVisualDescendants().OfType<TreeViewItem>().First(row => ReferenceEquals(row.DataContext, node));
            var toggle = item.GetVisualDescendants().OfType<global::Avalonia.Controls.Primitives.ToggleButton>().First(t => t.Name == "visibilityToggle");
            var icon = toggle.GetVisualDescendants().OfType<CMiX.UI.Controls.AppIcon>().FirstOrDefault();
            Assert.Equal(Brush("TreeTextRestBrush"), ((ISolidColorBrush)toggle.Foreground!).Color);
            Assert.Equal(Brush("TreeTextRestBrush"), ((ISolidColorBrush)icon!.Foreground!).Color);
        }

        // The tooltip of a toggle of a row has the colour of the row at the strength of a hover (ToolTipCard.Fill, inherited across the popup).
        [AvaloniaFact]
        public void ToolTip_OfAToggleOfARow_HasTheColourOfTheRow()
        {
            var screen = Show();
            var node = screen.LayerNode(1);
            var item = screen.View.GetVisualDescendants().OfType<TreeViewItem>().First(row => ReferenceEquals(row.DataContext, node));
            var toggle = item.GetVisualDescendants().OfType<global::Avalonia.Controls.Primitives.ToggleButton>().First(t => t.Name == "visibilityToggle");
            ToolTip.SetIsOpen(toggle, true);
            Settle(screen.Window);
            var property = typeof(ToolTip).GetField("ToolTipProperty", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)?.GetValue(null) as AvaloniaProperty;
            var tip = property != null ? toggle.GetValue(property) as ToolTip : toggle.GetLogicalChildren().OfType<ToolTip>().FirstOrDefault();
            var open = ToolTip.GetIsOpen(toggle);
            var card = tip?.GetVisualDescendants().OfType<CMiX.UI.Controls.Parts.ToolTipCard>().FirstOrDefault();
            var expected = (ISolidColorBrush)new CMiX.Studio.Avalonia.Converters.TreeToneConverter().Convert(node, typeof(IBrush), "FillHover", System.Globalization.CultureInfo.InvariantCulture)!;
            Assert.Equal(expected.Color, ((ISolidColorBrush)card!.Background!).Color);
        }

        [AvaloniaFact]
        public void IsCursor_AfterDown_FollowsTheCursorNode()
        {
            var screen = Show();
            var first = screen.LayerNode(0);
            var next = NextRow(screen, first);

            screen.Window.KeyPressQwerty(PhysicalKey.ArrowDown, RawInputModifiers.None);
            Settle(screen.Window);

            Assert.Same(next, screen.Tree.CursorNode);
            Assert.True(next.IsCursor);
            Assert.False(first.IsCursor);
            Assert.Contains(CursorRow.CursorPseudoClass, ItemOf(screen.View, next).Classes);
            Assert.DoesNotContain(CursorRow.CursorPseudoClass, ItemOf(screen.View, first).Classes);
        }

        [AvaloniaFact]
        // The cursor row of the tree is framed in the colour of its kind (TreeTone KindSelected), not in white or grey, with the focus and
        // without it. On the selected row the frame is white (the fill is the colour of the kind), so the cursor moves off it first.
        public void CursorFrame_CursorRowOfTheTree_IsTheThinFrameInTheColourOfTheKind()
        {
            var screen = Show();
            screen.Window.KeyPressQwerty(PhysicalKey.ArrowDown, RawInputModifiers.None);
            Settle(screen.Window);
            var node = screen.Tree.CursorNode!;
            Assert.NotSame(screen.Tree.SelectedNode, node);

            var frame = Part(screen.View, node, "CursorFrame");

            Assert.True(frame.IsVisible);
            Assert.Equal((Thickness)screen.View.FindResource("TreeSelectionThickness")!, frame.BorderThickness);
            var colour = ((ISolidColorBrush)frame.BorderBrush!).Color;
            Assert.NotEqual(Brush("CursorFrameBrush"), colour);
            Assert.NotEqual(Brush("CursorFrameInactiveBrush"), colour);
        }

        [AvaloniaFact]
        public void CursorFrame_RowThatIsNotTheCursor_IsHidden()
        {
            var screen = Show();
            var other = NextRow(screen, screen.Tree.CursorNode!);

            Assert.False(Part(screen.View, other, "CursorFrame").IsVisible);
        }
    }
}
