// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Animations
{
    public static class BeatHelper
    {
        public static float CalculateBPM(float period)
            => period > 0 ? (float)Math.Round(60000f / period, 2) : 0f;
    }
}
