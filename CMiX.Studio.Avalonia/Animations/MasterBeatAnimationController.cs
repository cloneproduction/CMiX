// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;

namespace CMiX.Studio.Avalonia.Animations
{
    public class MasterBeatAnimationController
    {
        private readonly MasterBeat _masterBeat;
        private readonly BeatAnimations _beatAnimations;

        public MasterBeatAnimationController(MasterBeat masterBeat)
        {
            _masterBeat = masterBeat;
            _beatAnimations = new BeatAnimations();

            // Only the period notification is listened for. The second clause used to also test for
            // BeatIndex, which the master beat never raises for itself: its BeatIndex is a
            // GenericValue that raises on its own instance. Nothing is lost by dropping it, since
            // every path that moves the index raises the period change on the way through.
            _masterBeat.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName != nameof(MasterBeat.Periods))
                    return;

                // Raised by a tap, a BPM entry and a load, which change the period table, and by a
                // multiply or a divide, which only move the index into it. MakeStoryBoard leaves a
                // matching table standing, so a tempo click ends up only repointing this controller
                // at another slot of the storyboard it already has.
                _beatAnimations.MakeStoryBoard(_masterBeat.Periods);
                UpdateAnimatedDouble();
            };

            _beatAnimations.MakeStoryBoard(_masterBeat.Periods);
            _masterBeat.AnimatedDoubleProvider = index =>
                index >= 0 && index < _beatAnimations.AnimatedDoubles.Count
                    ? _beatAnimations.AnimatedDoubles[index]
                    : null;
            UpdateAnimatedDouble();
        }

        private void UpdateAnimatedDouble()
        {
            int midIndex = _masterBeat.Index.Value + (_masterBeat.Periods.Length - 1) / 2;
            if (_beatAnimations.AnimatedDoubles.Count > midIndex)
            {
                _masterBeat.AnimatedDouble = _beatAnimations.AnimatedDoubles[midIndex];
            }
        }
    }
}
