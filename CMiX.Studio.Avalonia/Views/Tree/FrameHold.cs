// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views
{
    // A new claim ends the hold before, so two holds of one owner never run together.
    internal sealed class FrameHold
    {
        // A safe end, in case a panel never reaches its end.
        private const int MaxFrames = 120;

        private int token;

        public Func<bool> Claim()
        {
            var id = ++token;
            return () => id == token;
        }

        public void Run(TopLevel top, Action step, Func<bool> keepGoing)
        {
            var isCurrent = Claim();
            var frames = 0;
            void Keep(TimeSpan _)
            {
                if (!isCurrent())
                    return;

                step();
                if (keepGoing() && ++frames < MaxFrames)
                    top.RequestAnimationFrame(Keep);
            }

            top.RequestAnimationFrame(Keep);
        }

        public void Stop() => token++;
    }
}
