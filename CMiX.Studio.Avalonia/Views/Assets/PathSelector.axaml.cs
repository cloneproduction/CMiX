// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class PathSelector : UserControl
    {
        public PathSelector()
        {
            InitializeComponent();

            // TODO Avalonia: drag drop is wired in the drag and drop phase. The WPF code behind
            // attached an AssetSelectorDropHandler to pathComboBox when the DataContext became
            // an AssetSelector.
        }

        public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
            AvaloniaProperty.Register<PathSelector, IEnumerable>(nameof(ItemsSource));
        public IEnumerable ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly StyledProperty<object> SelectedItemProperty =
            AvaloniaProperty.Register<PathSelector, object>(nameof(SelectedItem), defaultBindingMode: BindingMode.TwoWay);
        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }
    }
}
