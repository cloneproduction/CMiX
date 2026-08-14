// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class LayerManagerItem : UserControl
    {
        public LayerManagerItem()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<bool> DragHandlerIsPressedProperty =
            AvaloniaProperty.Register<LayerManagerItem, bool>(nameof(DragHandlerIsPressed), false, defaultBindingMode: BindingMode.TwoWay);
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
