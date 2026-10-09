// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class ComposableManagerItem : UserControl
    {
        public ComposableManagerItem()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<bool> DragHandlerIsPressedProperty =
            AvaloniaProperty.Register<ComposableManagerItem, bool>(nameof(DragHandlerIsPressed), false, defaultBindingMode: BindingMode.TwoWay);
        public bool DragHandlerIsPressed
        {
            get => GetValue(DragHandlerIsPressedProperty);
            set => SetValue(DragHandlerIsPressedProperty, value);
        }

        private void Border_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (sender is Control control && e.GetCurrentPoint(control).Properties.IsLeftButtonPressed)
                DragHandlerIsPressed = true;
        }

        private void Border_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (e.InitialPressMouseButton == MouseButton.Left)
                DragHandlerIsPressed = false;
        }
    }
}
