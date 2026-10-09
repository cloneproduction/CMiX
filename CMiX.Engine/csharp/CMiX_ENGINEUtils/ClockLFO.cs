// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX_ENGINEUtils
{
    [ProcessNode]
    public class ClockLFO
    {
        private DateTime _referenceTime = DateTime.UtcNow;
        private DateTime? _pausedAt;
        private int _lastCycle;

        public float Update(float periodSeconds, bool pause, bool reset, out bool onNewCycle, out int cycleCount)
        {
            if (reset)
            {
                _referenceTime = DateTime.UtcNow;
                _pausedAt = null;
                _lastCycle = 0;
            }

            if (pause && _pausedAt == null)
            {
                _pausedAt = DateTime.UtcNow;
            }
            else if (!pause && _pausedAt != null)
            {
                _referenceTime += DateTime.UtcNow - _pausedAt.Value;
                _pausedAt = null;
            }

            var now = _pausedAt ?? DateTime.UtcNow;
            var totalCycles = (now - _referenceTime).TotalSeconds / periodSeconds;
            var currentCycle = (int)Math.Floor(totalCycles);

            onNewCycle = currentCycle > _lastCycle;
            cycleCount = currentCycle;
            _lastCycle = currentCycle;

            return (float)(totalCycles - currentCycle);
        }
    }
}
