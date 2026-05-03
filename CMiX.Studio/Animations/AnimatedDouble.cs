// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows;

namespace CMiX.Core.Animations
{
    public class AnimatedDouble : DependencyObject, IAnimatedDouble
    {
        public static readonly DependencyProperty AnimationPositionProperty =
            DependencyProperty.Register(
                "AnimationPosition",
                typeof(double),
                typeof(AnimatedDouble),
                new FrameworkPropertyMetadata(0.0, OnAnimationPositionChanged));

        private static void OnAnimationPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AnimatedDouble)d).PositionChanged?.Invoke(d, EventArgs.Empty);
        }

        public double AnimationPosition
        {
            get => (double)GetValue(AnimationPositionProperty);
            set => SetValue(AnimationPositionProperty, value);
        }

        public event EventHandler PositionChanged;
    }
}
