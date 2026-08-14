// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class ToggleButton : UserControl
    {
        public ToggleButton()
        {
            InitializeComponent();
            labelBorder.PointerPressed += (s, e) =>
            {
                if (!e.GetCurrentPoint(labelBorder).Properties.IsLeftButtonPressed)
                    return;

                toggleButton.IsChecked = toggleButton.IsChecked != true;
                e.Handled = true;
            };
        }

        public static readonly StyledProperty<string> CaptionProperty =
            AvaloniaProperty.Register<ToggleButton, string>(nameof(Caption), string.Empty);
        public string Caption
        {
            get => GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }

        public static readonly StyledProperty<bool> IsCheckedProperty =
            AvaloniaProperty.Register<ToggleButton, bool>(nameof(IsChecked), false, defaultBindingMode: BindingMode.TwoWay);
        public bool IsChecked
        {
            get => GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }
    }
}
