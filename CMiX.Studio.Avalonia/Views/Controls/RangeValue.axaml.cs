// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class RangeValue : UserControl
    {
        public RangeValue()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<string> LeftCaptionProperty =
            AvaloniaProperty.Register<RangeValue, string>(nameof(LeftCaption), string.Empty);
        public string LeftCaption
        {
            get => GetValue(LeftCaptionProperty);
            set => SetValue(LeftCaptionProperty, value);
        }

        public static readonly StyledProperty<string> RightCaptionProperty =
            AvaloniaProperty.Register<RangeValue, string>(nameof(RightCaption), string.Empty);
        public string RightCaption
        {
            get => GetValue(RightCaptionProperty);
            set => SetValue(RightCaptionProperty, value);
        }

        public static readonly StyledProperty<double> LeftValueProperty =
            AvaloniaProperty.Register<RangeValue, double>(nameof(LeftValue), 0.0, defaultBindingMode: BindingMode.TwoWay);
        public double LeftValue
        {
            get => GetValue(LeftValueProperty);
            set => SetValue(LeftValueProperty, value);
        }

        public static readonly StyledProperty<double> RightValueProperty =
            AvaloniaProperty.Register<RangeValue, double>(nameof(RightValue), 0.0, defaultBindingMode: BindingMode.TwoWay);
        public double RightValue
        {
            get => GetValue(RightValueProperty);
            set => SetValue(RightValueProperty, value);
        }

        public static readonly StyledProperty<ICommand> LeftResetCommandProperty =
            AvaloniaProperty.Register<RangeValue, ICommand>(nameof(LeftResetCommand));
        public ICommand LeftResetCommand
        {
            get => GetValue(LeftResetCommandProperty);
            set => SetValue(LeftResetCommandProperty, value);
        }

        public static readonly StyledProperty<ICommand> RightResetCommandProperty =
            AvaloniaProperty.Register<RangeValue, ICommand>(nameof(RightResetCommand));
        public ICommand RightResetCommand
        {
            get => GetValue(RightResetCommandProperty);
            set => SetValue(RightResetCommandProperty, value);
        }

        public static readonly StyledProperty<double> MaximumProperty =
            AvaloniaProperty.Register<RangeValue, double>(nameof(Maximum), 10000.0);
        public double Maximum
        {
            get => GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public static readonly StyledProperty<double> MinimumProperty =
            AvaloniaProperty.Register<RangeValue, double>(nameof(Minimum), -10000.0);
        public double Minimum
        {
            get => GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public static readonly StyledProperty<double> SmallChangeProperty =
            AvaloniaProperty.Register<RangeValue, double>(nameof(SmallChange), DragValue.DefaultSmallChange);
        public double SmallChange
        {
            get => GetValue(SmallChangeProperty);
            set => SetValue(SmallChangeProperty, value);
        }

        public static readonly StyledProperty<double> LargeChangeProperty =
            AvaloniaProperty.Register<RangeValue, double>(nameof(LargeChange), DragValue.DefaultLargeChange);
        public double LargeChange
        {
            get => GetValue(LargeChangeProperty);
            set => SetValue(LargeChangeProperty, value);
        }

        public static readonly StyledProperty<bool> IsIntegerProperty =
            AvaloniaProperty.Register<RangeValue, bool>(nameof(IsInteger), false);
        public bool IsInteger
        {
            get => GetValue(IsIntegerProperty);
            set => SetValue(IsIntegerProperty, value);
        }
    }
}
