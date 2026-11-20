// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections;
using Avalonia;
using Avalonia.Controls;
using CMiX.UI.Controls.Parts;

namespace CMiX.Studio.Avalonia.Views.Managers
{
    public partial class RepositoryManager : UserControl
    {
        public RepositoryManager()
        {
            InitializeComponent();
            detailList.ContainerPrepared += (_, e) => PrefabRowMenu.AttachTo(e.Container, RowMenu);
        }

        // The menu of the rows. None, the default, gives them no menu.
        public static readonly StyledProperty<RowMenuKind> RowMenuProperty =
            AvaloniaProperty.Register<RepositoryManager, RowMenuKind>(nameof(RowMenu));
        public RowMenuKind RowMenu
        {
            get => GetValue(RowMenuProperty);
            set => SetValue(RowMenuProperty, value);
        }

        public static readonly StyledProperty<Control?> SelectionPanelProperty =
            LogicalPanelContent.Register<RepositoryManager>(nameof(SelectionPanel));
        public Control? SelectionPanel
        {
            get => GetValue(SelectionPanelProperty);
            set => SetValue(SelectionPanelProperty, value);
        }

        public static readonly StyledProperty<Control?> EditingPanelProperty =
            LogicalPanelContent.Register<RepositoryManager>(nameof(EditingPanel));
        public Control? EditingPanel
        {
            get => GetValue(EditingPanelProperty);
            set => SetValue(EditingPanelProperty, value);
        }

        public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
            AvaloniaProperty.Register<RepositoryManager, IEnumerable>(nameof(ItemsSource));
        public IEnumerable ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
    }
}
