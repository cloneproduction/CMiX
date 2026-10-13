// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using CMiX.UI.Controls.Parts;

namespace CMiX.UI.Controls.ColorPicker
{
    public partial class ColorSelector : UserControlCaptioned
    {
        public ColorSelector()
        {
            InitializeComponent();

            AddHandler(ContextRequestedEvent, OnTunnelContextRequested, RoutingStrategies.Tunnel);
            Loaded += (s, e) => colorPickerPopup.PlacementTarget = PopupToggle;
            PopupToggle.IsCheckedChanged += (s, e) => colorPickerPopup.IsOpen = PopupToggle.IsChecked == true;
        }

        // The color picker popup sends its right-clicks up to here. They must not open the Reset menu of the color.
        private void OnTunnelContextRequested(object? sender, ContextRequestedEventArgs e)
        {
            if (ContextMenuGuard.FromPopup(this, e))
                e.Handled = true;
        }

        public static readonly StyledProperty<Color> SelectedColorProperty =
            AvaloniaProperty.Register<ColorSelector, Color>(nameof(SelectedColor), Colors.Yellow, defaultBindingMode: BindingMode.TwoWay);
        public Color SelectedColor
        {
            get => GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }
    }
}
