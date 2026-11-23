using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Themes;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Panels;

namespace CMiX.Studio.Avalonia.Tests.Support
{
    // A headless window that shows a project tree, and the parts of its rows.
    internal sealed class TreeTestScreen : IDisposable
    {
        private TreeTestScreen(Window window, TreeViewProject view)
        {
            Window = window;
            View = view;
        }

        public Window Window { get; }
        public TreeViewProject View { get; }

        // Shows the tree in a new window.
        // scale sets the render scaling. instant stops the expand animation. settle runs Settle after the show.
        public static TreeTestScreen Show(ProjectTree tree, double width, double height, double? scale = null, bool instant = true, bool settle = true) =>
            Show(new TreeViewProject { DataContext = tree }, width, height, scale, instant, settle);

        public static TreeTestScreen Show(TreeViewProject view, double width, double height, double? scale = null, bool instant = true, bool settle = true)
        {
            var window = new Window { Content = view, Width = width, Height = height };
            window.Show();
            if (scale is { } value)
                HeadlessScaling.Set(window, value);
            if (instant)
            {
                foreach (var panel in view.GetVisualDescendants().OfType<PanelCollapse>())
                    panel.Duration = TimeSpan.Zero;
            }

            var screen = new TreeTestScreen(window, view);
            if (settle)
                screen.Settle();
            return screen;
        }

        public void Settle() => Settle(Window);

        // Runs the jobs, lets the animations end, runs the layout and renders.
        public static void Settle(Window window)
        {
            Dispatcher.UIThread.RunJobs();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick(60);
            window.UpdateLayout();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            Dispatcher.UIThread.RunJobs();
        }

        // Renders the window, so that the hit test sees the last change.
        public void Render() => TreeDrag.Render(Window);

        public void Dispose() => Window.Close();

        public static TreeViewItem ItemOf(TreeViewProject view, TreeNode node) =>
            view.GetVisualDescendants().OfType<TreeViewItem>().First(item => ReferenceEquals(item.DataContext, node));

        // The first border with this name in the row of the node.
        public static Border Part(TreeViewProject view, TreeNode node, string name) =>
            ItemOf(view, node).GetVisualDescendants().OfType<Border>().First(border => border.Name == name);

        // The colored border of the row of the node.
        public static Border RowOf(TreeViewProject view, TreeNode node) => Part(view, node, "RowBorder");

        public static Color FillOf(Border border) => ((ISolidColorBrush)border.Background!).Color;

        public static Color FillOf(TreeViewProject view, TreeNode node) => FillOf(RowOf(view, node));

        // The fill that the theme gives to the row of the node for a role: the tone of its kind, or a grey for a group with no children.
        public static Color ExpectedFill(TreeNode node, ToneRole role)
        {
            if (node.Item != null || node.Children.Count > 0)
                return CategoryPalette.FromApplication().For(node.Category!.Value, node.ComposableDepth, node.KindDepth).Of(role);

            var key = role switch
            {
                ToneRole.Fill => "Grey300Brush",
                ToneRole.FillHover => "Grey400Brush",
                _ => "Grey500Brush"
            };
            return ((ISolidColorBrush)Application.Current!.FindResource(key)!).Color;
        }

        // The nodes and all the nodes under them, in tree order.
        public static IEnumerable<TreeNode> All(IEnumerable<TreeNode> nodes) =>
            nodes.SelectMany(node => new[] { node }.Concat(All(node.Children)));
    }
}
