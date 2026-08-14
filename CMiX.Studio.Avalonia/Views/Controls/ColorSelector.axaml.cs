// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class ColorSelector : UserControl
    {
        public ColorSelector()
        {
            InitializeComponent();

            Loaded += (s, e) => colorPickerPopup.PlacementTarget = PopupToggle;

            // The duplicated WPF Checked and Unchecked subscriptions collapse into one handler.
            PopupToggle.IsCheckedChanged += (s, e) => colorPickerPopup.IsOpen = PopupToggle.IsChecked == true;
        }

        public static readonly StyledProperty<string> CaptionProperty =
            AvaloniaProperty.Register<ColorSelector, string>(nameof(Caption), string.Empty);
        public string Caption
        {
            get => GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
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
