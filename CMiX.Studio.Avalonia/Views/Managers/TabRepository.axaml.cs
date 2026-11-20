// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections;
using Avalonia;
using Avalonia.Controls;
using CMiX.UI.Controls.Parts;

namespace CMiX.Studio.Avalonia.Views.Managers
{
    // Replaces the repeated MainWindow.axaml TabItem shape wrapping a ListRepository.
    public partial class TabRepository : TabItem
    {
        public TabRepository()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<string> IconProperty =
            AvaloniaProperty.Register<TabRepository, string>(nameof(Icon));
        public string Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly StyledProperty<string> ToolTipTextProperty =
            AvaloniaProperty.Register<TabRepository, string>(nameof(ToolTipText));
        public string ToolTipText
        {
            get => GetValue(ToolTipTextProperty);
            set => SetValue(ToolTipTextProperty, value);
        }

        public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
            AvaloniaProperty.Register<TabRepository, IEnumerable>(nameof(ItemsSource));
        public IEnumerable ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        // The menu of the rows of the list. None, the default, gives them no menu.
        public static readonly StyledProperty<RowMenuKind> RowMenuProperty =
            AvaloniaProperty.Register<TabRepository, RowMenuKind>(nameof(RowMenu));
        public RowMenuKind RowMenu
        {
            get => GetValue(RowMenuProperty);
            set => SetValue(RowMenuProperty, value);
        }

        public static readonly StyledProperty<Control?> SelectionPanelProperty =
            LogicalPanelContent.Register<TabRepository>(nameof(SelectionPanel));
        public Control? SelectionPanel
        {
            get => GetValue(SelectionPanelProperty);
            set => SetValue(SelectionPanelProperty, value);
        }

        public static readonly StyledProperty<Control?> EditingPanelProperty =
            LogicalPanelContent.Register<TabRepository>(nameof(EditingPanel));
        public Control? EditingPanel
        {
            get => GetValue(EditingPanelProperty);
            set => SetValue(EditingPanelProperty, value);
        }
    }
}
