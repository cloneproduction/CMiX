// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace CMiX.UI.Controls.Collection
{
    // The input of the lists that offer items to pick. A click on an item runs ItemCommand with the item.
    // TextPath is the path of the text of an item, such as PrefabService.Name.Value. Empty gives the ToString of the item.
    public class ListItemPickerBase : UserControl
    {
        public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
            AvaloniaProperty.Register<ListItemPickerBase, IEnumerable?>(nameof(ItemsSource));

        public IEnumerable? ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly StyledProperty<ICommand?> ItemCommandProperty =
            AvaloniaProperty.Register<ListItemPickerBase, ICommand?>(nameof(ItemCommand));

        public ICommand? ItemCommand
        {
            get => GetValue(ItemCommandProperty);
            set => SetValue(ItemCommandProperty, value);
        }

        public static readonly StyledProperty<string> TextPathProperty =
            AvaloniaProperty.Register<ListItemPickerBase, string>(nameof(TextPath), string.Empty);

        public string TextPath
        {
            get => GetValue(TextPathProperty);
            set => SetValue(TextPathProperty, value);
        }

        // The path of the icon key of an item. Empty, the default, gives rows with the text only.
        public static readonly StyledProperty<string> IconPathProperty =
            AvaloniaProperty.Register<ListItemPickerBase, string>(nameof(IconPath), string.Empty);

        public string IconPath
        {
            get => GetValue(IconPathProperty);
            set => SetValue(IconPathProperty, value);
        }

        // Raised after ItemCommand ran for an item, by a click or by Enter. The host can close its popup then.
        public event EventHandler? Picked;

        protected void RaisePicked() => Picked?.Invoke(this, EventArgs.Empty);
    }
}
