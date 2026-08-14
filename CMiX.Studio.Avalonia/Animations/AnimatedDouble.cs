// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Animations;

namespace CMiX.Studio.Avalonia.Animations
{
    // Clock driven replacement for the WPF DependencyObject that was animated by a Storyboard.
    public class AnimatedDouble : IAnimatedDouble
    {
        public AnimatedDouble(double period)
        {
            Period = period;
        }

        public double Period { get; }

        private double _animationPosition;
        public double AnimationPosition
        {
            get => _animationPosition;
            set
            {
                if (_animationPosition == value)
                    return;

                _animationPosition = value;
                PositionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler PositionChanged;
    }
}
