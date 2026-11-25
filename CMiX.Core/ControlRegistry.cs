// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core
{
    // Every receivable control registers here, and Find gives the one that answers to an id.
    public class ControlRegistry
    {
        private readonly List<ReceivableControl> _controls = new();
        private readonly Dictionary<Guid, WeakReference<ReceivableControl>> _byId = new();
        private int _sizeAtLastPurge;

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

        // The control that answers to this id, or null when it is inactive or gone.
        public ReceivableControl Find(Guid id) =>
            _byId.TryGetValue(id, out var weak) && weak.TryGetTarget(out var control) && control.IsActive ? control : null;

        // A receiver calls this when its id is set. The old key goes, the new key points to the control.
        public void Rekey(ReceivableControl control, Guid oldId, Guid newId)
        {
            if (oldId != Guid.Empty && _byId.TryGetValue(oldId, out var old) && old.TryGetTarget(out var same) && ReferenceEquals(same, control))
                _byId.Remove(oldId);
            if (newId != Guid.Empty)
                _byId[newId] = new WeakReference<ReceivableControl>(control);
            if (_byId.Count > 2 * _sizeAtLastPurge + 64)
                Purge();
        }

        // Drops the entries whose control is gone.
        private void Purge()
        {
            foreach (var key in _byId.Where(pair => !pair.Value.TryGetTarget(out _)).Select(pair => pair.Key).ToList())
                _byId.Remove(key);
            _sizeAtLastPurge = _byId.Count;
        }
    }
}
