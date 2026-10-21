// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using CMiX.Studio.Avalonia.Views.Managers;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class ComposableManagerItem : UserControl
    {
        public ComposableManagerItem()
        {
            InitializeComponent();
            PrefabRowMenu.AttachTo(this, RowMenuKind.Composable, () => editableTextBox);
        }

        public static readonly StyledProperty<bool> DragHandleIsPressedProperty =
            AvaloniaProperty.Register<ComposableManagerItem, bool>(nameof(DragHandleIsPressed), false, defaultBindingMode: BindingMode.TwoWay);
        public bool DragHandleIsPressed
        {
            get => GetValue(DragHandleIsPressedProperty);
            set => SetValue(DragHandleIsPressedProperty, value);
        }

        private void Border_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (sender is Control control && e.GetCurrentPoint(control).Properties.IsLeftButtonPressed)
                DragHandleIsPressed = true;
        }

        private void Border_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (e.InitialPressMouseButton == MouseButton.Left)
                DragHandleIsPressed = false;
        }
    }
}
