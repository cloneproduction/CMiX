// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class CaptionedToggleButton : CaptionedUserControl
    {
        public CaptionedToggleButton()
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

        public static readonly StyledProperty<bool> IsCheckedProperty =
            AvaloniaProperty.Register<CaptionedToggleButton, bool>(nameof(IsChecked), false, defaultBindingMode: BindingMode.TwoWay);
        public bool IsChecked
        {
            get => GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }
    }
}
