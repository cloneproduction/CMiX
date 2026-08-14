// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class PrefabManagerComboBox : UserControl
    {
        public PrefabManagerComboBox()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
            AvaloniaProperty.Register<PrefabManagerComboBox, IEnumerable>(nameof(ItemsSource));
        public IEnumerable ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly StyledProperty<object> SelectedItemProperty =
            AvaloniaProperty.Register<PrefabManagerComboBox, object>(nameof(SelectedItem), defaultBindingMode: BindingMode.TwoWay);
        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static readonly StyledProperty<ICommand> AddItemCommandProperty =
            AvaloniaProperty.Register<PrefabManagerComboBox, ICommand>(nameof(AddItemCommand));
        public ICommand AddItemCommand
        {
            get => GetValue(AddItemCommandProperty);
            set => SetValue(AddItemCommandProperty, value);
        }

        public static readonly StyledProperty<object> AddItemCommandParameterProperty =
            AvaloniaProperty.Register<PrefabManagerComboBox, object>(nameof(AddItemCommandParameter));
        public object AddItemCommandParameter
        {
            get => GetValue(AddItemCommandParameterProperty);
            set => SetValue(AddItemCommandParameterProperty, value);
        }
    }
}
