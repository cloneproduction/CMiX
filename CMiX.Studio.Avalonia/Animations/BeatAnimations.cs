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
            // This constructor overload starts the timer, which the clock must not do before
            // something observes a position.
            _timer.Stop();
        }

        public ObservableCollection<AnimatedDouble> AnimatedDoubles { get; set; }

        // Keeps the WPF method name so the controller ports verbatim.
        public void MakeStoryBoard(float[] periods)
        {
            foreach (var animatedDouble in AnimatedDoubles)
                animatedDouble.ObservationStarted -= OnObservationStarted;

            AnimatedDoubles.Clear();

            for (var i = 0; i < periods.Length; i++)
            {
                var animatedDouble = new AnimatedDouble(periods[i]);
                animatedDouble.ObservationStarted += OnObservationStarted;
                AnimatedDoubles.Add(animatedDouble);
            }

            // The positions are a closed form of the elapsed time, so stopping and restarting the
            // timer never shifts the phase. Only this restart does, exactly as it did before.
            _stopwatch.Restart();
            StartIfObserved();
        }

        public void ResetAnimation()
        {
            _stopwatch.Restart();
        }

        // Exposed so a diagnostic or a test can tell an idle clock from a running one.
        public bool IsRunning => _timer.IsEnabled;

        private void OnObservationStarted(object sender, EventArgs e)
        {
            if (!_timer.IsEnabled)
                _timer.Start();
        }

        private void StartIfObserved()
        {
            foreach (var animatedDouble in AnimatedDoubles)
            {
                if (!animatedDouble.IsObserved) continue;
                if (!_timer.IsEnabled) _timer.Start();
                return;
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            var elapsedMs = _stopwatch.Elapsed.TotalMilliseconds;
            var anyObserved = false;

            foreach (var animatedDouble in AnimatedDoubles)
            {
                if (!animatedDouble.IsObserved)
                    continue;

                anyObserved = true;

                if (animatedDouble.Period <= 0)
                    continue;

                var frac = elapsedMs % animatedDouble.Period / animatedDouble.Period;
                animatedDouble.AnimationPosition = (1 - frac) * (1 - frac);
            }

            if (!anyObserved)
                _timer.Stop();
        }
    }
}
