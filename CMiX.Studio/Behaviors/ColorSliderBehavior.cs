// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;
using CMiX.Studio.Views.Controls;

namespace CMiX.Studio.Behaviors
{
    public class ColorSliderBehavior : Behavior<CMiXSlider>, IColorStateStorage
    {
        public static readonly DependencyProperty ColorStateProperty =
            DependencyProperty.Register(nameof(ColorState), typeof(ColorState), typeof(ColorSliderBehavior),
                new PropertyMetadata(new ColorState(0, 0, 0, 1, 0, 0, 0, 0, 0, 0), OnColorStatePropertyChange));
        public ColorState ColorState
        {
            get => (ColorState)GetValue(ColorStateProperty);
            set => SetValue(ColorStateProperty, value);
        }


        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(nameof(SelectedColor), typeof(Color), typeof(ColorSliderBehavior),
                new PropertyMetadata(Colors.Black, OnSelectedColorPropertyChange));
        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }


        public static readonly RoutedEvent ColorChangedEvent =
            EventManager.RegisterRoutedEvent("ColorChanged",
                RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ColorSliderBehavior));
        public static void AddColorChangedHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement uie = d as UIElement;
            if (uie != null)
                uie.AddHandler(ColorChangedEvent, handler);
        }
        public static void RemoveColorChangedHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement uie = d as UIElement;
            if (uie != null)
                uie.RemoveHandler(ColorChangedEvent, handler);
        }


        private bool ignoreColorChange;
        private bool ignoreColorPropertyChange;
        private Color previousColor = System.Windows.Media.Color.FromArgb(5, 5, 5, 5);
        public NotifyableColor Color { get; set; }


        public ColorSliderBehavior()
        {

        }


        private static void OnColorStatePropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((ColorSliderBehavior)d).Color.UpdateEverything();
        }

        private static void OnSelectedColorPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var sender = (ColorSliderBehavior)d;
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


        protected override void OnAttached()
        {
            base.OnAttached();
            Color = new NotifyableColor(this);
            Color.PropertyChanged += Color_PropertyChanged;
            AddColorChangedHandler(AssociatedObject, handler: OnColorChanged);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            Color.PropertyChanged -= Color_PropertyChanged;
            RemoveColorChangedHandler(AssociatedObject, handler: OnColorChanged);
        }

        private void Color_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var newColor = System.Windows.Media.Color.FromArgb(
                (byte)Math.Round(Color.A),
                (byte)Math.Round(Color.RGB_R),
                (byte)Math.Round(Color.RGB_G),
                (byte)Math.Round(Color.RGB_B));
            if (newColor != previousColor)
            {
                AssociatedObject.RaiseEvent(new ColorRoutedEventArgs(ColorChangedEvent, newColor));
                previousColor = newColor;
            }
        }

        private void OnColorChanged(object sender, RoutedEventArgs e)
        {
            if (!ignoreColorChange)
            {
                ignoreColorPropertyChange = true;
                SelectedColor = ((ColorRoutedEventArgs)e).Color;
                ignoreColorPropertyChange = false;
            }
        }
    }
}
