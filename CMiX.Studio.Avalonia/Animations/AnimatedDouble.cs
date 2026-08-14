// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using CMiX.Core.Animations;

namespace CMiX.Studio.Avalonia.Animations
{
    // Clock driven replacement for the WPF DependencyObject that was animated by a Storyboard.
    // INotifyPropertyChanged replaces the change notifications the WPF dependency property
    // gave to bindings such as the tap button beat pulse.
    public class AnimatedDouble : IAnimatedDouble, INotifyPropertyChanged
    {
        private static readonly PropertyChangedEventArgs AnimationPositionChangedArgs = new(nameof(AnimationPosition));

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
                PropertyChanged?.Invoke(this, AnimationPositionChangedArgs);
            }
        }

        public event EventHandler PositionChanged;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
