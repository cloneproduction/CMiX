// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using CMiX.UI.AttachedProperties;

namespace CMiX.UI.Controls.Collection
{
    // A list, a delete button and an add area under it, and a detail of the selected item under that.
    // The application sets the row templates in DataTemplates, and fills AddContent and DetailContent.
    // The DataContext of DetailContent is the selected item. It shows only while an item is selected.
    public partial class ListBoxDetail : UserControl
    {
        static ListBoxDetail()
        {
            LogicalPanelContent.Track<ListBoxDetail>(AddContentProperty);
            LogicalPanelContent.Track<ListBoxDetail>(DetailContentProperty);
        }

        public ListBoxDetail()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
            AvaloniaProperty.Register<ListBoxDetail, IEnumerable?>(nameof(ItemsSource));

        public IEnumerable? ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly StyledProperty<object?> SelectedItemProperty =
            AvaloniaProperty.Register<ListBoxDetail, object?>(nameof(SelectedItem), defaultBindingMode: BindingMode.TwoWay);

        public object? SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        // Runs with the selected item as its parameter.
        public static readonly StyledProperty<ICommand?> DeleteCommandProperty =
            AvaloniaProperty.Register<ListBoxDetail, ICommand?>(nameof(DeleteCommand));

        public ICommand? DeleteCommand
        {
            get => GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }

        public static readonly StyledProperty<Control?> AddContentProperty =
            AvaloniaProperty.Register<ListBoxDetail, Control?>(nameof(AddContent));

        public Control? AddContent
        {
            get => GetValue(AddContentProperty);
            set => SetValue(AddContentProperty, value);
        }

        public static readonly StyledProperty<Control?> DetailContentProperty =
            AvaloniaProperty.Register<ListBoxDetail, Control?>(nameof(DetailContent));

        public Control? DetailContent
        {
            get => GetValue(DetailContentProperty);
            set => SetValue(DetailContentProperty, value);
        }
    }
}
