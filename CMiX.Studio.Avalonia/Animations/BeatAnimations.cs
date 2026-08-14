// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Studio.Avalonia.Animations
{
    // Clock driven replacement for the WPF Storyboard of infinite DoubleAnimations.
    // The WPF animation ran From 1 To 0 with a QuadraticEase in EaseOut mode, so the
    // value at normalized time t is the closed form (1 - t) squared.
    public class BeatAnimations : ObservableObject
    {
        private readonly Stopwatch _stopwatch = new();
        private readonly DispatcherTimer _timer;

        public BeatAnimations()
        {
            AnimatedDoubles = new ObservableCollection<AnimatedDouble>();
            _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(16), DispatcherPriority.Render, OnTick);
        }

        public ObservableCollection<AnimatedDouble> AnimatedDoubles { get; set; }

        // Keeps the WPF method name so the controller ports verbatim.
        public void MakeStoryBoard(float[] periods)
        {
            AnimatedDoubles.Clear();

            for (var i = 0; i < periods.Length; i++)
                AnimatedDoubles.Add(new AnimatedDouble(periods[i]));

            _stopwatch.Restart();
            _timer.Start();
        }

        public void ResetAnimation()
        {
            _stopwatch.Restart();
        }

        private void OnTick(object sender, EventArgs e)
        {
            var elapsedMs = _stopwatch.Elapsed.TotalMilliseconds;

            foreach (var animatedDouble in AnimatedDoubles)
            {
                if (animatedDouble.Period <= 0)
                    continue;

                var frac = elapsedMs % animatedDouble.Period / animatedDouble.Period;
                animatedDouble.AnimationPosition = (1 - frac) * (1 - frac);
            }
        }
    }
}
