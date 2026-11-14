// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;
using CMiX.Studio.Avalonia.Themes;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Converters
{
    // The brush of the icon of a row of the tree. The values are the node, IsCursor of the node and IsSelected of the item.
    // A selected row and the cursor row show the icon white. Any other row shows it in the colour of its kind. With the parameter
    // Text the brush is for the name of the row: white on the same rows, a step dimmer (TreeTextRest) on the others. The fourth
    // value is the count of items of the row: a group with nothing inside looks dead, icon and name in grey, like its fill. The colour is a
    // local value: the name (TextBoxEditable) and the icon (AppIcon) have styles of their own that a style of the tree can not beat. The colour is a local value of the icon: AppIcon has a style of
    // its own for its Foreground, which a style of the tree can not beat.
    public sealed class KindIconBrushConverter : IMultiValueConverter
    {
        private readonly TreeToneConverter tone = new();

        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            var isCursor = values.Count > 1 && values[1] is true;
            var isSelected = values.Count > 2 && values[2] is true;
            var isEmptyGroup = values[0] is TreeNode { Item: null } && values.Count > 3 && values[3] is 0;
            if (isSelected || isCursor)
                return Resource("ActiveForegroundBrush");
            if (isEmptyGroup)
                return Resource("Grey700Brush");
            if (parameter is "Text")
                return Resource("TreeTextRestBrush");
            return tone.Convert(values[0], targetType, nameof(ToneRole.KindSelected), culture);
        }

        private static object? Resource(string key) =>
            Application.Current != null && Application.Current.TryFindResource(key, out var value) ? value : Brushes.Transparent;
    }
}
