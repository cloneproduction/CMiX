// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;

namespace CMiX.Studio.Avalonia.Tests.Support
{
    // A headless window that shows the new list tree. The rows move with no animation.
    internal sealed record ListTreeTestScreen(Window Window, ListTreeProject View)
    {
        public static ListTreeTestScreen Show(ProjectTree tree, double width, double height)
        {
            var view = new ListTreeProject { DataContext = tree };
            view.Rows.MoveDuration = TimeSpan.Zero;
            var window = new Window { Content = view, Width = width, Height = height };
            window.Show();
            TreeTestScreen.Settle(window);
            return new ListTreeTestScreen(window, view);
        }
    }
}
