// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Themes;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls;
using CMiX.UI.Controls.String;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests.Support
{
    // The reads of the look of the list tree: parts, colors and theme tokens.
    internal static class ListTreeLook
    {
        public static Border Named(Control root, string name) => root.GetVisualDescendants().OfType<Border>().First(border => border.Name == name);

        // ColorOf fails on a brush that is not solid. FillOf gives Transparent for it, because a fill can be empty.
        public static Color ColorOf(IBrush? brush) => Assert.IsAssignableFrom<ISolidColorBrush>(brush).Color;

        public static Color FillOf(IBrush? brush) => brush is ISolidColorBrush solid ? solid.Color : Colors.Transparent;

        public static Color Token(string key) => ColorOf((IBrush)Application.Current!.FindResource(key)!);

        public static double TokenSize(string key) => (double)Application.Current!.FindResource(key)!;

        public static Thickness TokenThickness(string key) => (Thickness)Application.Current!.FindResource(key)!;

        public static Color IconOf(RowListTree row) =>
            ColorOf(row.GetVisualDescendants().OfType<AppIcon>().First(icon => icon.Name == "kindIcon").Foreground);

        public static Control NameOf(RowListTree row) =>
            (Control?)row.GetVisualDescendants().OfType<TextBoxEditable>().FirstOrDefault(box => box.IsVisible)
            ?? row.GetVisualDescendants().OfType<TextBlock>().First(text => text.Name == "groupName");

        public static Color TextOf(RowListTree row) =>
            NameOf(row) is TextBoxEditable box ? ColorOf(box.Foreground) : ColorOf(((TextBlock)NameOf(row)).Foreground);

        public static ListTreeKindColors KindOf(TreeNode node) => CategoryPalette.FromApplication().ForListTree(node.Category!.Value, node.ComposableDepth);
    }
}
