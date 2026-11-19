// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    internal static class TreeRows
    {
        public const string RowBorderName = "RowBorder";

        public static TreeViewItem? ItemOf(object? source) =>
            (source as Visual)?.FindAncestorOfType<TreeViewItem>(includeSelf: true);

        // The source itself counts.
        public static bool HasAncestor<T>(object? source) where T : Visual =>
            (source as Visual)?.FindAncestorOfType<T>(includeSelf: true) != null;

        public static TreeViewItem? ItemOfNode(TreeViewItem from, TreeNode node)
        {
            for (var item = from; item != null; item = item.FindAncestorOfType<TreeViewItem>())
                if (ReferenceEquals(item.DataContext, node))
                    return item;

            return null;
        }

        // Without the rows under it.
        public static Border? RowBorderOf(TreeViewItem item) =>
            item.GetVisualDescendants().OfType<Border>().FirstOrDefault(border => border.Name == RowBorderName);

        public static int WheelSteps(PointerWheelEventArgs e)
        {
            var steps = (int)Math.Round(e.Delta.Y);
            return steps != 0 ? steps : Math.Sign(e.Delta.Y);
        }

        public static double? RowTopIn(TreeViewItem item, ScrollViewer scroller) =>
            RowBorderOf(item) is { } row ? row.TranslatePoint(new Point(0, 0), scroller)?.Y : null;
    }
}
