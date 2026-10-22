// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections;
using Avalonia;
using Avalonia.Controls;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Panels;

namespace CMiX.Studio.Avalonia.Views.Managers
{
    public partial class PrefabSlotManager : UserControl
    {
        // Roots panel content in the logical tree so its deferred bindings apply; see LogicalPanelContent.
        static PrefabSlotManager()
        {
            LogicalPanelContent.Track<PrefabSlotManager>(SelectionPanelProperty);

            // The selection panel is the middle of the row: the swap toggle is on its left, the arrows on its right.
            SelectionPanelProperty.Changed.AddClassHandler<PrefabSlotManager>((_, e) =>
            {
                if (e.NewValue is Control panel)
                    PositionedControl.SetPosition(panel, ControlPosition.None);
            });
        }

        public PrefabSlotManager()
        {
            InitializeComponent();
            prefabListBox.ContainerPrepared += (_, e) => PrefabRowMenu.AttachTo(e.Container, RowMenu);
        }

        // The menu of the rows. None, the default, gives them no menu.
        public static readonly StyledProperty<RowMenuKind> RowMenuProperty =
            AvaloniaProperty.Register<PrefabSlotManager, RowMenuKind>(nameof(RowMenu));
        public RowMenuKind RowMenu
        {
            get => GetValue(RowMenuProperty);
            set => SetValue(RowMenuProperty, value);
        }

        // The least number of rows of the list. Zero, the default, takes the token ListVisibleItems when the
        // manager is in a window.
        public static readonly StyledProperty<int> MinItemCountProperty =
            AvaloniaProperty.Register<PrefabSlotManager, int>(nameof(MinItemCount));
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

        public static readonly StyledProperty<Control> SelectionPanelProperty =
            AvaloniaProperty.Register<PrefabSlotManager, Control>(nameof(SelectionPanel));
        public Control SelectionPanel
        {
            get => GetValue(SelectionPanelProperty);
            set => SetValue(SelectionPanelProperty, value);
        }

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
