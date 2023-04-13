// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Animation;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Beat
{
    public class BeatAnimations : ObservableObject
    {
        public BeatAnimations()
        {
            AnimatedDoubles = new ObservableCollection<AnimatedDouble>();
            Storyboard = new Storyboard();
        }

        private ObservableCollection<AnimatedDouble> _animatedDoubles;
        public ObservableCollection<AnimatedDouble> AnimatedDoubles
        {
            get => _animatedDoubles;
            set => _animatedDoubles = value;
        }


        private Storyboard _storyboard;
        public Storyboard Storyboard
        {
            get => _storyboard;
            set => SetProperty(ref _storyboard, value);
        }

        public void MakeStoryBoard(float[] periods)
        {
            Storyboard.Children.Clear();
            AnimatedDoubles.Clear();

            for (var i = 0; i < periods.Length; i++)
            {
                Storyboard.Children.Add(CreateAnimation(periods[i]));
            }

            Storyboard.RepeatBehavior = RepeatBehavior.Forever;
            Storyboard.Begin();
        }

        private DoubleAnimation CreateAnimation(double period)
        {
            var animatedDouble = new AnimatedDouble();
            AnimatedDoubles.Add(animatedDouble);
            var newda = new DoubleAnimation();

            if (period > 0)
            {
                newda.From = 1;
                newda.To = 0;
                var easing = new QuadraticEase();
                easing.EasingMode = System.Windows.Media.Animation.EasingMode.EaseOut;
                newda.EasingFunction = easing;
                newda.Duration = new Duration(TimeSpan.FromMilliseconds(period));
                newda.RepeatBehavior = RepeatBehavior.Forever;
            }

            Storyboard.SetTarget(newda, animatedDouble);
            Storyboard.SetTargetProperty(newda, new PropertyPath(AnimatedDouble.AnimationPositionProperty));
            return newda;
        }

        public void ResetAnimation()
        {
            Storyboard.Stop();
            Storyboard.Begin();
        }
    }
}
