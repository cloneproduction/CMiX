// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections;
using Avalonia;
using Avalonia.Controls;
using CMiX.UI.Controls.Parts;

namespace CMiX.Studio.Avalonia.Views.Managers
{
    public partial class ListRepository : UserControl
    {
        public ListRepository()
        {
            InitializeComponent();
            detailList.ContainerPrepared += (_, e) => PrefabRowMenu.AttachTo(e.Container, RowMenu);
        }

        // The menu of the rows. None, the default, gives them no menu.
        public static readonly StyledProperty<RowMenuKind> RowMenuProperty =
            AvaloniaProperty.Register<ListRepository, RowMenuKind>(nameof(RowMenu));
        public RowMenuKind RowMenu
        {
            get => GetValue(RowMenuProperty);
            set => SetValue(RowMenuProperty, value);
        }

        public static readonly StyledProperty<Control?> AddContentProperty =
            LogicalPanelContent.Register<ListRepository>(nameof(AddContent));
        public Control? AddContent
        {
            get => GetValue(AddContentProperty);
            set => SetValue(AddContentProperty, value);
        }

        public static readonly StyledProperty<Control?> EditingPanelProperty =
            LogicalPanelContent.Register<ListRepository>(nameof(EditingPanel));
        public Control? EditingPanel
        {
            get => GetValue(EditingPanelProperty);
            set => SetValue(EditingPanelProperty, value);
        }

        public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
            AvaloniaProperty.Register<ListRepository, IEnumerable>(nameof(ItemsSource));
        public IEnumerable ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
    }
}
