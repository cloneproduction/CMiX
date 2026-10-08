// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class ColorSelector : CaptionedUserControl
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
