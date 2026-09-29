// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class IntegerValue : CaptionedUserControl
    {
        public IntegerValue()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<double> ValueProperty =
            AvaloniaProperty.Register<IntegerValue, double>(nameof(Value), 0.0, defaultBindingMode: BindingMode.TwoWay);
        public double Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly StyledProperty<double> MaximumProperty =
            AvaloniaProperty.Register<IntegerValue, double>(nameof(Maximum), 10000.0, defaultBindingMode: BindingMode.TwoWay);
        public double Maximum
        {
            get => GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public static readonly StyledProperty<double> MinimumProperty =
            AvaloniaProperty.Register<IntegerValue, double>(nameof(Minimum), -10000.0, defaultBindingMode: BindingMode.TwoWay);
        public double Minimum
        {
            get => GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        // Set only when this control is one axis of a composite (Integer2Value, Integer3Value),
        // so its context menu can offer "Reset All" next to its own "Reset".
        public static readonly StyledProperty<ICommand> ResetAllCommandProperty =
            AvaloniaProperty.Register<IntegerValue, ICommand>(nameof(ResetAllCommand));
        public ICommand ResetAllCommand
        {
            get => GetValue(ResetAllCommandProperty);
            set => SetValue(ResetAllCommandProperty, value);
        }
    }
}
