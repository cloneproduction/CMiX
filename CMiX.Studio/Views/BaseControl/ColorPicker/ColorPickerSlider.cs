// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;

namespace CMiX.Studio.Views.BaseControl
{
    public class ColorPickerSlider : CMiXSlider, IColorStateStorage
    {
        public static readonly DependencyProperty ColorStateProperty =
            DependencyProperty.Register(nameof(ColorState), typeof(ColorState), typeof(ColorPickerSlider),
                new PropertyMetadata(new ColorState(0, 0, 0, 1, 0, 0, 0, 0, 0, 0), OnColorStatePropertyChange));
        public ColorState ColorState
        {
            get => (ColorState)GetValue(ColorStateProperty);
            set => SetValue(ColorStateProperty, value);
        }


        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(nameof(SelectedColor), typeof(Color), typeof(ColorPickerSlider),
                new PropertyMetadata(Colors.Black, OnSelectedColorPropertyChange));
        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }


        public static readonly RoutedEvent ColorChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(ColorChanged),
                RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ColorPickerSlider));
        public event RoutedEventHandler ColorChanged
        {
            add => AddHandler(ColorChangedEvent, value);
            remove => RemoveHandler(ColorChangedEvent, value);
        }


        private bool ignoreColorChange;
        private bool ignoreColorPropertyChange;

        private Color previousColor = System.Windows.Media.Color.FromArgb(5, 5, 5, 5);
        public NotifyableColor Color { get; set; }


        public ColorPickerSlider()
        {
            Color = new NotifyableColor(this);
            Color.PropertyChanged += (sender, args) =>
            {
                var newColor = System.Windows.Media.Color.FromArgb(
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
                    SelectedColor = ((ColorRoutedEventArgs)newColor).Color;
                    ignoreColorPropertyChange = false;
                }
            };
        }


        private static void OnColorStatePropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((ColorPickerSlider)d).Color.UpdateEverything((ColorState)args.OldValue);
        }

        private static void OnSelectedColorPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sender = (ColorPickerSlider)d;
            if (sender.ignoreColorPropertyChange)
                return;
            var newValue = (Color)args.NewValue;
            sender.ignoreColorChange = true;
            sender.Color.A = newValue.A;
            sender.Color.RGB_R = newValue.R;
            sender.Color.RGB_G = newValue.G;
            sender.Color.RGB_B = newValue.B;
            sender.ignoreColorChange = false;
        }
    }
}
