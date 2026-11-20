// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using CMiX.Core;
using CMiX.Core.Prefabs.Managers;
using CMiX.UI.Controls.Parts;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class SelectorPrefab : UserControl
    {
        public SelectorPrefab()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not PrefabManagerBase selector) return;
            var newItem = (sender as SelectingItemsControl)?.SelectedItem as IControl;
            if (newItem == null || ReferenceEquals(newItem, selector.SelectedItem)) return;
            Dispatcher.UIThread.Post(() =>
            {
                selector.SelectedItem = newItem;
                // The manager can refuse the item. The box then shows the real selection again.
                if (!ReferenceEquals(selector.SelectedItem, newItem) && sender is SelectingItemsControl box)
                    box.SelectedItem = selector.SelectedItem;
            });
        }

        public static readonly StyledProperty<Control?> SelectionPanelProperty =
            LogicalPanelContent.Register<SelectorPrefab>(nameof(SelectionPanel));
        public Control? SelectionPanel
        {
            get => GetValue(SelectionPanelProperty);
            set => SetValue(SelectionPanelProperty, value);
        }

        public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
            AvaloniaProperty.Register<SelectorPrefab, IEnumerable>(nameof(ItemsSource));
        public IEnumerable ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
    }
}
