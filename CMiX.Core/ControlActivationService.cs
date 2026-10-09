// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core
{
    public class ControlActivationService
    {
        private readonly List<ReceivableControl> _controls = new();

        public void Register(ReceivableControl control)
        {
            if (!control.IsActive && !_controls.Contains(control))
                _controls.Add(control);
        }

        public void ActivateAll()
        {
            _controls.ForEach(c => c.Activate());
            _controls.Clear();
        }

        public void Clear() => _controls.Clear();

        // The controls built since the last ActivateAll.
        public IReadOnlyList<ReceivableControl> Pending => _controls;
        public int PendingCount => _controls.Count;
    }
}
