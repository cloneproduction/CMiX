// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

// This behavior depends on ColorState, NotifyableColor, IColorStateStorage and
// ColorRoutedEventArgs from CMiX.Studio.Avalonia.Views.Controls. These types are ported
// in the ColorPicker phase, this file will not compile until they exist.

using System;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Xaml.Interactivity;
using CMiX.Studio.Avalonia.Views.Controls;

namespace CMiX.Studio.Avalonia.Behaviors
{
    public class ColorSliderBehavior : Behavior<CMiXSlider>, IColorStateStorage
    {
        public static readonly StyledProperty<ColorState> ColorStateProperty =
            AvaloniaProperty.Register<ColorSliderBehavior, ColorState>(
                nameof(ColorState), new ColorState(0, 0, 0, 1, 0, 0, 0, 0, 0, 0));
        public ColorState ColorState
        {
            get => GetValue(ColorStateProperty);
            set => SetValue(ColorStateProperty, value);
        }


        public static readonly StyledProperty<Color> SelectedColorProperty =
            AvaloniaProperty.Register<ColorSliderBehavior, Color>(nameof(SelectedColor), Colors.Black);
        public Color SelectedColor
        {
            get => GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        public static readonly RoutedEvent<ColorRoutedEventArgs> ColorChangedEvent =
            RoutedEvent.Register<ColorSliderBehavior, ColorRoutedEventArgs>(
                "ColorChanged", RoutingStrategies.Bubble);
        public static void AddColorChangedHandler(Interactive element, EventHandler<ColorRoutedEventArgs> handler)
        {
            element.AddHandler(ColorChangedEvent, handler);
        }
        public static void RemoveColorChangedHandler(Interactive element, EventHandler<ColorRoutedEventArgs> handler)
        {
            element.RemoveHandler(ColorChangedEvent, handler);
        }


        private bool ignoreColorChange;
        private bool ignoreColorPropertyChange;
        private Color previousColor = global::Avalonia.Media.Color.FromArgb(5, 5, 5, 5);
        public NotifyableColor? Color { get; set; }


        static ColorSliderBehavior()
        {
            ColorStateProperty.Changed.AddClassHandler<ColorSliderBehavior>(OnColorStatePropertyChange);
            SelectedColorProperty.Changed.AddClassHandler<ColorSliderBehavior>(OnSelectedColorPropertyChange);
        }


        private static void OnColorStatePropertyChange(ColorSliderBehavior behavior, AvaloniaPropertyChangedEventArgs args)
        {
            behavior.Color?.UpdateEverything();
        }

        private static void OnSelectedColorPropertyChange(ColorSliderBehavior sender, AvaloniaPropertyChangedEventArgs args)
        {
            if (sender.ignoreColorPropertyChange || sender.Color == null)
                return;

            var newValue = args.GetNewValue<Color>();
            sender.ignoreColorChange = true;
            sender.Color.A = newValue.A;
            sender.Color.RGB_R = newValue.R;
            sender.Color.RGB_G = newValue.G;
            sender.Color.RGB_B = newValue.B;
            sender.ignoreColorChange = false;
        }


        protected override void OnAttached()
        {
            base.OnAttached();
            Color = new NotifyableColor(this);
            Color.PropertyChanged += Color_PropertyChanged;
            if (AssociatedObject != null)
                AddColorChangedHandler(AssociatedObject, OnColorChanged);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (Color != null)
                Color.PropertyChanged -= Color_PropertyChanged;
            if (AssociatedObject != null)
                RemoveColorChangedHandler(AssociatedObject, OnColorChanged);
        }

        private void Color_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Color == null)
                return;

            var newColor = global::Avalonia.Media.Color.FromArgb(
                (byte)Math.Round(Color.A),
                (byte)Math.Round(Color.RGB_R),
                (byte)Math.Round(Color.RGB_G),
                (byte)Math.Round(Color.RGB_B));
            if (newColor != previousColor)
            {
                AssociatedObject?.RaiseEvent(new ColorRoutedEventArgs(ColorChangedEvent, newColor));
                previousColor = newColor;
            }
        }

        private void OnColorChanged(object? sender, ColorRoutedEventArgs e)
        {
            if (!ignoreColorChange)
            {
                ignoreColorPropertyChange = true;
                SelectedColor = e.Color;
                ignoreColorPropertyChange = false;
            }
        }
    }
}
