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
                _positionChanged?.Invoke(this, EventArgs.Empty);
                _propertyChanged?.Invoke(this, AnimationPositionChangedArgs);
            }
        }

        private EventHandler _positionChanged;
        private PropertyChangedEventHandler _propertyChanged;
        private int _observerCount;

        public event EventHandler PositionChanged
        {
            add
            {
                _positionChanged += value;
                if (CountsAsObserver(value)) AddObserver();
            }
            remove
            {
                _positionChanged -= value;
                if (CountsAsObserver(value)) RemoveObserver();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                _propertyChanged += value;
                AddObserver();
            }
            remove
            {
                _propertyChanged -= value;
                RemoveObserver();
            }
        }

        // True while something reads this instance: a binding through PropertyChanged, or a beat
        // modifier stepping through PositionChanged. The beat clock only advances observed instances.
        public bool IsObserved => _observerCount > 0;

        // Raised when the first observer arrives, so a stopped beat clock can start again.
        public event EventHandler ObservationStarted;

        // The master beat re raises PositionChanged as its own change notification for as long as
        // it holds this instance, which is the whole application lifetime, so that subscription on
        // its own must not count as an observer or the clock could never idle. Its own bindings
        // subscribe through PropertyChanged and are counted there.
        private static bool CountsAsObserver(EventHandler handler) => handler?.Target is not MasterBeat;

        private void AddObserver()
        {
            _observerCount++;
            if (_observerCount == 1)
                ObservationStarted?.Invoke(this, EventArgs.Empty);
        }

        private void RemoveObserver()
        {
            if (_observerCount > 0)
                _observerCount--;
        }
    }
}
