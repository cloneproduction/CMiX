// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Behaviors;
using CMiX.UI.Panels;

namespace CMiX.UI.Controls.Collection
{
    // A list of items of one type, with a column of buttons on its right: add and remove, then move up
    // and move down. The application sets the ItemTemplate, so an item can
    // be a DragValue, a ColorSelector or any other editor. A button shows only when its command is set.
    // When ReorderService is set, each row has a drag handle and the user can drag the rows.
    public partial class ListBoxEditable : UserControl
    {
        static ListBoxEditable()
        {
            MinItemCountProperty.Changed.AddClassHandler<ListBoxEditable>((list, _) => list.UpdateMinHeight());

            // A button shows only when its command is set, and the corners depend on which buttons show.
            foreach (var property in new[] { AddCommandProperty, RemoveCommandProperty, MoveUpCommandProperty, MoveDownCommandProperty })
                property.Changed.AddClassHandler<ListBoxEditable>((list, e) => list.UpdateButtons());
        }

        public ListBoxEditable()
        {
            InitializeComponent();
            UpdateButtons();

            itemsList.ContainerPrepared += (s, e) => SetRowNumber(e.Container, e.Index + 1);
            itemsList.ContainerIndexChanged += (s, e) => SetRowNumber(e.Container, e.NewIndex + 1);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            UpdateMinHeight();
        }

        // Room for MinItemCount rows (or the token ListEditableVisibleItems) of ControlHeightMD, the spaces
        // between them and the padding of the list. The numbers are tokens in Geometry.axaml. Before the list
        // is in a window they are not found, and the list calls this again when it is attached.
        private void UpdateMinHeight()
        {
            if (!this.TryFindResource("ControlHeightMD", ActualThemeVariant, out var row) || row is not double rowHeight
                || !this.TryFindResource("ListEditableVisibleItems", ActualThemeVariant, out var count) || count is not int tokenRows
                || !this.TryFindResource("ListEditableSpacing", ActualThemeVariant, out var spacing) || spacing is not double space
                || !this.TryFindResource("ListEditablePadding", ActualThemeVariant, out var padding) || padding is not Thickness margin)
                return;

            var rows = MinItemCount > 0 ? MinItemCount : tokenRows;
            itemsList.MinHeight = rows * rowHeight + Math.Max(0, rows - 1) * space + margin.Top + margin.Bottom;
        }

        // The place of a row in the list, from 1. It is set on the container of the row, so the
        // template of the item can use it as a caption, for example Step 1. It follows the moves.
        public static readonly AttachedProperty<int> RowNumberProperty =
            AvaloniaProperty.RegisterAttached<ListBoxEditable, Control, int>("RowNumber");

        public static int GetRowNumber(Control control) => control.GetValue(RowNumberProperty);

        private static void SetRowNumber(Control container, int number) => container.SetValue(RowNumberProperty, number);

        private void UpdateButtons()
        {
            SetGroup(editGroup, addButton, AddCommand != null, removeButton, RemoveCommand != null);
            SetGroup(moveGroup, moveUpButton, MoveUpCommand != null, moveDownButton, MoveDownCommand != null);
        }

        // Two buttons in a group are rounded at the two ends of the group. One button is rounded all
        // around. A group with no button is not shown, so it keeps no space.
        private static void SetGroup(Control group, Control first, bool firstShown, Control second, bool secondShown)
        {
            group.IsVisible = firstShown || secondShown;
            first.IsVisible = firstShown;
            second.IsVisible = secondShown;

            var both = firstShown && secondShown;
            PositionedControl.SetPosition(first, both ? ControlPosition.Top : ControlPosition.All);
            PositionedControl.SetPosition(second, both ? ControlPosition.Bottom : ControlPosition.All);
        }

        // The least number of rows of the list, for example 6. Zero, the default, takes the token
        // ListEditableVisibleItems. The list is as high as its items when it has more rows.
        public static readonly StyledProperty<int> MinItemCountProperty =
            AvaloniaProperty.Register<ListBoxEditable, int>(nameof(MinItemCount));

        public int MinItemCount
        {
            get => GetValue(MinItemCountProperty);
            set => SetValue(MinItemCountProperty, value);
        }

        public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
            AvaloniaProperty.Register<ListBoxEditable, IEnumerable?>(nameof(ItemsSource));

        public IEnumerable? ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty =
            AvaloniaProperty.Register<ListBoxEditable, IDataTemplate?>(nameof(ItemTemplate));

        public IDataTemplate? ItemTemplate
        {
            get => GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public static readonly StyledProperty<object?> SelectedItemProperty =
            AvaloniaProperty.Register<ListBoxEditable, object?>(nameof(SelectedItem), defaultBindingMode: BindingMode.TwoWay);

        public object? SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static readonly StyledProperty<int> SelectedIndexProperty =
            AvaloniaProperty.Register<ListBoxEditable, int>(nameof(SelectedIndex), -1, defaultBindingMode: BindingMode.TwoWay);

        public int SelectedIndex
        {
            get => GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        // Without a service the items cannot be reordered with the pointer.
        public static readonly StyledProperty<IReorderService?> ReorderServiceProperty =
            AvaloniaProperty.Register<ListBoxEditable, IReorderService?>(nameof(ReorderService));

        public IReorderService? ReorderService
        {
            get => GetValue(ReorderServiceProperty);
            set => SetValue(ReorderServiceProperty, value);
        }

        // The remove and move commands get the selected item as parameter.
        public static readonly StyledProperty<ICommand?> RemoveCommandProperty =
            AvaloniaProperty.Register<ListBoxEditable, ICommand?>(nameof(RemoveCommand));

        public ICommand? RemoveCommand
        {
            get => GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }

        public static readonly StyledProperty<ICommand?> MoveUpCommandProperty =
            AvaloniaProperty.Register<ListBoxEditable, ICommand?>(nameof(MoveUpCommand));

        public ICommand? MoveUpCommand
        {
            get => GetValue(MoveUpCommandProperty);
            set => SetValue(MoveUpCommandProperty, value);
        }

        public static readonly StyledProperty<ICommand?> MoveDownCommandProperty =
            AvaloniaProperty.Register<ListBoxEditable, ICommand?>(nameof(MoveDownCommand));

        public ICommand? MoveDownCommand
        {
            get => GetValue(MoveDownCommandProperty);
            set => SetValue(MoveDownCommandProperty, value);
        }

        // The add command gets AddCommandParameter, for example the type of the item to add.
        public static readonly StyledProperty<ICommand?> AddCommandProperty =
            AvaloniaProperty.Register<ListBoxEditable, ICommand?>(nameof(AddCommand));

        public ICommand? AddCommand
        {
            get => GetValue(AddCommandProperty);
            set => SetValue(AddCommandProperty, value);
        }

        public static readonly StyledProperty<object?> AddCommandParameterProperty =
            AvaloniaProperty.Register<ListBoxEditable, object?>(nameof(AddCommandParameter));

        public object? AddCommandParameter
        {
            get => GetValue(AddCommandParameterProperty);
            set => SetValue(AddCommandParameterProperty, value);
        }
    }
}
