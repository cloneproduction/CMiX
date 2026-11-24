// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using Avalonia.Layout;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;

namespace CMiX.Studio.Avalonia.Tests.Support
{
    // A headless window that shows the list tree. The rows move with no animation.
    internal sealed record ListTreeTestScreen(Window Window, ListTreeProject View)
    {
        // The controls below go under the tree, the first one at the bottom.
        public static ListTreeTestScreen Show(ProjectTree tree, double width, double height, params Control[] below)
        {
            var view = NewView(tree);
            if (below.Length == 0)
                return new ListTreeTestScreen(ShowWindow(view, width, height), view);

            var panel = new DockPanel();
            foreach (var control in below)
            {
                DockPanel.SetDock(control, Dock.Bottom);
                panel.Children.Add(control);
            }
            panel.Children.Add(view);
            return new ListTreeTestScreen(ShowWindow(panel, width, height), view);
        }

        // The search box over the tree, as in the side panel.
        public static (ListTreeTestScreen Screen, TextBoxSearchTree Box) ShowWithSearch(ProjectTree tree, double width, double height)
        {
            var box = new TextBoxSearchTree { DataContext = tree };
            var view = NewView(tree);
            DockPanel.SetDock(box, Dock.Top);
            var window = ShowWindow(new DockPanel { Children = { box, view } }, width, height);
            return (new ListTreeTestScreen(window, view), box);
        }

        // Two views of one tree side by side, each with its own height.
        public static (Window Window, ListTreeProject First, ListTreeProject Second) ShowTwoViews(ProjectTree tree,
            double firstHeight = 600, double secondHeight = 600)
        {
            var first = NewView(tree);
            var second = NewView(tree);
            first.Height = firstHeight;
            second.Height = secondHeight;
            first.VerticalAlignment = VerticalAlignment.Top;
            second.VerticalAlignment = VerticalAlignment.Top;
            var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*") };
            Grid.SetColumn(second, 1);
            grid.Children.Add(first);
            grid.Children.Add(second);
            return (ShowWindow(grid, 800, 900), first, second);
        }

        public void Settle() => Settle(Window);

        public static void Settle(Window window) => window.Settle();

        // The nodes and all the nodes under them, in tree order.
        public static IEnumerable<TreeNode> All(IEnumerable<TreeNode> nodes) =>
            nodes.SelectMany(node => new[] { node }.Concat(All(node.Children)));

        private static ListTreeProject NewView(ProjectTree tree)
        {
            var view = new ListTreeProject { DataContext = tree };
            view.Rows.MoveDuration = TimeSpan.Zero;
            return view;
        }

        private static Window ShowWindow(Control content, double width, double height)
        {
            var window = new Window { Content = content, Width = width, Height = height };
            window.Show();
            window.Settle();
            return window;
        }
    }
}
