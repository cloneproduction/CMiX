// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Animations
{
    public static class BeatHelper
    {
        public static float CalculateBPM(float period)
            => period > 0 ? (float)Math.Round(60000f / period, 2) : 0f;
    }
}
