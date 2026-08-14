// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections;
using Avalonia;
using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class PrefabSlotManager : UserControl
    {
        public PrefabSlotManager()
        {
            InitializeComponent();
        }

        // WPF FrameworkElement becomes the Avalonia Control base type.
        public static readonly StyledProperty<Control> SelectionPanelProperty =
            AvaloniaProperty.Register<PrefabSlotManager, Control>(nameof(SelectionPanel));
        public Control SelectionPanel
        {
            get => GetValue(SelectionPanelProperty);
            set => SetValue(SelectionPanelProperty, value);
        }

        // The WPF property was typed FrameworkElement (not DataTemplate) and is unused
        // by the XAML; ported as Control to keep the public surface.
        public static readonly StyledProperty<Control> ItemTemplateProperty =
            AvaloniaProperty.Register<PrefabSlotManager, Control>(nameof(ItemTemplate));
        public Control ItemTemplate
        {
            get => GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
            AvaloniaProperty.Register<PrefabSlotManager, IEnumerable>(nameof(ItemsSource));
        public IEnumerable ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
    }
}
