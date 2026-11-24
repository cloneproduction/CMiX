// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Threading;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;

namespace CMiX.Studio.Avalonia.Tests.Support
{
    // A headless window that shows the list tree. The rows move with no animation.
    internal sealed record ListTreeTestScreen(Window Window, ListTreeProject View)
    {
        public static ListTreeTestScreen Show(ProjectTree tree, double width, double height)
        {
            var view = new ListTreeProject { DataContext = tree };
            view.Rows.MoveDuration = TimeSpan.Zero;
            var window = new Window { Content = view, Width = width, Height = height };
            window.Show();
            Settle(window);
            return new ListTreeTestScreen(window, view);
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

        // The nodes and all the nodes under them, in tree order.
        public static IEnumerable<TreeNode> All(IEnumerable<TreeNode> nodes) =>
            nodes.SelectMany(node => new[] { node }.Concat(All(node.Children)));
    }
}
