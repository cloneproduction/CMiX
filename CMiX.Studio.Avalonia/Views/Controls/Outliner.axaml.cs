// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections;
using Avalonia;
using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class Outliner : UserControl
    {
        public Outliner()
        {
            InitializeComponent();
        }

        // WPF FrameworkElement becomes the Avalonia Control base type.
        public static readonly StyledProperty<Control> SelectionPanelProperty =
            AvaloniaProperty.Register<Outliner, Control>(nameof(SelectionPanel));
        public Control SelectionPanel
        {
            get => GetValue(SelectionPanelProperty);
            set => SetValue(SelectionPanelProperty, value);
        }

        public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
            AvaloniaProperty.Register<Outliner, IEnumerable>(nameof(ItemsSource));
        public IEnumerable ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
    }
}
