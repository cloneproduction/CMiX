// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Studio.Avalonia.Animations
{
    public class BeatAnimations : ObservableObject
    {
        private readonly Stopwatch _stopwatch = new();
        private readonly DispatcherTimer _timer;

        public BeatAnimations()
        {
            AnimatedDoubles = new ObservableCollection<AnimatedDouble>();
            _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(16), DispatcherPriority.Render, OnTick);
            _timer.Stop();
        }

        public ObservableCollection<AnimatedDouble> AnimatedDoubles { get; set; }

        public void MakeStoryBoard(float[] periods)
        {
            if (MatchesCurrentStoryBoard(periods))
                return;

            foreach (var animatedDouble in AnimatedDoubles)
                animatedDouble.ObservationStarted -= OnObservationStarted;

            AnimatedDoubles.Clear();

            for (var i = 0; i < periods.Length; i++)
            {
                var animatedDouble = new AnimatedDouble(periods[i]);
                animatedDouble.ObservationStarted += OnObservationStarted;
                AnimatedDoubles.Add(animatedDouble);
            }

            _stopwatch.Restart();
            StartIfObserved();
        }

        private bool MatchesCurrentStoryBoard(float[] periods)
        {
            if (periods == null || periods.Length != AnimatedDoubles.Count)
                return false;

            for (var i = 0; i < periods.Length; i++)
            {
                if (AnimatedDoubles[i].Period != periods[i])
                    return false;
            }

            return true;
        }

        public void ResetAnimation()
        {
            _stopwatch.Restart();
        }

        public bool IsRunning => _timer.IsEnabled;

        private void OnObservationStarted(object? sender, EventArgs e)
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

        private void OnTick(object? sender, EventArgs e)
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
