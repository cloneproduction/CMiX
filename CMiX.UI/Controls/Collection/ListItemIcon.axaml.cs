// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace CMiX.UI.Controls.Collection
{
    // A row of a list: an icon, a name that can be edited, and a toggle on the right (an eye).
    // The application sets the properties. The row knows nothing about the model.
    public partial class ListItemIcon : UserControl
    {
        public ListItemIcon()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<string> IconProperty =
            AvaloniaProperty.Register<ListItemIcon, string>(nameof(Icon));

        // The key of the icon, see Icons.
        public string Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly StyledProperty<string?> TitleProperty =
            AvaloniaProperty.Register<ListItemIcon, string?>(nameof(Title), defaultBindingMode: BindingMode.TwoWay);

        public string? Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly StyledProperty<bool> IsCheckedProperty =
            AvaloniaProperty.Register<ListItemIcon, bool>(nameof(IsChecked), false, defaultBindingMode: BindingMode.TwoWay);

        // The toggle on the right. The default theme shows an eye.
        public bool IsChecked
        {
            get => GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public static readonly StyledProperty<bool?> IsUpdatingProperty =
            AvaloniaProperty.Register<ListItemIcon, bool?>(nameof(IsUpdating), null, defaultBindingMode: BindingMode.TwoWay);

        // The power toggle. It is hidden while this property has no value.
        public bool? IsUpdating
        {
            get => GetValue(IsUpdatingProperty);
            set => SetValue(IsUpdatingProperty, value);
        }
    }
}
