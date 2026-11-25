// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using CMiX.Studio.Avalonia.Themes;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Converters
{
    // The kind colour of a node of the list tree for a mark, the name of a ListTreeKindColors member as the parameter.
    public sealed class ListTreeKindBrushConverter : IValueConverter
    {
        public const string HoverBorder = nameof(ListTreeKindColors.HoverBorder);
        public const string SelectionBorder = nameof(ListTreeKindColors.SelectionBorder);
        public const string DropOutline = nameof(ListTreeKindColors.DropOutline);
        public const string Inspected = nameof(ListTreeKindColors.Inspected);
        public const string InspectedHover = nameof(ListTreeKindColors.InspectedHover);

        private CategoryPalette? palette;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not TreeNode { Category: { } category } node)
                return Brushes.Transparent;

            palette ??= CategoryPalette.FromApplication();
            var colors = palette.ForListTree(category, node.ComposableDepth);
            return parameter switch
            {
                HoverBorder => new ImmutableSolidColorBrush(colors.HoverBorder),
                SelectionBorder => new ImmutableSolidColorBrush(colors.SelectionBorder),
                DropOutline => new ImmutableSolidColorBrush(colors.DropOutline),
                Inspected => new ImmutableSolidColorBrush(colors.Inspected),
                InspectedHover => new ImmutableSolidColorBrush(colors.InspectedHover),
                _ => Brushes.Transparent
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
