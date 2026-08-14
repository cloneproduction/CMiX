// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public class PickerControlBase : UserControl, IColorStateStorage
    {
        public static readonly StyledProperty<ColorState> ColorStateProperty =
            AvaloniaProperty.Register<PickerControlBase, ColorState>(nameof(ColorState),
                new ColorState(0, 0, 0, 1, 0, 0, 0, 0, 0, 0));

        public static readonly StyledProperty<global::Avalonia.Media.Color> SelectedColorProperty =
            AvaloniaProperty.Register<PickerControlBase, global::Avalonia.Media.Color>(nameof(SelectedColor),
                global::Avalonia.Media.Colors.Black);

        public static readonly RoutedEvent<ColorRoutedEventArgs> ColorChangedEvent =
            RoutedEvent.Register<PickerControlBase, ColorRoutedEventArgs>(nameof(ColorChanged), RoutingStrategies.Bubble);

        private bool ignoreColorChange;

        private bool ignoreColorPropertyChange;
        private global::Avalonia.Media.Color previousColor = global::Avalonia.Media.Color.FromArgb(5, 5, 5, 5);

        static PickerControlBase()
        {
            ColorStateProperty.Changed.AddClassHandler<PickerControlBase>(OnColorStatePropertyChange);
            SelectedColorProperty.Changed.AddClassHandler<PickerControlBase>(OnSelectedColorPropertyChange);
        }

        public PickerControlBase()
        {
            Color = new NotifyableColor(this);
            Color.PropertyChanged += (sender, args) =>
            {
                var newColor = global::Avalonia.Media.Color.FromArgb(
                    (byte)Math.Round(Color.A),
                    (byte)Math.Round(Color.RGB_R),
                    (byte)Math.Round(Color.RGB_G),
                    (byte)Math.Round(Color.RGB_B));
                if (newColor != previousColor)
                {
                    RaiseEvent(new ColorRoutedEventArgs(ColorChangedEvent, newColor));
                    previousColor = newColor;
                }
            };
            ColorChanged += (sender, newColor) =>
            {
                if (!ignoreColorChange)
                {
                    ignoreColorPropertyChange = true;
                    SelectedColor = newColor.Color;
                    ignoreColorPropertyChange = false;
                }
            };
        }

        public global::Avalonia.Media.Color SelectedColor
        {
            get => GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        public NotifyableColor Color { get; set; }

        public ColorState ColorState
        {
            get => GetValue(ColorStateProperty);
            set => SetValue(ColorStateProperty, value);
        }

        public event EventHandler<ColorRoutedEventArgs> ColorChanged
        {
            add => AddHandler(ColorChangedEvent, value);
            remove => RemoveHandler(ColorChangedEvent, value);
        }

        private static void OnColorStatePropertyChange(PickerControlBase sender, AvaloniaPropertyChangedEventArgs args)
        {
            sender.Color.UpdateEverything();
        }

        private static void OnSelectedColorPropertyChange(PickerControlBase sender, AvaloniaPropertyChangedEventArgs args)
        {
            if (sender.ignoreColorPropertyChange)
                return;
            var newValue = args.GetNewValue<global::Avalonia.Media.Color>();
            sender.ignoreColorChange = true;
            sender.Color.A = newValue.A;
            sender.Color.RGB_R = newValue.R;
            sender.Color.RGB_G = newValue.G;
            sender.Color.RGB_B = newValue.B;
            sender.ignoreColorChange = false;
        }
    }
}
