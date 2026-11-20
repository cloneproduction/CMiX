// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections;
using Avalonia;
using Avalonia.Controls;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Panels;

namespace CMiX.Studio.Avalonia.Views.Managers
{
    public partial class ListPrefabSlot : UserControl
    {
        static ListPrefabSlot()
        {
            // The selection panel is the middle of the row: the swap toggle is on its left, the arrows on its right.
            AddContentProperty.Changed.AddClassHandler<ListPrefabSlot>((_, e) =>
            {
                if (e.NewValue is Control panel)
                    PositionedControl.SetPosition(panel, ControlPosition.None);
            });
        }

        public ListPrefabSlot()
        {
            InitializeComponent();
            prefabListBox.ContainerPrepared += (_, e) => PrefabRowMenu.AttachTo(e.Container, RowMenu);
        }

        // An item of the repository was picked in the list of the swap toggle. The popup closes.
        private void OnSwapListPicked(object? sender, System.EventArgs e) => swapToggle.IsChecked = false;

        // The menu of the rows. None, the default, gives them no menu.
        public static readonly StyledProperty<RowMenuKind> RowMenuProperty =
            AvaloniaProperty.Register<ListPrefabSlot, RowMenuKind>(nameof(RowMenu));
        public RowMenuKind RowMenu
        {
            get => GetValue(RowMenuProperty);
            set => SetValue(RowMenuProperty, value);
        }

        // The least number of rows of the list. Zero, the default, takes the token ListVisibleItems when the
        // manager is in a window.
        public static readonly StyledProperty<int> MinItemCountProperty =
            AvaloniaProperty.Register<ListPrefabSlot, int>(nameof(MinItemCount));
        public int MinItemCount
        {
            get => GetValue(MinItemCountProperty);
            set => SetValue(MinItemCountProperty, value);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            if (MinItemCount == 0 && this.TryFindResource("ListVisibleItems", ActualThemeVariant, out var rows) && rows is int count)
                SetCurrentValue(MinItemCountProperty, count);
        }

        public static readonly StyledProperty<Control?> AddContentProperty =
            LogicalPanelContent.Register<ListPrefabSlot>(nameof(AddContent));
        public Control? AddContent
        {
            get => GetValue(AddContentProperty);
            set => SetValue(AddContentProperty, value);
        }

        public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
            AvaloniaProperty.Register<ListPrefabSlot, IEnumerable>(nameof(ItemsSource));
        public IEnumerable ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
    }
}
