// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections;
using Avalonia;
using Avalonia.Controls;
using CMiX.UI.Controls.Parts;

namespace CMiX.Studio.Avalonia.Views.Managers
{
    // The slot list of a manager and the editor of its selected item. The DataContext is the manager.
    public partial class EditorPrefabSlot : UserControl
    {
        public EditorPrefabSlot()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
            AvaloniaProperty.Register<EditorPrefabSlot, IEnumerable>(nameof(ItemsSource));
        public IEnumerable ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly StyledProperty<Control?> SelectionPanelProperty =
            LogicalPanelContent.Register<EditorPrefabSlot>(nameof(SelectionPanel));
        public Control? SelectionPanel
        {
            get => GetValue(SelectionPanelProperty);
            set => SetValue(SelectionPanelProperty, value);
        }

        public static readonly StyledProperty<Control?> EditingPanelProperty =
            LogicalPanelContent.Register<EditorPrefabSlot>(nameof(EditingPanel));
        public Control? EditingPanel
        {
            get => GetValue(EditingPanelProperty);
            set => SetValue(EditingPanelProperty, value);
        }
    }
}
