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

            _masterBeat.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(MasterBeat.Periods))
                {
                    _beatAnimations.MakeStoryBoard(_masterBeat.Periods);
                }

                if (e.PropertyName == nameof(MasterBeat.Periods) ||
                    e.PropertyName == nameof(MasterBeat.BeatIndex))
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
