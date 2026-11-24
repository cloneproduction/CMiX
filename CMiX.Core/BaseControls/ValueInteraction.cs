// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.BaseControls
{
    // The one open drag of the Studio. The sliders open it, the value syncs enlist in it.
    public sealed class ValueInteraction
    {
        private readonly List<IInteractiveValue> _participants = new();
        private long _openScopeToken;
        private long _lastToken;

        public bool IsActive { get; private set; }

        public ValueInteractionScope BeginScope()
        {
            End();
            _openScopeToken = ++_lastToken;
            IsActive = true;
            return new ValueInteractionScope(this, _openScopeToken);
        }

        public void Enlist(IInteractiveValue participant)
        {
            if (!IsActive || participant == null) return;
            if (!_participants.Contains(participant))
                _participants.Add(participant);
        }

        internal void End()
        {
            if (!IsActive) return;
            IsActive = false;
            _openScopeToken = 0;

            for (int i = 0; i < _participants.Count; i++)
                _participants[i].EndInteraction();

            _participants.Clear();
        }

        internal void End(long token)
        {
            if (token == 0 || token != _openScopeToken) return;
            End();
        }
    }
}
